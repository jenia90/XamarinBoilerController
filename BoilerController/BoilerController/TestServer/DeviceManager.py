import sqlite3
import RPi.GPIO as GPIO
from datetime import datetime, timedelta
from apscheduler.schedulers.background import BackgroundScheduler

from LcdHelper import LcdHelper

ID_IDX = 0
PIN_IDX = 1
START_IDX = 3
END_IDX = 2
TYPE_IDX = 4
DAYS_IDX = 5

ON_STATE = '1'
OFF_STATE = '0'

OnState = GPIO.HIGH
OffState = GPIO.LOW

DT_FORMAT = "%Y-%m-%d %H:%M"


class DeviceManager:
    def __init__(self, name, devPin, heaterPin, relayPin):
        self.name = name
        self.devPin = devPin
        self.pins = {self.devPin: {'name': 'Boiler', 'state': OffState}}
        self.heaterPin = heaterPin
        self.relayPin = relayPin
        self.lastStart = ''
        self.nextEnd = ''
        self.scheduler = BackgroundScheduler()
        self.lcd = LcdHelper()
        self.setup_gpio()

    def __exit__(self, exc_type, exc_val, exc_tb):
        GPIO.cleanup()

    def setup_gpio(self):
        GPIO.setmode(GPIO.BCM)
        GPIO.setwarnings(False)
        GPIO.setup(self.devPin, GPIO.OUT)
        GPIO.output(self.devPin, GPIO.LOW)
        GPIO.setup(self.heaterPin, GPIO.IN, pull_up_down=GPIO.PUD_UP)
        GPIO.add_event_detect(self.heaterPin, GPIO.BOTH,
                              callback=self.manual_override, bouncetime=300)

    def manual_override(self, channel):
        state = GPIO.input(channel)
        if state:
            self.set_state(OFF_STATE)
        else:
            self.set_state(ON_STATE)

    def get_state(self):
        return self.pins[self.devPin]['state']

    def get_last_start(self):
        return self.lastStart

    def get_next_end(self):
        return self.nextEnd

    def get_name(self):
        return self.name

    def set_state(self, state, start=datetime.now(),
                  end=datetime.now() + timedelta(hours=2)):
        """
        Sets desired state
        :param state: desired state string
        :param start: if scheduled job then initial start time; blank otherwise
        :param end: end time of scheduled job
        :return:
        """
        if state == ON_STATE:
            self.pins[self.devPin]['state'] = OnState
            self.add_scheduled_job('datetime', start, end)
            self.nextEnd = str(end.strftime(DT_FORMAT))
            self.lastStart = str(start.strftime(DT_FORMAT))
            self.lcd.print_text('Active Since:\n' + self.lastStart)
        elif state == OFF_STATE or state is None:
            self.pins[self.devPin]['state'] = OffState
            self.lcd.print_text(
                    'Last Active:\n' + datetime.now().strftime(DT_FORMAT))
            self.remove_job(self.lastStart, self.nextEnd)
            self.lastStart = ''
            self.nextEnd = ''

        GPIO.output(self.devPin, self.pins[self.devPin]['state'])
        return True

    def add_scheduled_job(self, type, start, end, days=''):
        if type == 'datetime':
            if datetime.now() < start:  # add start of future job
                self.scheduler.add_job(self.set_state, 'date', id=str(start),
                                       run_date=start,
                                       misfire_grace_time=60,
                                       args=[ON_STATE, start, end])
                # add future end
                self.scheduler.add_job(self.set_state, 'date', id=str(end),
                                       run_date=end,
                                       args=[OFF_STATE, start, end],
                                       misfire_grace_time=60)
        elif type == 'cron':
            self.scheduler.add_job(self.set_state, 'cron', id=str(start),
                                   hour=start.hour, minute=start.minute,
                                   day_of_week=days,
                                   misfire_grace_time=60,
                                   args=[ON_STATE, start, end])
            self.scheduler.add_job(self.set_state, 'cron', id=str(end),
                                   hour=end.hour, minute=end.minute,
                                   day_of_week=days,
                                   args=[OFF_STATE, start, end],
                                   misfire_grace_time=60)

    def remove_job(self, start, end):
        self.scheduler.remove_job(job_id=start + ':00')
        self.scheduler.remove_job(job_id=end + ':00')

    def print_to_scree(self, text):
        self.lcd.print_text(text)
