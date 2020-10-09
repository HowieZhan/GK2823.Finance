using System;
using System.Collections.Generic;
using System.Text;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace GK2823.BizLib.Shared
{
    public class RabbitMQHelper
    {
        static ConnectionFactory factory;
        static void BuildFactory()
        {
            var _appConfigs = AutofacContainer.Resolve<IOptions<AppSettings>>();
            if (factory == null)
            {
                factory = new ConnectionFactory()
                {
                    HostName = _appConfigs.Value.RabbitMQ.MQHost,
                    UserName = _appConfigs.Value.RabbitMQ.MQUser,
                    Password = _appConfigs.Value.RabbitMQ.MQPasswd,
                    Port = _appConfigs.Value.RabbitMQ.MQPort
                };
            }
        }
        /// <summary>
        /// 发送消息队列
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="message">消息内容(一般用JSON)</param>
        /// <param name="routingKey">路由(默认=queueName)</param>
        /// <param name="durable">是否需要持久化</param>
        /// <returns></returns>
        public static bool Send(string queueName, string message, string routingKey = null, bool durable = false)
        {

            routingKey = string.IsNullOrEmpty(routingKey) ? queueName : routingKey;

            try
            {
                BuildFactory();
                // Log.GetLogger().Info("host:{0},port:{1},user:{2},pass:{3}", factory.HostName, factory.Port, factory.UserName, factory.Password);
                using (var connection = factory.CreateConnection())
                {
                    //3. 创建信道
                    using (var channel = connection.CreateModel())
                    {
                        //4. 申明队列(指定durable:true,告知rabbitmq对消息进行持久化)
                        channel.QueueDeclare(queue: queueName, durable: durable, exclusive: false, autoDelete: false);
                        string exchange = "";
                        // 如果queueName 与 routingKey不等，使用Exchange模式
                        if (routingKey != queueName)
                        {
                            exchange = queueName + "Exchange";
                            channel.ExchangeDeclare(exchange, "direct", durable);
                            channel.QueueBind(queueName, exchange, routingKey);
                        }

                        //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                        var properties = channel.CreateBasicProperties();
                        if (durable)
                        {
                            properties.Persistent = true;
                        }
                        //5. 构建byte消息数据包
                        var body = Encoding.UTF8.GetBytes(message);

                        //6. 发送数据包(指定basicProperties)
                        channel.BasicPublish(exchange, routingKey, properties, body);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var logCon = $"MQException:{ex.Message}-[name:{queueName}]|[router:{routingKey}]|[message:{message}]\r\n";
                logCon += ex.StackTrace;
                //ConfigLoader.LogError(logCon, ex);
                //Log.GetLogger().Error(logCon, ex);
            }
            return false;
        }

    }
}
