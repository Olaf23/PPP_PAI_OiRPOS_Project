import requests as r
import json 

from types import SimpleNamespace
from Models.ProductionPart import ProductionPart
from datetime import date, datetime

class MesApiConnector():
    def __init__(self, server, port):
        self.server = server
        self.port = port

    def GetOeeData(self):
        serverPath = self.server + ":" + self.port
        session = r.Session()
        getResult = session.get('http://'+serverPath+'/oee/visualization/GetOeeResult', cookies={'AccessToken': '8M3dcr3CVgaRPgJP1xGmyd6d5rTsN9mKQNPzXq769sQC4mWWd01qawvJK/v1qp7S'})
        oeeResultInJson = getResult.text
        oeeResult = json.loads(oeeResultInJson, object_hook=lambda d: SimpleNamespace(**d))
        return oeeResult

    def GetPartCounters(self, oeeResult):
        oeeHourSummary = oeeResult.oeeHourSummariesCollection
        stationSummary = [x for x in oeeHourSummary if x.hour == -1 and x.idStation == 1138]
        return stationSummary[0]

    def GetLastWorkStatus(self, oeeResult):
        oeeWorkTimesCollection = oeeResult.oeeWorkTimesCollection
        stationWorkTimes = [x for x in oeeWorkTimesCollection if x.idStation == 1138]
        return max(stationWorkTimes, key= lambda workTime: workTime.dtStart)

    def PutNewPart(self, partStatus, partIdent):
        productionPart = {
            "idStation": 1138,
            "partIdent": partIdent,
            "dtStart": datetime.now(),
            "dtStop": datetime.now(),
            "partStatus": partStatus,
            "variant": 1
        }
        partInJson = json.dumps(productionPart, indent=4, sort_keys=True, default=str)
        serverPath = self.server + ":" + self.port
        headers = {'Content-Type': "application/json", 'Accept': "application/json"}
        session = r.Session()
        session.put(
            'http://'+serverPath+'/oee/plcS7Driver/PutPartResult',
            cookies={'AccessToken': '8M3dcr3CVgaRPgJP1xGmyd6d5rTsN9mKQNPzXq769sQC4mWWd01qawvJK/v1qp7S'},
            headers = headers,
            data= partInJson)

    def PutNewWorkStatus(self, workStatus):
        workTime = {
            "idStation": 1138,
            "workStatus": workStatus,
            "dtStart": datetime.now(),
            "variant": 1,
            "DowntimeReason": 1
        }
        workTimeInJson = json.dumps(workTime, indent=4, sort_keys=True, default=str)
        serverPath = self.server + ":" + self.port
        headers = {'Content-Type': "application/json", 'Accept': "application/json"}
        session = r.Session()
        session.put(
            'http://'+serverPath+'/oee/plcS7Driver/PutDowntime',
            cookies={'AccessToken': '8M3dcr3CVgaRPgJP1xGmyd6d5rTsN9mKQNPzXq769sQC4mWWd01qawvJK/v1qp7S'},
            headers = headers,
            data= workTimeInJson)