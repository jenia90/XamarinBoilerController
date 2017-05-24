from flask import Flask, request
from datetime import datetime, timedelta
import json
import sqlite3
import re
from apscheduler.schedulers.background import BackgroundScheduler

OnState = True  # Replace with GPIO.HIGH
OffState = False  # Replace With GPIO.LOW

# OnState = GPIO.HIGH
# OffState = GPIO.LOW

scheduler = BackgroundScheduler()

app = Flask(__name__)

pins = {17: {'name': 'LED', 'state': OffState}}

db = sqlite3.connect('boiler.db')


@app.route('/api/remove')
def delete_item():
    id = request.args['id']

    curs = db.cursor()
    curs.execute("SELECT * FROM schedule WHERE ID=" + str(id))
    _, _, end, start, _ = curs.fetchone()
    curs.execute(
            "DELETE FROM schedule WHERE ID=?;", (id,))
    db.commit()
    curs.execute("SELECT * FROM schedule")
    if len(curs.fetchall()) == 0:
        curs.execute("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE "
                     "NAME='schedule';")
        db.commit()
    scheduler.remove_job(job_id=start)
    scheduler.remove_job(job_id=end)

    start = datetime.strptime(start, "%Y-%m-%d %H:%M")
    end = datetime.strptime(end, "%Y-%m-%d %H:%M")
    if end > datetime.now() > start:
        set_state('0')

    return 'OK', 200


def update_jobs_from_db():
    c = db.cursor()
    c.execute("SELECT * FROM schedule")
    for job in c.fetchall():
        start = datetime.strptime(job[3], "%Y-%m-%d %H:%M")
        end = datetime.strptime(job[2], "%Y-%m-%d %H:%M")
        if datetime.now() < end:
            if datetime.now() < start:  # add start of future job
                scheduler.add_job(set_state, 'date', id=str(start),
                                  run_date=start,
                                  misfire_grace_time=60,
                                  args=['1'])
            # add future end
            scheduler.add_job(set_state, 'date', id=str(end),
                              run_date=end,
                              args=['0'],
                              misfire_grace_time=60)
            if end > start:
                set_state('1')


def set_state(state=None):
    if state == '1':
        pins[17]['state'] = OnState
    elif state == '0':
        pins[17]['state'] = OffState

    elif state is None:
        pins[17]['state'] = not pins[17]['state']


@app.route('/api/settime', methods=['POST'])
def set_time():
    j = request.json
    pin = j['pin']
    start = j['start']
    end = j['end']
    sched_type = j['type']

    if sched_type == 'datetime':
        curs = db.cursor()
        try:
            curs.execute("INSERT INTO schedule (dev, turnon, turnoff, type) "
                         "VALUES (?,?,?,?);",
                         (pin, start, end, sched_type))
            db.commit()
            jobstart = datetime.strptime(start, "%Y-%m-%d %H:%M")
            jobend = datetime.strptime(end, "%Y-%m-%d %H:%M")
            scheduler.add_job(set_state, 'date', id=start,
                              run_date=jobstart,
                              misfire_grace_time=60,
                              args=['1'])
            scheduler.add_job(set_state, 'date', id=end,
                              run_date=jobend,
                              args=['0'],
                              misfire_grace_time=60)
        # print(scheduler.get_jobs())
        except Exception as e:
            db.commit()
            print(e)
            return 'BAD', 500
    else:
        return'BAD', 500
    return 'OK', 200


@app.route('/api/addcron')
def add_cron_job():
    j = request.json
    pin = j['pin']
    start = j['start']
    end = j['end']
    sched_type = j['type']



@app.route('/api/gettimes')
def get_times():
    curs = db.cursor()
    try:
        curs.execute("SELECT * FROM schedule")
        query = curs.fetchall()
    except:
        return 'BAD', 500

    return json.dumps([{'ID'   : q[0],
                        'pin'  : q[1],
                        'start': q[3],
                        'end'  : q[2]}
                       for q in query])


@app.route('/api/setstate')
def set_led():
    num = int(request.args['dev'])
    state = request.args['state']
    if state == '1':
        pins[num]['state'] = OnState
    elif state == '0':
        pins[num]['state'] = OffState
    else:
        return 'BAD', 500
    # GPIO.output(num, pins[num]['state'])
    return 'OK', 200


@app.route('/api/getstate')
def get_led():
    num = int(request.args['dev'])
    if pins[num]['state'] == OnState:
        return 'On'
    return 'Off'


@app.route('/')
def default_route():
    return 'Please use correct commands.'


if __name__ == '__main__':
    update_jobs_from_db()
    scheduler.start()
    app.run(host='0.0.0.0')
    db.close()
