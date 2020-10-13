using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public static class DatabaseProvider<TProvider> where TProvider : DatabaseProvider
    {
        public static TProvider Instance { get; } = Activator.CreateInstance<TProvider>();
    }
    public abstract class DatabaseProvider
    {
        public abstract DbProviderFactory Factory { get; }

        public static bool IsAppVeyor { get; } = Environment.GetEnvironmentVariable("Appveyor")?.ToUpperInvariant() == "TRUE";
        public virtual void Dispose() { }
        public abstract string GetConnectionString();

      //  public static DbConnection _conn;
        public DbConnection GetOpenConnection()
        {
            //var _conn = Factory.CreateConnection();
            //if (_conn==null)
            //{
              var  _conn = Factory.CreateConnection();
                _conn.ConnectionString = GetConnectionString();
            //    bool wasColsed = _conn.State == ConnectionState.Closed;
            //    if (wasColsed)
            //    {
            //        _conn.Open();
            //    }
            //    else
            //    {
            //        _conn.Close();
            //    }
            //}

            //if (_conn.State != ConnectionState.Open)
            //{
            //    _conn.Open();
            //}
            return _conn;
        }

        public DbConnection GetClosedConnection()
        {
            var conn = Factory.CreateConnection();
            conn.ConnectionString = GetConnectionString();
            if (conn.State != ConnectionState.Closed) conn.Close();
            return conn;
        }

        public DbParameter CreateRawParameter(string name, object value)
        {
            var p = Factory.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            return p;
        }
    }
}
