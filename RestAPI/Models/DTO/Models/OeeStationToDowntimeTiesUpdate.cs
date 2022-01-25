using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    public class OeeStationToDowntimeTiesUpdate
    {
        public int IdDowntime { get; set; }
        public List<StationToTie> StationsToTie { get; set; }
    }

    public class StationToTie
    {
        public int Id { get; set; }
        public bool IsTiedToDowntime { get; set; }
    }
}
