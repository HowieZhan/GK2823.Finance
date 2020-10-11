using GK2823.UtilLib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class FinancePPDB
    {
        public const string CONN_CONFIG_KEY = "Finance";

        [ThreadStatic]
        static PetaPocoDBContext _context;
        public static PetaPocoDBContext GetContext()
        {
            if (_context == null)
            {
                var connString = AppSettingHelper.GetConnectDBString(CONN_CONFIG_KEY, "ConnectionStrings");
                var connProvider = AppSettingHelper.GetConnectDBString(CONN_CONFIG_KEY, "ProviderName");
                _context = new PetaPocoDBContext(connString, connProvider);
            }
            return _context;
        }
        public PetaPocoDBContext GetInstance()
        {
            return FinancePPDB.GetContext();
        }
    }
}
