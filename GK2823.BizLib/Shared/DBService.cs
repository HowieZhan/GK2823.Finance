
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class DBService
    {
        public static DbConnection _FinanceDB;
        public   DbConnection FinanceDB { get {
                if (_FinanceDB == null)
                {
                    _FinanceDB= FinanceIntDB.GetOpenConnection();
                }
                return _FinanceDB;
            } }
        public static DbConnection _FYJSystemDB;
        public DbConnection FYJSystemDB {
            get {
                if (_FYJSystemDB == null)
                {
                    _FYJSystemDB= FYJSystemIntDB.GetOpenConnection();
                }
                return _FYJSystemDB;
            } }

        public static DbConnection _AdminDB;
        public DbConnection AdminDB { get {
                if (_AdminDB == null)
                {
                    _AdminDB= AdminIntDB.GetOpenConnection();
                }
                return _AdminDB;
            } }


        private FinanceDB FinanceIntDB { get; } =
           DatabaseProvider<FinanceDB>.Instance;
        private FYJSystemDB FYJSystemIntDB { get; } =
           DatabaseProvider<FYJSystemDB>.Instance;
        private AdminDB AdminIntDB { get; } =
          DatabaseProvider<AdminDB>.Instance;


        public PetaPocoDBContext FinancePPDB { get { return Shared.FinancePPDB.GetContext(); } }

       
    }
}
