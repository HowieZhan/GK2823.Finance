using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class FinanceDB : DatabaseProvider
    {
        const string CONN_CONFIG_KEY = "Finance";
        public override DbProviderFactory Factory => Npgsql.NpgsqlFactory.Instance;

        public override string GetConnectionString()
        {
            var connStr = AppSettingHelper.GetConnectDBString(CONN_CONFIG_KEY, "ConnectionStrings");
            return connStr;
        }
    }
}
