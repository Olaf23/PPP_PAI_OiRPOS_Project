using API_Standard.Models.Base.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    public class UserMachineInfo
    {
        public List<Area> AreasCollection { get; set; }
        public List<Station> StationsCollection { get; set; }
        public List<Variant> VariantsCollection { get; set; }
    }

    public class Variant
    {
        public int IdStation { get; set; }
        public int Code { get; set; }
        public int ReferenceCycleTime { get; set; }
    }
}
