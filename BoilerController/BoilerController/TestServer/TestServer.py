from flask import Flask, request
import sqlite3

app = Flask(__name__)
pins = {17: {'name': 'LED', 'state': False}}


@app.route('/settime')
def set_time():
    db = sqlite3.connect('boiler.db')
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
    db.close()
    return 'OK'


@app.route('/gettimes')
def get_times():
    pass


@app.route('/setled<num>/<state>')
def set_led(num, state):
    num = int(num)
    if state == '1':
        pins[num]['state'] = True
    elif state == '0':
        pins[num]['state'] = False
    return 'OK'


@app.route('/getled<num>')
def get_led(num):
    num = int(num)
    if pins[num]['state'] == True:
        return 'On'
    return 'Off'


@app.route('/')
def default_route():
    return 'Please use correct commands.'


if __name__ == '__main__':
    app.run(host='0.0.0.0')
