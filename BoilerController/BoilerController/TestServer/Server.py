#!/usr/bin/python3

from flask import Flask, request
from datetime import datetime
import json
import sqlite3
from apscheduler.schedulers.background import BackgroundScheduler
import logging
from logging.handlers import RotatingFileHandler
import base64

from DeviceManager import DeviceManager

DT_FORMAT = "%Y-%m-%d %H:%M"


ID_IDX = 0
PIN_IDX = 1
START_IDX = 3
END_IDX = 2
TYPE_IDX = 4
DAYS_IDX = 5

ON_STATE = '1'
OFF_STATE = '0'

BOILER_PIN = 17
HEATER_STATUS_PIN = 23
RELAY_STATUS_PIN = 24


PROD = True

if PROD:
    # production strings
    DATABASE = '/home/pi/BoilerServer/boiler.db'
    LOG_PATH = 'boilerserver.log'
else:
    # # dev server db location
    DATABASE = 'boiler.db'
    LOG_PATH = 'boilerserver.log'

scheduler = BackgroundScheduler()
log = None
app = Flask(__name__)
db = sqlite3.connect(DATABASE)
boiler = DeviceManager('Boiler',
                       BOILER_PIN,
                       HEATER_STATUS_PIN,
                       RELAY_STATUS_PIN)


def is_auth(creds):
    decoded = base64.b64decode(creds.split()[1]).decode('utf-8')
    username, password = decoded.split(':')

    try:
        curs = db.cursor()
        curs.execute('SELECT * FROM users WHERE Username=?',
                     (username,))
        user = curs.fetchone()
        if base64.b64decode(user[3]).decode('utf-8') == password:
            return True
    except db.DatabaseError as e:
        print(e)
    return len(curs.fetchall()) > 0


def get_jobs_from_db():
    cur_hour = datetime.now().hour
    cur_min = datetime.now().minute
    try:
        c = db.cursor()
        c.execute("SELECT * FROM schedule")
        for job in c.fetchall():
            start = datetime.strptime(job[START_IDX], DT_FORMAT)
            end = datetime.strptime(job[END_IDX], DT_FORMAT)
            if end <= datetime.now():
                remove_job_from_db(job[ID_IDX])
                continue

            boiler.add_scheduled_job(job[TYPE_IDX], start, end,
                                   job[DAYS_IDX])
            if end.hour >= cur_hour >= start.hour and \
                                    end.minute > cur_min >= start.minute:
                boiler.set_state(ON_STATE, start, end)
    except sqlite3.DatabaseError as e:
        print(e)


def remove_job_from_db(id):
    try:
        curs = db.cursor()
        curs.execute("DELETE FROM schedule WHERE ID=?;", (id,))
        db.commit()
        curs.execute("SELECT * FROM schedule")
        # Check if the table is empty, in which case we reset the ID field.
        if len(curs.fetchall()) == 0:
            curs.execute("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE "
                         "NAME='schedule';")
            db.commit()
    except sqlite3.DatabaseError as e:
        print(e)


@app.route('/api/remove', methods=['DELETE'])
def delete_item():
    if not is_auth(request.headers['authorization']):
        return 'Unauthorized', 401

    id = request.args['id']
    if id is None:
        return 'BAD', 500

    try:
        curs = db.cursor()
        curs.execute("SELECT * FROM schedule WHERE ID=" + str(id))
        _, _, end, start, _, _ = curs.fetchone()
        remove_job_from_db(id)
    except sqlite3.DatabaseError as e:
        print(e)

    boiler.remove_job(start, end)

    start = datetime.strptime(start, DT_FORMAT)
    end = datetime.strptime(end, DT_FORMAT)
    if end > datetime.now() > start and boiler.get_state():
        boiler.set_state(OFF_STATE)

    return 'OK', 200


@app.route('/api/settime', methods=['POST'])
def set_time():
    """
    Adds one-time scheduled job
    :return:
    """
    j = request.json
    if len(j) < 4:
        return 'BAD', 500
    if not is_auth(request.headers['authorization']):
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
            start = datetime.strptime(start, DT_FORMAT)
            end = datetime.strptime(end, DT_FORMAT)
            boiler.add_scheduled_job(sched_type, start, end)
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
    if len(j) < 5:
        return 'BAD', 500

    if not is_auth(request.headers['authorization']):
        return 'Unauthorized', 401

    pin = j['pin']
    start = j['start']
    end = j['end']
    sched_type = j['type']
    days = ','.join(j['days'])

    if sched_type == 'cron':
        curs = db.cursor()

        try:
            curs.execute("INSERT INTO schedule "
                         "(dev,  turnon, turnoff, type, daysofweek) "
                         "VALUES (?,?,?,?,?);",
                         (pin, start, end, sched_type, days))
            db.commit()
            jobstart = datetime.strptime(start, DT_FORMAT)
            jobend = datetime.strptime(end, DT_FORMAT)
            boiler.add_scheduled_job(sched_type, jobstart, jobend, days)
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
    if not is_auth(request.headers['authorization']):
        return 'Unauthorized', 401
    curs = db.cursor()
    try:
        curs.execute("SELECT * FROM schedule")
        query = curs.fetchall()
    except sqlite3.DatabaseError:
        return 'BAD', 500

    lst = []
    for q in query:
        days = q[DAYS_IDX]
        if days is None:
            days = []
        else:
            days = days.split(',')

        lst.append({'ID'   : q[ID_IDX],
                    'pin'  : q[PIN_IDX],
                    'dev'  : boiler.get_name(),
                    'start': q[START_IDX],
                    'end'  : q[END_IDX],
                    'type' : q[TYPE_IDX],
                    'days' : days})
    print(lst)
    return json.dumps(lst)


@app.route('/api/setstate')
def remote_set_state():
    """
    Sets working state
    """
    if not is_auth(request.headers['authorization']):
        return 'Unauthorized', 401

    num = int(request.args['dev'])
    state = request.args['state']

    if boiler.set_state(state):
        return 'OK', 200
    # if state == ON_STATE:
    #     pins[num]['state'] = OnState
    #     add_scheduled_job('datetime', start, end)
    #     nextEnd = str(end)
    #     lastStart = str(datetime.now())
    #     lcd.print_text('Active since:\n' + lastStart)
    # elif state == OFF_STATE:
    #     pins[num]['state'] = OffState
    #     scheduler.remove_job(nextEnd)
    #     lcd.print_text('Last Active:\n' + datetime.now().strftime(DT_FORMAT))
    #     lastStart = ''
    else:
        return 'BAD', 500


@app.route('/api/getstate')
def remote_get_state():
    """
    Replies with the current working state
    :return: json string with the state.
    """
    if not is_auth(request.headers['authorization']):
        return 'Unauthorized', 401

    num = int(request.args['dev'])
    if num is None:
        return 'BAD', 500

    if boiler.get_state():
        return json.dumps({'state': 'On', 'on_since': boiler.lastStart})
    return json.dumps({'state': 'Off', 'on_since': None})


def run_server():
    # boiler.print_to_scree('Server Running!')
    get_jobs_from_db()
    app.run(host='0.0.0.0')
    boiler.set_state(OFF_STATE)
    db.close()


if __name__ == '__main__':
    handler = RotatingFileHandler(LOG_PATH, maxBytes=10000,
                                  backupCount=1)
    log = logging.getLogger('werkzeug')
    handler.setLevel(logging.DEBUG)
    log.addHandler(handler)
    run_server()
