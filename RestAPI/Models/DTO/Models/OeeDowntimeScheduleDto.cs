using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    public class OeeDowntimeScheduleDto
    {
        public int Id { get; set; }
        public int IdStation { get; set; }
        public int IdDowntimeReason { get; set; }
        public long HourStartTicks { get; set; }
        public long HourStopTicks { get; set; }
    }
}
