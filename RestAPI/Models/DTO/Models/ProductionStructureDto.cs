using AmiMesApi.Model;
using API_Standard.Models.Base.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_AmiOEE.Models.DTO.Models
{
    public class ProductionStructureDto
    {
        public List<Station> Stations { get; set; }
        public List<Area> Areas { get; set; }
        public List<ProductionShift> ProductionShifts { get; set; }
    }
}
