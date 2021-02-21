import json
import time
import logging
import httpclient
import cronjob
from logging.handlers import RotatingFileHandler
from httpclient import HttpClient
from cronjob import Cronjob

print('+------------------------------------------------------------------------------+')
print('| RASPBERRY PI SENSOR SKRIPT                                                   |')
print('+------------------------------------------------------------------------------+')

#logging.basicConfig(format = '%(asctime)s %(message)s', filename = 'log.txt', level = logging.ERROR)
logging.basicConfig(format = '%(asctime)s %(message)s', level = logging.DEBUG)
logger = logging.getLogger()

# Callback Methode zum Senden des Heartbeats.
def send_heartbeat():
    logger.debug("Send Heartbeat.")
    try:
        client.send_heartbeat()
    except Exception as e:
        logger.error("Failed to send heartbeat.")

# Callback Methode zum Einlesen und Senden der Messwerte. Exceptions sollen abgefangen werden,
# da sonst der ganze Prozess beendet wird.
def read_temp_sensor():
    logger.debug("Send TEMP.")
    values = {'temperature':23.1, 'timestamp':time.time()}
    try:
        client.send_data(values)  
    except Exception as e:
        logger.error("Failed to send temperature.")


# Konfigurationseinstellungen aus config.json laden. Exceptions sollen abgefangen werden,
# da sonst der ganze Prozess beendet wird.
config = {}
with open('config.json') as json_file:
    config = json.load(json_file)

client = HttpClient(config['Secret'], logger)
cron = Cronjob(logger)

# Die einzelnen Methoden in der Cronjob Klasse registrieren.
cron.append_work(id = "HEART", action = send_heartbeat, interval = 10)
cron.append_work(id = "TEMP", action = read_temp_sensor, interval = 2)

try:
    cron.start()
except KeyboardInterrupt:
    print("Cancelled")
except Exception:
    logging.exception('Programmabbruch')


