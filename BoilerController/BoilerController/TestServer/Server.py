from flask import Flask, request
import json
import sqlite3
from apscheduler.schedulers.background import BackgroundScheduler

OnState = True # Replace with GPIO.HIGH
OffState = False # Replace With GPIO.LOW

#OnState = GPIO.HIGH
#OffState = GPIO.LOW

sched = 0

app = Flask(__name__)
pins = {17: {'name': 'LED', 'state': OffState}}

db = sqlite3.connect('boiler.db')


@app.route('/remove')
def delete_item():
    id = request.args['id']

    curs = db.cursor()
    try:
        curs.execute(
            "DELETE from schedule where ID=?;", (id,))
        db.commit()
    except Exception as e:
        print(e)
        return 'BAD'
    return 'OK'

@app.route('/settime')
def set_time():
    pin = request.args['dev']
    start = request.args['ondate'] + " " + request.args['ontime']
    end = request.args['offdate'] + " " + request.args['offtime']
    values = (pin, start, end)

    curs = db.cursor()
    try:
        curs.execute("INSERT INTO schedule (dev, turnon, turnoff) "
                     "VALUES (?,?,?);",
                     values)
        db.commit()
    except:
        return 'BAD'
    return 'OK'


@app.route('/gettimes')
def get_times():
    d = list()
    curs = db.cursor()
    try:
        curs.execute("SELECT * FROM schedule")
        query = curs.fetchall()
    except:
        return 'BAD'
    for q in query:
        d.append({'ID': q[0],
                  'pin': q[1],
                  'start': q[2],
                  'end': q[3]})
    return json.dumps(d)


@app.route('/setled<num>/<state>')
def set_led(num, state):
    num = int(num)
    if state == '1':
        pins[num]['state'] = OnState
    elif state == '0':
        pins[num]['state'] = OffState
    # GPIO.output(num, pins[num]['state'])
    return 'OK'


@app.route('/getled<num>')
def get_led(num):
    num = int(num)
    if pins[num]['state'] == OnState:
        return 'On'
    return 'Off'


@app.route('/')
def default_route():
    return 'Please use correct commands.'


if __name__ == '__main__':
    app.run(host='0.0.0.0')
    db.close()
