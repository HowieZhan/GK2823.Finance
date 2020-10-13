using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class DBService
    {

        public  DbConnection FinanceDB { get { return FinanceIntDB.GetOpenConnection(); } }

        private FinanceDB FinanceIntDB { get; } =
           DatabaseProvider<FinanceDB>.Instance;


        public PetaPocoDBContext FinancePPDB { get { return Shared.FinancePPDB.GetContext(); } }
    }
}
