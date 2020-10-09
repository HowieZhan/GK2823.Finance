using System;
using System.Collections.Generic;
using System.Text;

namespace Finance.TaskRunner
{
    public class AppSettings
    {
        public string appName { get; set; }

        public List<Developers> developers { get; set; }

        public List<RangeTime> rangeTime { get; set; }

        public class Developers
        {
            public string phone { get; set; }
            public string email { get; set; }
        }

        public class RangeTime
        {
            public string taskName { get; set; }
            public double second { get; set; }
            public int opened { get; set; }
        }
    }
}
