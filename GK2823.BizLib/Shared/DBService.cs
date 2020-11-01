
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class DBService
    {
        //private ISqlDapper _FinanceIntDB { get; } =
        //   DBServerProvider.SqlDapper;
       // public DbConnection _FinanceDB { get { return _FinanceIntDB.; } }
        public   DbConnection FinanceDB { get { return FinanceIntDB.GetOpenConnection(); } }
        public DbConnection FYJSystemDB { get { return FYJSystemIntDB.GetOpenConnection(); } }

        public DbConnection AdminDB { get { return AdminIntDB.GetOpenConnection(); } }

        private FinanceDB FinanceIntDB { get; } =
           DatabaseProvider<FinanceDB>.Instance;
        private FYJSystemDB FYJSystemIntDB { get; } =
           DatabaseProvider<FYJSystemDB>.Instance;
        private AdminDB AdminIntDB { get; } =
          DatabaseProvider<AdminDB>.Instance;


        public PetaPocoDBContext FinancePPDB { get { return Shared.FinancePPDB.GetContext(); } }

       
    }
}
