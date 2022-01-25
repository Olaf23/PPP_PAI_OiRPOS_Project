using AmiMesApi.DataBase.ProcedureResults;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    [NotMapped]
    public class OeeWorkTime : OeeModel
    {
        public int Id { get; set; }
        public int SubId { get; set; }
        public int WorkStatus { get; set; }
        public int WorkTimeCode { get; set; }
        public string DowntimeReason { get; set; }
        public DateTime DtStart { get; set; }
        public int? Duration { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public OeeWorkTime()
        {

        }

        public OeeWorkTime(OeeDowntimeProc downtime, int idArea, int idStation)
        {
            IdArea = idArea;
            IdStation = idStation;
            WorkStatus = downtime.WorkStatus;
            WorkTimeCode = downtime.WorkTimeCode;
            DowntimeReason = downtime.DowntimeReason;
            DtStart = downtime.StartDate;
            Duration = downtime.Duration;
            Comment = string.Empty;
            Name = string.Empty;
        }
    }
}
