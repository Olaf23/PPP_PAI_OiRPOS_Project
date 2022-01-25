using AmiMesApi.DataBase.ProcedureResults;
using API_Standard.Models.Base.Structure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmiMesApi.Model.Oee
{
    [NotMapped]
    [Keyless]
    public class OeeHourSummary : OeeModel
    {
        public int Hour { get; set; }
        public int OkCount { get; set; }
        public int NokCount { get; set; }
        public int Target { get; set; }
        public OeeHourSummary()
        {

        }

        public OeeHourSummary(Station station, int hour, OeeProcedureResult oeeStatus)
        {
            IdArea = station.IdArea;
            IdStation = station.Id;
            Hour = hour;
            OkCount = oeeStatus.IOcount;
            NokCount = oeeStatus.NIOcount + oeeStatus.Reworkedcount;
            Target = oeeStatus.Target;
        }
    }
}