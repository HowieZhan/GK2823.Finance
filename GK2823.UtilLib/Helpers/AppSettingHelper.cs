using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GK2823.UtilLib.Helpers
{
    public class AppSettingHelper
    {
        const string CONNECT_DB_KEY = "FYJ_ConnectionStrings";
        const string REDIS_KEY = "redis";
        static AppSettingHelper()
        {
            var config = AutofacContainer.GetService<IConfiguration>();
            if (config == null)
            {
              

                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
#if DEBUG
                    .AddJsonFile("appsettings.debug.json");
                if (System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "appsettings.debug.json")))
                {
                    builder.AddJsonFile("appsettings.debug.json");
                }
#elif RELEASE
                    .AddJsonFile("appsettings.json");
                    if (System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "appsettings.json")))
                {
                    builder.AddJsonFile("appsettings.json");
                }
#endif

                config = builder.Build();
            }

            _config = config;
        }

        private static IConfiguration _config { get; }

        /// <summary>
        /// 从AppSettings获取key的值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return _config[key];
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="nameOfCon">连接字符串名</param>
        /// <returns></returns>
        public static string GetConnectionString(string nameOfCon)
        {
            return _config.GetConnectionString(nameOfCon);
        }

        /// <summary>
        /// 获取Section
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationSection GetAppSection(string key)
        {
            return _config.GetSection(key);
        }

        /// <summary>
        /// 获取Section下的所有节点的配置集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAppDictionary(string key, IConfigurationSection section = null)
        {
            if (section == null)
            {
                section = _config.GetSection(key);
            }
            else
            {
                section = section.GetSection(key);
            }
            var result = new Dictionary<string, string>();
            foreach (var item in section.GetChildren())
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

        public static string GetConnectDBString(string dbKey, string key)
        {
            string result = "";
            var section = GetAppSection(CONNECT_DB_KEY).GetSection(dbKey);
            result = GetAppValue<string>(key, section);
            return result;
        }

        public static string GetRedisString()
        {
            string result = "";
            result = _config.GetValue<string>(REDIS_KEY);       
            return result;
        }

        public const string RABBITMQ_KEY = "RabbitMQ";
        public static string GetRabbitMQString(string key)
        {
            string result = "";
            var section = GetAppSection(RABBITMQ_KEY);
            result = GetAppValue<string>(key, section);
            return result;
        }

        public static T GetAppValue<T>(string key, IConfigurationSection section = null)
        {
            if (section == null)
            {
                return _config.GetValue<T>(key);
            }
            else
            {
                return section.GetValue<T>(key);
            }
        }

        public static IConfigurationSection LoadConfig(string path, string key)
        {
            var result = new ConfigurationBuilder().AddJsonFile(path).Build();
            return result.GetSection(key);
        }
    }
}
