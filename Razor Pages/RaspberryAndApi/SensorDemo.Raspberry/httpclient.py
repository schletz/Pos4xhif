import time
import json
import ssl
import http.client
import hmac
import hashlib
import base64

####################################################################################################
# Klasse zum Senden der Daten als POST Request an die WebAPI
####################################################################################################
class HttpClient:
    __server = "localhost:5001"
    __url = "/api/raspberry"

    def __init__(self, secret, logger):
        self.__logger = logger
        self.__secret = base64.b64decode(secret)
        self.__logger.debug("HttpClient init")

    def send_data(self, data):
        self.__logger.debug(f'Try to send payload {data}')
        self.__send_request(data, 'sensordata')

    def send_heartbeat(self):
        self.__logger.debug(f'Try to send heartbeat.')
        self.__send_request(str(time.time()), 'heartbeat')

    # Generiert einen temporären API Key, der aus dem Zeitstempel sowie dem Hashwert mit dem
    # Secret besteht.
    def __generate_token(self):
        # Deutsches , durch das normale Komma (.) ersetzen.
        timestamp = str(time.time()).replace(",", ".")
        hash_val = base64.b64encode(hmac.new(self.__secret, msg = bytes(timestamp, 'UTF-8'), digestmod = hashlib.sha256).digest()).decode()
        return f"{timestamp}:{hash_val}"

    # Sendet einen POST Request mit den übergebenen Daten an den Server. Dabei wird ein Key
    # auf Basis des Secrets generiert, den der Server prüft.
    def __send_request(self, data, action):
        try:
            token = self.__generate_token()
            body = json.dumps(data)
            # Akzeptiert auch selbst generierte Zertifikate. Sollte in Produktion mit dem 
            # Kontext Parameter von HTTPSConnection entfernt werden
            context = ssl.SSLContext()
            conn = http.client.HTTPSConnection(self.__server, context=context)
            url = self.__url + "/" + action
            try:
                conn.request(
                    method = "POST", 
                    url = url, 
                    body = body, 
                    headers = {'X-API-Key': token, 'Content-type': 'application/json'})
                response = conn.getresponse()
                if response.status == 403:
                    raise Exception(f"Server responded not authorized.")
                if response.status == 404:
                    raise Exception(f"Server responded not found.")            
                if response.status > 299:
                    raise Exception(f"Server responded with status {response.status}.") 
                self.__logger.debug("Request has successfully sent.")
            finally:
                conn.close()
        except Exception as e:
            self.__logger.error(f"Error from {url}: {e}")
            raise
