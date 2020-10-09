using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.ModelLib.Shared
{
    public class AppSettings
    {
        public string appName { get; set; }

        public List<Developers> developers { get; set; }

        public List<RangeTime> rangeTime { get; set; }
        public _MainEmail MainEmail { get; set; }
        public _RabbitMQ RabbitMQ { get; set; }
        public _MQHandle MQHandle { get; set; }
        public _MainEmail Email { get; set; }
        public List<HttpReqs> Webs { get; set; }

        public class HttpReqs
        {
            public string Url { get; set; }
            public string Desc { get; set; }
        }
        public class _MQHandle
        {
            public MQFuncConfig DefaultQueue { get; set; }
            public class MQFuncConfig
            {
                public string QueueName { get; set; }
                public MQFunctions MQFunction { get; set; }

                public class MQFunctions
                {
                    public string SendEmail { get; set; }
                }
            }
        }
        public class _RabbitMQ
        {
            public string MQHost { get; set; }
            public int MQPort { get; set; }
            public string MQUser { get; set; }
            public string MQPasswd { get; set; }
        }
        public class _MainEmail
        {
            public string Host { get; set; }
            public string UserAddress { get; set; }
            public string UserPassword { get; set; }
        }
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
