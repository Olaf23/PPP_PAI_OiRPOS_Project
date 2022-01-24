import json

class ProductionPart():
    def __init__(self, idStation, partIdent, dtStart, dtStop, partStatus, variant):
        self.idStation = idStation
        self.partIdent = partIdent
        self.dtStart = dtStart
        self.dtStop = dtStop
        self.partStatus = partStatus
        self.variant = variant

    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dir__, 
            sort_keys=True, indent=4)