using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    public class OeeDowntimeReasonDto : DbDowntimeReason
    {
        public int IdStation { get; set; }
    }
}
