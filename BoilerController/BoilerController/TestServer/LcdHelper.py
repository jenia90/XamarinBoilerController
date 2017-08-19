import Adafruit_CharLCD as LCD


class LcdHelper():
    def __init__(self):
        lcd_rs = 8
        lcd_en = 11
        lcd_d4 = 1
        lcd_d5 = 0
        lcd_d6 = 5
        lcd_d7 = 6
        lcd_columns = 16
        lcd_rows = 2
        self.lcd = LCD.Adafruit_CharLCD(lcd_rs, lcd_en, lcd_d4, lcd_d5, lcd_d6,
                                   lcd_d7,
                                   lcd_columns, lcd_rows)

        self.lcd.clear()

    def print_text(self, msg):
        self.lcd.clear()
        self.lcd.message(msg)

    def set_cursor(self, col, row):
        self.lcd.set_cursor(col, row)
