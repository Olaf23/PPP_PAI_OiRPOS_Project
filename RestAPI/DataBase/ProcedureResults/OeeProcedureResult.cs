using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.DataBase.ProcedureResults
{
    [Keyless]
    public class OeeProcedureResult
    {
        public int GoodWorkTimeInSec { get; set; }
        public int BadWorkTimeInSec { get; set; }
        [Column("OEE")]
        public float OeeRate { get; set; }
        [Column("Availability")]
        public float AvailabilityRate { get; set; }
        [Column("Performance")]
        public float PerformanceRate { get; set; }
        [Column("Quality")]
        public float QualityRate { get; set; }
        public int Target { get; set; }
        public int IOcount { get; set; }
        public int NIOcount { get; set; }
        public int Reworkedcount { get; set; }
    }

    [Keyless]
    public class OeeDowntimeProc
    {
        public int WorkStatus { get; set; }
        public int WorkTimeCode { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public string DowntimeReason { get; set; }
    }

    [Keyless]
    public class VariantsCountsProc
    {
        public int IOCount { get; set; }
        public int NIOcount { get; set; }
        public int Reworkedcount { get; set; }
    }
}
