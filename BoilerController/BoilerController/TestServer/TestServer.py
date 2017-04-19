from flask import Flask, request
import json
import sqlite3

OnState = True # Replace with GPIO.HIGH
OffState = False # Replace With GPIO.LOW

app = Flask(__name__)
pins = {17: {'name': 'LED', 'state': OffState}}

db = sqlite3.connect('boiler.db')


@app.route('/settime')
def set_time():
    pin = request.args['dev']
    start = request.args['ondate'] + " " + request.args['ontime']
    end = request.args['offdate'] + " " + request.args['offtime']
    values = (pin, start, end)

    curs = db.cursor()
    try:
        curs.execute("INSERT INTO schedule VALUES (?,?,?);", values)
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
        d.append({'pin': q[0],
                  'start': q[1],
                  'end': q[2]})
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
