using AmiMesApi.DataBase.ProcedureResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    [NotMapped]
    [Keyless]
    public class OeeRating
    {
        [NotMapped]
        public int IdArea { get; set; }
        [NotMapped]
        public int IdStation { get; set; }
        [NotMapped]
        public int Variant { get; set; }
        [Column("OEE")]
        public float OeeRate { get; set; }
        [Column("Availability")]
        public float AvailabilityRate { get; set; }
        [Column("Performance")]
        public float PerformanceRate { get; set; }
        [Column("Quality")]
        public float QualityRate { get; set; }

        public OeeRating()
        {

        }

        public OeeRating(OeeProcedureResult oeeStatus, int idArea, int idStation)
        {
            IdArea = idArea;
            IdStation = idStation;
            OeeRate = oeeStatus.OeeRate;
            AvailabilityRate = oeeStatus.AvailabilityRate;
            PerformanceRate = oeeStatus.PerformanceRate;
            QualityRate = oeeStatus.QualityRate;
        }
    }
}
