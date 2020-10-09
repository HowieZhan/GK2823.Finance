using Dapper;
using Dapper.Contrib.Extensions;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.BizLib.Shared
{
    public class BaseService
    {
        private readonly DBService _dBService;
        private readonly IOptions<AppSettings> _appsettings;
        public BaseService()
        {            
            _dBService = AutofacContainer.Resolve<DBService>();
            _appsettings = AutofacContainer.Resolve<IOptions<AppSettings>>();
        }

        protected void SetTaskLog(string taskName,object remark=null)
        {
            try
            {
                var dynamicParams = new DynamicParameters();
                var todayTime = DateTime.Now;
                dynamicParams.Add("todayTime", todayTime.Date);
                dynamicParams.Add("tomorrowTime", DateTime.Now.AddDays(1).Date);
                dynamicParams.Add("taskName", taskName);
                var item = _dBService.FinanceDB.QueryFirstOrDefault<TaskLog>("select * from task_log where last_time>=@todayTime and last_time<@tomorrowTime and task_name=@taskName", dynamicParams);
                switch (taskName)
                {
                    case "get_from_xuangubao":

                        if (item == null)
                        {
                            //insert
                            var taskLog = new TaskLog();
                            taskLog.task_name = taskName;
                            taskLog.last_time = todayTime;
                            taskLog.remark = string.Join(",", remark as List<int>) + "|";
                            taskLog.run_count = 1;
                            _dBService.FinanceDB.Insert<TaskLog>(taskLog);
                        }
                        else
                        {
                            var taskLog = item;
                            taskLog.task_name = taskName;
                            taskLog.last_time = todayTime;
                            taskLog.run_count++;
                            var rkAdd = taskLog.remark.Split('|')[0];
                            var rkUpdate = string.Join(",", remark as List<int>);
                            taskLog.remark = rkAdd + "|" + rkUpdate;
                            _dBService.FinanceDB.Update<TaskLog>(taskLog);
                            //update
                        }
                        break;
                    case "error_get_from_xuangubao":
                        if (item == null)
                        {
                            //insert
                            var taskLog = new TaskLog();
                            taskLog.task_name = taskName;
                            taskLog.last_time = todayTime;
                            taskLog.remark = remark.ToString();
                            taskLog.run_count = 1;
                            _dBService.FinanceDB.Insert<TaskLog>(taskLog);
                        }
                        else
                        {
                            var taskLog = item;
                            taskLog.task_name = taskName;
                            taskLog.last_time = todayTime;
                            taskLog.run_count++;
                           // var rkAdd = taskLog.remark.Split('|')[0];
                           // var rkUpdate = string.Join(",", remark as List<int>);
                            taskLog.remark = remark.ToString();
                            _dBService.FinanceDB.Update<TaskLog>(taskLog);
                            //update
                        }
                        break;
                    case "error_GetTodayPoolDetail":
                        if (item == null)
                        {
                            var taskLog = new TaskLog();
                            taskLog.task_name = taskName;
                            taskLog.last_time = todayTime;
                            taskLog.remark = remark.ToString();
                            taskLog.run_count = 1;
                            _dBService.FinanceDB.Insert<TaskLog>(taskLog);
                        }
                        else
                        {
                            var taskLog = item;
                            taskLog.task_name = taskName;
                            taskLog.last_time = todayTime;
                            taskLog.run_count++;
                            taskLog.remark = remark.ToString();
                            _dBService.FinanceDB.Update<TaskLog>(taskLog);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                var a = ex;
                Console.WriteLine("游客权限："+ex.Message);
            }
        }

        public void SendEmail(string toEmail, string title, string content, string userAddress = null, string password = null)
        {
            if (userAddress == null)
            {
                userAddress = _appsettings.Value.Email.UserAddress;
                password = _appsettings.Value.Email.UserPassword;
            }
            else
            {
                var dic = new Dictionary<string, string>();
                dic.Add(_appsettings.Value.MainEmail.UserAddress, _appsettings.Value.MainEmail.UserPassword);
                dic.Add(_appsettings.Value.Email.UserAddress, _appsettings.Value.Email.UserPassword);
                foreach (var item in dic)
                {
                    if (item.Key == userAddress)
                    {
                        userAddress = item.Key;
                        password = item.Value;
                    }
                }
            }
            var mailer = new MailInfo()
            {
                Port = 465,
                UseSsl = true,
                Host = _appsettings.Value.Email.Host,
                UserName = "Form " + _appsettings.Value.Webs[0].Url,
                UserAddress = userAddress,
                Password = password,
                Content = content,
                Subject = title
            };
            mailer.Reset();
            mailer.AddToAddress(toEmail);
            _ = MailHelper.SendAsync(mailer);
        }
    }
}
