#!/usr/bin/python3

from flask import Flask, request
from datetime import datetime, timedelta
import json
import sqlite3
import RPi.GPIO as GPIO
from apscheduler.schedulers.background import BackgroundScheduler
from threading import Timer

ON_STATE = '1'
OFF_STATE = '0'

ID_IDX = 0
PIN_IDX = 1
START_IDX = 3
END_IDX = 2
TYPE_IDX = 4
DAYS_IDX = 5

# production DB location
# DATABASE = '/home/pi/BoilerServer/boiler.db'

# dev server db location
DATABASE = 'boiler.db'
API_KEY = 'Basic LC9BhYqKWXAlduiwu0fUgr8ZwW6GSbRUz1pOMWh2+NM='

OnState = GPIO.HIGH
OffState = GPIO.LOW

scheduler = BackgroundScheduler()

app = Flask(__name__)
db = sqlite3.connect(DATABASE)

pins = {17: {'name': 'BOILER', 'state': OffState}}

lastStart = ""
nextEnd = ''


def manual_override(channel):
    start = datetime.now()
    end = datetime.now() + timedelta(hours=2)

    global lastStart
    global nextEnd
    if pins[17]['state'] == OffState:
        pins[17]['state'] = OnState
        add_scheduled_job('datetime', start, end)
        nextEnd = str(end)
        lastStart = str(datetime.now())
    elif pins[17]['state'] == OnState:
        pins[17]['state'] = OffState
        scheduler.remove_job(nextEnd)

    GPIO.output(17, pins[17]['state'])


def setup_gpio():
    GPIO.setmode(GPIO.BCM)
    GPIO.setwarnings(False)
    GPIO.setup(17, GPIO.OUT)
    GPIO.setup(23, GPIO.IN, pull_up_down=GPIO.PUD_UP)
    GPIO.output(17, GPIO.LOW)
    GPIO.add_event_detect(23, GPIO.FALLING,
                          callback=manual_override, bouncetime=300)


def add_scheduled_job(type, start, end, days=''):
    if type == 'datetime':
        if datetime.now() < start:  # add start of future job
            scheduler.add_job(set_state, 'date', id=str(start),
                              run_date=start,
                              misfire_grace_time=60,
                              args=[ON_STATE])
        # add future end
        scheduler.add_job(set_state, 'date', id=str(end),
                          run_date=end,
                          args=[OFF_STATE],
                          misfire_grace_time=60)
    elif type == 'cron':
        scheduler.add_job(set_state, 'cron', id=str(start),
                          hour=start.hour, minute=start.minute,
                          day_of_week=days,
                          misfire_grace_time=60,
                          args=[ON_STATE])
        scheduler.add_job(set_state, 'cron', id=str(end),
                          hour=end.hour, minute=end.minute,
                          day_of_week=days,
                          args=[OFF_STATE],
                          misfire_grace_time=60)


def update_jobs_from_db():
    cur_hour = datetime.now().hour
    cur_min = datetime.now().minute
    try:
        c = db.cursor()
        c.execute("SELECT * FROM schedule")
        for job in c.fetchall():
            start = datetime.strptime(job[START_IDX], "%Y-%m-%d %H:%M")
            end = datetime.strptime(job[END_IDX], "%Y-%m-%d %H:%M")

            add_scheduled_job(job[TYPE_IDX], start, end, job[DAYS_IDX])
            if end.hour >= cur_hour >= start.hour and \
                                    end.minute > cur_min >= start.minute:
                set_state(ON_STATE)
    except sqlite3.DatabaseError as e:
        print(e)


@app.route('/api/remove', methods=['DELETE'])
def delete_item():
    key = request.headers['authorization']
    if key != API_KEY:
        return 'Unauthorized', 401
    id = request.args['id']
    if id is None:
        return 'BAD', 500

    try:
        curs = db.cursor()
        curs.execute("SELECT * FROM schedule WHERE ID=" + str(id))
        _, _, end, start, _, _ = curs.fetchone()
        curs.execute(
                "DELETE FROM schedule WHERE ID=?;", (id,))
        db.commit()
        curs.execute("SELECT * FROM schedule")
        # Check if the table is empty, in which case we reset the ID field.
        if len(curs.fetchall()) == 0:
            curs.execute("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE "
                         "NAME='schedule';")
            db.commit()
    except sqlite3.DatabaseError as e:
        print(e)

    try:
        scheduler.remove_job(job_id=start + ':00')
        scheduler.remove_job(job_id=end + ':00')
    except Exception as e:
        print(e)

    start = datetime.strptime(start, "%Y-%m-%d %H:%M")
    end = datetime.strptime(end, "%Y-%m-%d %H:%M")
    if end > datetime.now() > start and pins[17]['state'] == OnState:
        set_state(OFF_STATE)

    return 'OK', 200


def set_state(state=None):
    """
    Sets desired state
    :param state: desired state string
    :return:
    """
    global lastStart
    if state == ON_STATE:

        pins[17]['state'] = OnState
        lastStart = str(datetime.now())
    elif state == OFF_STATE or state is None:
        pins[17]['state'] = OffState
        lastStart = ''

    GPIO.output(17, pins[17]['state'])


@app.route('/api/settime', methods=['POST'])
def set_time():
    """
    Adds one-time scheduled job
    :return:
    """
    j = request.json
    if len(j) < 5:
        return 'BAD', 500
    key = request.headers['authorization']
    if key != API_KEY:
        return 'Unauthorized', 401
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
            start = datetime.strptime(start, "%Y-%m-%d %H:%M")
            end = datetime.strptime(end, "%Y-%m-%d %H:%M")
            add_scheduled_job(sched_type, start, end)
        except Exception as e:
            db.commit()
            print(e)
            return 'BAD', 500
    else:
        return 'BAD', 500
    return 'OK', 200


@app.route('/api/addcron', methods=['POST'])
def add_cron_job():
    """
    Adds recurring scheduled job
    """
    j = request.json
    if len(j) < 6:
        return 'BAD', 500

    key = request.headers['authorization']
    if key != API_KEY:
        return 'Unauthorized', 401

    pin = j['pin']
    start = j['start']
    end = j['end']
    sched_type = j['type']
    days = ','.join(j['days'])

    if sched_type == 'cron':
        curs = db.cursor()

        try:
            curs.execute("INSERT INTO schedule (dev,  turnon, turnoff, type, "
                         "daysofweek) VALUES (?,?,?,?,?);",
                         (pin, start, end, sched_type, days))
            db.commit()
            jobstart = datetime.strptime(start, "%Y-%m-%d %H:%M")
            jobend = datetime.strptime(end, "%Y-%m-%d %H:%M")
            add_scheduled_job(sched_type, jobstart, jobend, days)
        except Exception as e:
            db.commit()
            print(e)
            return 'BAD', 500
    else:
        return 'BAD', 500

    return 'OK', 200


@app.route('/api/gettimes')
def get_times():
    """
    Replies a list of scheduled jobs
    """
    key = request.headers['authorization']
    if key != API_KEY:
        return 'Unauthorized', 401
    curs = db.cursor()
    lst = []
    try:
        curs.execute("SELECT * FROM schedule")
        query = curs.fetchall()
    except sqlite3.DatabaseError:
        return 'BAD', 500

    for q in query:
        days = q[DAYS_IDX]
        if days is None:
            days = []
        elif ',' in days:
            days = days.split(',')

        lst.append({'ID'   : q[ID_IDX],
                    'pin'  : q[PIN_IDX],
                    'start': q[START_IDX],
                    'end'  : q[END_IDX],
                    'type' : q[TYPE_IDX],
                    'days' : days})

    return json.dumps(lst)


@app.route('/api/setstate')
def remote_set_state():
    """
    Sets working state
    """
    key = request.headers['authorization']
    if key != API_KEY:
        return 'Unauthorized', 401

    num = int(request.args['dev'])
    state = request.args['state']

    start = datetime.now()
    end = datetime.now() + timedelta(hours=2)

    global lastStart
    global nextEnd

    if state == ON_STATE:
        pins[num]['state'] = OnState
        add_scheduled_job('datetime', start, end)
        nextEnd = str(end)
        lastStart = str(datetime.now())
    elif state == OFF_STATE:
        pins[num]['state'] = OffState
        scheduler.remove_job(nextEnd)
        lastStart = ''
    else:
        return 'BAD', 500
    GPIO.output(num, pins[num]['state'])
    return 'OK', 200


@app.route('/api/getstate')
def remote_get_state():
    """
    Replies with the current working state
    :return: json string with the state.
    """
    key = request.headers['authorization']
    if key != API_KEY:
        return 'Unauthorized', 401

    num = int(request.args['dev'])
    if num is None:
        return 'BAD', 500

    if pins[num]['state'] == OnState:
        return json.dumps({'state': 'On', 'on_since': lastStart})
    return json.dumps({'state': 'Off', 'on_since': None})


if __name__ == '__main__':
    setup_gpio()
    update_jobs_from_db()
    scheduler.start()
    app.run(host='0.0.0.0')
    GPIO.cleanup()
    db.close()
