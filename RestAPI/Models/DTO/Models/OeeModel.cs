using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model
{
    public class OeeModel
    {
        public int IdArea { get; set; }
        public int IdStation { get; set; }
        public int Variant { get; set; }
    }
}
