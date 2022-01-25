using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    public class OeeDowntimesManager
    {
        public List<DbDowntimeReason> DowntimeReasons { get; set; }
        public List<DbStationDowntimeTies> StationToDowntimeTies { get; set; }
        public List<DbDowntimeSchedule> DowntimeSchedule { get; set; }
    }
}
