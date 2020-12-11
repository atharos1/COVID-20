using COVID_20.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COVID_20.ViewModels {
    public class CasesStatsViewModel {
        public Province Province { get; set; }
        public DateTime? Timestamp { get; set; }
        public int ConfirmedDeaths { get; set; }
        public int ConfirmedCases { get; set; }
        public int AccumulatedDeaths { get; set; }
        public int AccumulatedCases { get; set; }
        public double Mortality { get; set; }
    }
}
