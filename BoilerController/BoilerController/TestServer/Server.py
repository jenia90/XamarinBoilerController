#!/usr/bin/python3

from flask import Flask, request
from datetime import datetime, timedelta
import json
import sqlite3
import re
import RPi.GPIO as GPIO
from apscheduler.schedulers.background import BackgroundScheduler

ID_IDX = 0
PIN_IDX = 1
START_IDX = 3
END_IDX = 2
TYPE_IDX = 4
DAYS_IDX = 5

OnState = GPIO.HIGH
OffState = GPIO.LOW

# OnState = True
# OffState = False

scheduler = BackgroundScheduler()

app = Flask(__name__)

pins = {17: {'name': 'LED', 'state': OffState}}

db = sqlite3.connect('boiler.db')


def setup_gpio():
    GPIO.setwarnings(False)
    GPIO.setmode(GPIO.BCM)
    GPIO.setup([17], GPIO.OUT)
    GPIO.output([17], GPIO.LOW)


def add_scheduled_job(type, start, end, days=''):
    if type == 'datetime':
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
    elif type == 'cron':
        scheduler.add_job(set_state, 'cron', id=str(start),
                          hour=start.hour, minute=start.minute,
                          day_of_week=days,
                          misfire_grace_time=60,
                          args=['1'])
        scheduler.add_job(set_state, 'cron', id=str(end),
                          hour=end.hour, minute=end.minute,
                          day_of_week=days,
                          args=['0'],
                          misfire_grace_time=60)


def update_jobs_from_db():
    c = db.cursor()
    c.execute("SELECT * FROM schedule")
    for job in c.fetchall():
        start = datetime.strptime(job[3], "%Y-%m-%d %H:%M")
        end = datetime.strptime(job[2], "%Y-%m-%d %H:%M")

        add_scheduled_job(job[4], start, end, job[5])
        if end > datetime.now():
            set_state('1')


@app.route('/api/remove')
def delete_item():
    id = request.args['id']

    curs = db.cursor()
    curs.execute("SELECT * FROM schedule WHERE ID=" + str(id))
    _, _, end, start, _, _ = curs.fetchone()
    curs.execute(
            "DELETE FROM schedule WHERE ID=?;", (id,))
    db.commit()
    curs.execute("SELECT * FROM schedule")
    if len(curs.fetchall()) == 0:
        curs.execute("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE "
                     "NAME='schedule';")
        db.commit()

    scheduler.remove_job(job_id=start + ':00')
    scheduler.remove_job(job_id=end + ':00')

    start = datetime.strptime(start, "%Y-%m-%d %H:%M")
    end = datetime.strptime(end, "%Y-%m-%d %H:%M")
    if end > datetime.now() > start:
        set_state('0')

    return 'OK', 200


def set_state(state=None):
    if state == '1':
        pins[17]['state'] = OnState
    elif state == '0' or state is None:
        pins[17]['state'] = OffState

    GPIO.output(17, pins[17]['state'])


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
            add_scheduled_job(sched_type, jobstart, jobend)
        except Exception as e:
            db.commit()
            print(e)
            return 'BAD', 500
    else:
        return 'BAD', 500
    return 'OK', 200


@app.route('/api/addcron', methods=['POST'])
def add_cron_job():
    j = request.json
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
    curs = db.cursor()
    lst = []
    try:
        curs.execute("SELECT * FROM schedule")
        query = curs.fetchall()
    except:
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
def set_led():
    num = int(request.args['dev'])
    state = request.args['state']
    if state == '1':
        pins[num]['state'] = OnState
    elif state == '0':
        pins[num]['state'] = OffState
    else:
        return 'BAD', 500
    GPIO.output(num, pins[num]['state'])
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
    setup_gpio()
    update_jobs_from_db()
    scheduler.start()
    app.run(host='0.0.0.0')
    db.close()
