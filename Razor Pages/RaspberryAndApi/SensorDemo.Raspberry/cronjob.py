import time
import threading

####################################################################################################
# Klasse zur Ausführung von Cronjobs.
####################################################################################################
class Cronjob:
    __work_items = []
    def __init__(self, logger):
        self.__logger = logger

    # Fügt eine Callback Funktion hinzu, die in bestimmten Abständen ausgeführt werden soll.
    def append_work(self, id, action, interval):
        self.__work_items.append({'id': id, 'action': action, 'interval': interval, 'next_run': int(time.time() / interval + 1) * interval})
        self.__logger.debug(self.__work_items)

    # Führt jede Callback Methode zur definierten Zeit lt. Intervall in einem eigenen Thread aus.
    def start(self):
        while(True):
            current_time = time.time()
            for i, item in enumerate(self.__work_items):
                if current_time >= item['next_run']:
                    threading.Thread(target = item['action']).start()
                    item['next_run'] = int(time.time() / item['interval'] + 1) * item['interval']
                    self.__logger.debug(f"{item['id']} started at {current_time}.")
            time.sleep(1)
