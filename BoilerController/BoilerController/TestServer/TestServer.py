from flask import Flask, request
import sqlite3

app = Flask(__name__)
pins = { 17: {'name': 'LED', 'state': False}}

db = sqlite3.connect('boiler.db')

@app.route('/settime')
def set_time():
	on_time = request.form['ontime']
	on_date = request.form['ondate']
	off_time = request.form['offtime']
	off_date = request.form['offdate']

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
	app.run(host='0.0.0.0:5000')