using GK2823.BizLib.Shared.PetaPoco;
using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class PetaPocoDBContext : Database
    {
        public PetaPocoDBContext(string connectionString, string providerName)
      : base(connectionString, providerName)
        {
        }


        public int ExecuteCount(string tableName, string strWhere, params object[] p)
        {
            var sql = PetaPoco.Sql.Builder.Select("COUNT(0)").From(tableName).Where(strWhere, p);
            return this.ExecuteScalar<int>(sql);
        }
        public int ExecuteDelete(string tableName, string strWhere, params object[] p)
        {
            var sql = new PetaPoco.Sql("DELETE FROM " + tableName + " ").Where(strWhere, p);
            return this.Execute(sql);
        }
        public int ExecuteUpdate(string tableName, Dictionary<string, object> updateSets, string strWhere, params object[] p)
        {
            return this.ExecuteUpdate(tableName, "", updateSets, strWhere, p);
        }

        public int ExecuteUpdate(string tableName, string setString, Dictionary<string, object> updateSets, string strWhere, params object[] p)
        {
            var sql = new PetaPoco.Sql("UPDATE " + tableName + " SET ");
            var values = new List<object>();
            var list = new List<string>();
            if (!string.IsNullOrEmpty(setString)) list.Add(setString);
            var n = 0;
            foreach (var kv in updateSets)
            {
                list.Add(string.Format("{0}=@{1} ", kv.Key, n));
                n++;
                values.Add(kv.Value);
            }
            sql.Append(string.Join(",", list.ToArray()), values.ToArray());
            sql.Where(strWhere, p);
            return this.Execute(sql);
        }
    }
}
