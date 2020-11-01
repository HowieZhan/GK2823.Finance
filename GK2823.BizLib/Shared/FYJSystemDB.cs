using GK2823.UtilLib.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace GK2823.BizLib.Shared
{

    public class FYJSystemDB : DatabaseProvider
    {
        const string CONN_CONFIG_KEY = "FYJSystem";
        public override DbProviderFactory Factory => SqlClientFactory.Instance;

        public override string GetConnectionString()
        {
            var connStr = AppSettingHelper.GetConnectDBString(CONN_CONFIG_KEY, "ConnectionStrings");
            return connStr;
        }
    }
}
