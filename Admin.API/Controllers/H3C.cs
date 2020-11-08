using Admin.API.Models;
using GK2823.BizLib.Admin.Services;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Text;

namespace Admin.API.Controllers
{
    public class H3C:Base
    {
        private readonly H3CService _h3CService;
        public H3C()
        {
            _h3CService = AutofacContainer.Resolve<H3CService>();
        }

        [HttpGet("h3c/oasis_table_info")]
        [HttpPost("h3c/oasis_table_info")]
        public IActionResult GetH3COasisTableInfo()
        {
            var msg = new MsgResult();
            msg.data = _h3CService.H3COasisTableInfo();
            ReqOasisTableInfo req = Request.Method.ToLower() == "post" ? this.GetRequest<ReqOasisTableInfo>().Result : new ReqOasisTableInfo()
            {
                year = Request.Query["year"],
                quarter = Request.Query["quarter"],
                productLine = Request.Query["productLine"],
                office = Request.Query["office"],
                isDownload = Request.Query["isDownload"]
            };


            if (Request.Method.ToLower() == "post")
            {
                return ReJson(msg);
            }
            else
            {
                var filepath = "C:\\Users\\HowieZhan\\Desktop\\图标.xls";
               
                using (var sw = new FileStream(filepath, FileMode.Open))
                {
                    var bytes = new byte[sw.Length];
                    sw.Read(bytes, 0, bytes.Length);
                    sw.Close();
                    return new FileContentResult(bytes, "application/vnd.ms-excel");
                }

            }
        }

        public static T ParseDictionaryToModel<T>(Dictionary<string, object> dict)
        {
            //
            T obj = default(T);
            obj = Activator.CreateInstance<T>();

            //根据Key值设定 Columns
            foreach (KeyValuePair<string, object> item in dict)
            {
                PropertyInfo prop = obj.GetType().GetProperty(item.Key);
                //if (!string.IsNullOrEmpty(item.Value))
                //{
                    object value = item.Value;
                    //Nullable 获取Model类字段的真实类型
                    Type itemType = Nullable.GetUnderlyingType(prop.PropertyType) == null ? prop.PropertyType : Nullable.GetUnderlyingType(prop.PropertyType);
                    //根据Model类字段的真实类型进行转换
                    prop.SetValue(obj, Convert.ChangeType(value, itemType), null);
                //}


            }
            return obj;
        }
    }
}
