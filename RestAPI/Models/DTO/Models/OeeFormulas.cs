using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    public class OeeFormulas
    {
        public int PartsOkCount { get; set; }
        public int PartsNokCount { get; set; }
        public int PartsRewCount { get; set; }
        public double ReferenceCycleTime { get; set; }
        public double WorkTime { get; set; }
        public double AccidentDowntimeTime { get; set; }
        public double PlannedDowntimeTime { get; set; }
        public double AvailableWorkTime { get => WorkTime - PlannedDowntimeTime; }

        public int GetAvailability()
        {
            double result;
            if (AvailableWorkTime > 0)
                result = ((AvailableWorkTime - AccidentDowntimeTime) / AvailableWorkTime) * 100;
            else
                result = 0;
            if (result < 0 || result.ToString() == "NaN")
                result = 0;
            return (int)Math.Round(result, 0);
        }

        public int GetPerformance()
        {
            double result;
            if (ReferenceCycleTime > 0 && AvailableWorkTime != 0)
                result = ((PartsOkCount + PartsNokCount) / (AvailableWorkTime / ReferenceCycleTime)) * 100;
            else
                result = 0;
            if (result < 0 || result.ToString() == "NaN")
                result = 0;
            return (int)Math.Round(result, 0);
        }

        public int GetQuality()
        {
            double result;
            if ((PartsOkCount + PartsNokCount) > 0)
                result = ((PartsOkCount - PartsRewCount) / (PartsOkCount + PartsNokCount)) * 100;
            else
                result = 0;
            if (result < 0 || result.ToString() == "NaN")
                result = 0;
            return (int)Math.Round(result, 0);
        }

        public int GetOEE()
        {
            double result = (double)(GetAvailability() * GetPerformance() * GetQuality()) / 10000;
            return (int)Math.Round(result, 0);
        }
    }
}
