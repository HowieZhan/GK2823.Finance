
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

        private FinanceDB FinanceIntDB { get; } =
           DatabaseProvider<FinanceDB>.Instance;


        public PetaPocoDBContext FinancePPDB { get { return Shared.FinancePPDB.GetContext(); } }

       
    }
}
