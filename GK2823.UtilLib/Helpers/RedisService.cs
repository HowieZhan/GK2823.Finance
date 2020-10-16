using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GK2823.UtilLib.Helpers
{
    /// <summary>
    /// 缓存操作接口类
    /// </summary>
    public interface IRedisService
    {
        #region 设置缓存

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="value">值</param>
        /// <typeparam name="T">数据类型</typeparam>
        void SetCache(string key, object value);

        /// <summary>
        /// 设置缓存
        /// 注：默认过期类型为绝对过期
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间间隔</param>
        /// <typeparam name="T">数据类型</typeparam>
        void SetCache(string key, object value, TimeSpan timeout);

        /// <summary>
        /// 设置缓存
        /// 注：默认过期类型为绝对过期
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间间隔</param>
        /// <param name="expireType">过期类型</param>
        /// <typeparam name="T">数据类型</typeparam>
        void SetCache(string key, object value, TimeSpan timeout, ExpireType expireType);

        /// <summary>
        /// 设置键失效时间
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="expire">从现在起时间间隔</param>
        void SetKeyExpire(string key, TimeSpan expire);

        #endregion

        #region 获取缓存

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">主键</param>
        object GetCache(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">主键</param>
        /// <typeparam name="T">数据类型</typeparam>
        T GetCache<T>(string key) where T : class;

        /// <summary>
        /// 是否存在键值
        /// </summary>
        /// <param name="key">主键</param>
        /// <returns></returns>
        bool ContainsKey(string key);

        #endregion

        #region 删除缓存

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key">主键</param>
        void RemoveCache(string key);

        void RemoveAllCache();

        #endregion
    }

    #region 类型定义

    /// <summary>
    /// 值信息
    /// </summary>
    public struct ValueInfoEntry
    {
        public string Value { get; set; }
        public string TypeName { get; set; }
        public TimeSpan? ExpireTime { get; set; }
        public ExpireType? ExpireType { get; set; }
    }

    /// <summary>
    /// 过期类型
    /// </summary>
    public enum ExpireType
    {
        /// <summary>
        /// 绝对过期
        /// 注：即自创建一段时间后就过期
        /// </summary>
        Absolute,

        /// <summary>
        /// 相对过期
        /// 注：即该键未被访问后一段时间后过期，若此键一直被访问则过期时间自动延长
        /// </summary>
        Relative,
    }

    #endregion
    public class RedisService: IRedisService
    {
        /// <summary>
        /// Redis缓存
        /// </summary>

        /// <summary>
        /// 默认构造函数
        /// 注：使用默认配置，即localhost:6379,无密码
        /// </summary>
        public RedisService()
        {
            _databaseIndex = 0;
            string config =AppSettingHelper.GetRedisString();
            _redisConnection = ConnectionMultiplexer.Connect(config);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">配置字符串</param>
        /// <param name="databaseIndex">数据库索引</param>
        public RedisService(string config, int databaseIndex = 0)
        {
            _databaseIndex = databaseIndex;
            _redisConnection = ConnectionMultiplexer.Connect(config);
        }

        private ConnectionMultiplexer _redisConnection { get; }
        private IDatabase _db { get => _redisConnection.GetDatabase(_databaseIndex); }
        private int _databaseIndex { get; }
        public bool ContainsKey(string key)
        {
            return _db.KeyExists(key);
        }

        public object GetCache(string key)
        {
            object value = null;
            var redisValue = _db.StringGet(key);
            if (!redisValue.HasValue)
                return null;

            ValueInfoEntry valueEntry = redisValue.ToString().ToObject<ValueInfoEntry>();

            if (valueEntry.TypeName == typeof(string).FullName)
                value = valueEntry.Value;
            else
                //value = JsonConvert.DeserializeObject<object>(valueEntry.Value);
                value = valueEntry.Value.ToObject(Type.GetType(valueEntry.TypeName));

            if (valueEntry.ExpireTime != null && valueEntry.ExpireType == ExpireType.Relative)
                SetKeyExpire(key, valueEntry.ExpireTime.Value);

            return value;
        }

        public T GetCache<T>(string key) where T : class
        {
            //return  System.Text.Json.JsonSerializer.Deserialize<T>(GetCache(key).ToString());
            return (T)GetCache(key);
        }

        public void SetKeyExpire(string key, TimeSpan expire)
        {
            _db.KeyExpire(key, expire);
        }

        public void RemoveCache(string key)
        {
            _db.KeyDelete(key);
        }

        public void SetCache(string key, object value)
        {
            _SetCache(key, value, null, null);
        }

        public void SetCache(string key, object value, TimeSpan timeout)
        {
            _SetCache(key, value, timeout, ExpireType.Absolute);
        }

        public void SetCache(string key, object value, TimeSpan timeout, ExpireType expireType)
        {
            _SetCache(key, value, timeout, expireType);
        }

        private void _SetCache(string key, object value, TimeSpan? timeout, ExpireType? expireType)
        {
            string jsonStr = string.Empty;
            if (value is string)
                jsonStr = value as string;
            else
                jsonStr = JsonConvert.SerializeObject(value);

            ValueInfoEntry entry = new ValueInfoEntry
            {
                Value = jsonStr,
                TypeName = value.GetType().FullName,
                ExpireTime = timeout,
                ExpireType = expireType
            };

            string theValue = JsonConvert.SerializeObject(entry);
            if (timeout == null)
                _db.StringSet(key, theValue);
            else
                _db.StringSet(key, theValue, timeout);
        }

        public void RemoveAllCache()
        {
            Type items = typeof(Constants.Redis);
            MemberInfo[] memberInfo = items.GetMembers();
            foreach (MemberInfo mInfo in memberInfo)
            {
                if (mInfo.Name.StartsWith("FYJSYSTEM"))
                {
                    _db.KeyDelete(mInfo.Name);
                }
            }
        }
    }

    public class Constants
    {
        public const string WEBSITE_51JOB = "QC";
        public const string WEBSITE_ZHAOPIN = "ZL";
        public const string WEBSITE_ZHIPIN = "ZP";
        public const string WEBSITE_LAGOU = "LG";
        public class Redis
        {
            //首页诗文
            public const string FYJSYSTEM_POETRIES_INDEX = "FYJSYSTEM_POETRIES_INDEX";
            //首页技术文章
            public const string FYJSYSTEM_TECHNOLOGY_INDEX = "FYJSYSTEM_TECHNOLOGY_INDEX";
            // public const string FYJSYSTEM_RECORD_INDEX = "FYJSYSTEM_RECORD_INDEX";
            //首页热文
            public const string FYJSYSTEM_HOTARTICLES_INDEX = "FYJSYSTEM_HOTARTICLES_INDEX";
            // public const string FYJSYSTEM_CLIENT_IP = "FYJSYSTEM_CLIENT_IP";

            //首页技术-波波
            public const string FYJSYSTEM_TECHNOLOGY_INDEX_BOBO = "FYJSYSTEM_TECHNOLOGY_INDEX_BOBO";
            //首页热文-波波
            public const string FYJSYSTEM_HOTARTICLES_INDEX_BOBO = "FYJSYSTEM_HOTARTICLES_INDEX_BOBO";

            public const string FAPI_GetPoolDetail = "FAPI_GetPoolDetail";

            public const string FAPI_GetEverydayLBS = "FAPI_GetEverydayLBS";

        }
    }
}
