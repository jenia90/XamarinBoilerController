from flask import Flask, request
from datetime import datetime
import json
import sqlite3
from apscheduler.schedulers.background import BackgroundScheduler

OnState = True  # Replace with GPIO.HIGH
OffState = False  # Replace With GPIO.LOW

# OnState = GPIO.HIGH
# OffState = GPIO.LOW

scheduler = BackgroundScheduler({
    'apscheduler.jobstores.default': {
        'type'     : 'sqlalchemy',
        'tablename': 'scheduler_jobs',
        'url'      : 'sqlite:///boiler.db'
    },
})

app = Flask(__name__)

pins = {17: {'name': 'LED', 'state': OffState}}

db = sqlite3.connect('boiler.db')


@app.route('/api/remove')
def delete_item():
    id = request.args['id']

    curs = db.cursor()
    try:
        curs.execute(
                "DELETE FROM schedule WHERE ID=?;", (id,))
        db.commit()
        curs.execute("SELECT * FROM schedule")
        if len(curs.fetchall()) == 0:
            curs.execute("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE "
                         "NAME='schedule';")
            db.commit()

        scheduler.remove_job(job_id=id, jobstore='default')
    except Exception as e:
        print(e)
        return 'BAD'
    return 'OK'


def set_state(state):
    if state == '1':
        pins[17]['state'] = OnState
    elif state == '0':
        pins[17]['state'] = OffState


@app.route('/api/settime')
def set_time():
    pin = request.args['dev']
    start = request.args['ondate'] + " " + request.args['ontime']
    end = request.args['offdate'] + " " + request.args['offtime']

    curs = db.cursor()
    try:
        curs.execute("INSERT INTO schedule (dev, turnon, turnoff) "
                     "VALUES (?,?,?);",
                     (pin, start, end))
        db.commit()

        scheduler.add_job(set_state, 'date',
                          args=['1'],
                          next_run_time=start)
        scheduler.add_job(set_state, 'date', next_run_time=end, args=['0'])
        print(scheduler.get_jobs())
    except Exception as e:
        print(e)
        return 'BAD'
    return 'OK'


@app.route('/api/gettimes')
def get_times():
    curs = db.cursor()
    try:
        curs.execute("SELECT * FROM schedule")
        query = curs.fetchall()
    except:
        return 'BAD'

    return json.dumps([{'ID'   : q[0],
                        'pin'  : q[1],
                        'start': q[2],
                        'end'  : q[3]}
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
        return 'BAD'
    # GPIO.output(num, pins[num]['state'])
    return 'OK'


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
    scheduler.start()
    app.run(host='0.0.0.0')
    db.close()
