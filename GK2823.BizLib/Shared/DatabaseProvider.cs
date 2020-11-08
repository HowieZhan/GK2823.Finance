using GK2823.UtilLib.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class DatabaseProviderV0 : IDbConnection
    {
        public string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int ConnectionTimeout => throw new NotImplementedException();

        public string Database => throw new NotImplementedException();

        public ConnectionState State => throw new NotImplementedException();

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }
    }
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


        public static DbConnection _gconn;

        public DbConnection GetOpenConnection()
        {
            //if (AppSettingHelper.GetValue("appName") == "金融API")
            //{
            var _conn = Factory.CreateConnection();
            _conn.ConnectionString = GetConnectionString();
            if (_conn.State == ConnectionState.Closed)
            {

                _conn.Open();

            }
            return _conn;
            //}
            //else
            //{
            //if (_gconn == null)
            //{
            //    _gconn = Factory.CreateConnection();
            //    _gconn.ConnectionString = GetConnectionString();

            //}
            //if (_gconn.State == ConnectionState.Closed)
            //{
            //    _gconn.Open();
            //}
            //return _gconn;
            //}
        }

        public DbConnection GetClosedConnection()
        {
            var _conn = Factory.CreateConnection();
            //_conn.ConnectionString = GetConnectionString();
            if (_conn.State != ConnectionState.Closed) _conn.Close();
            return _conn;
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
