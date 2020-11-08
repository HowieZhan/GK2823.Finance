using Admin.API.Models;
using GK2823.BizLib.Admin.Services;
using GK2823.BizLib.Finance.Services;
using GK2823.ModelLib.Admin.API;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Admin.API.Controllers
{
    public class Account:Base
    {
        public readonly AccountService _accountService;
        public Account()
        {
            _accountService = AutofacContainer.Resolve<AccountService>();
        }

        [HttpPost("api/login")]
        public IActionResult Login([FromBody]User user)
        {
            var req = Request;
            var result = new MsgResult();
            result.data = _accountService.CheckLogin(user.username, user.password);
            if(string.IsNullOrEmpty(result.data.ToString()))
            {
                result.SetCustomErr("登录异常");
            }
            return Ok(result);
        }

        [HttpPost("api/userinfo")]
        public IActionResult GetUserInfo()
        {
            var tokenHeader = Request.Headers["Authorization"].ToString();
            string token = null;
            if (tokenHeader != null && tokenHeader.StartsWith("Bearer"))
            {
                token = tokenHeader.Substring("Bearer ".Length).Trim();
            }
            var result = new MsgResult();
            result.data = _accountService.GetClientAppInfo(token);
            if(result.data==null)
            {
                result.SetCustomErr("尝试重新登录", 401);
            }
            return ReJson(result);
        }

        [HttpPost("api/getMenu")]
        public IActionResult GetMenu()
        {         
            var result = new MsgResult();
            var token = this.GetJWT();
            var menus = _accountService.GetMenuByRole();
          

           
            return Ok(result);
        }


        [HttpPost("api/refresh_token")]
        public IActionResult RefreshToken()
        {
            var tokenHeader = Request.Headers["Authorization"].ToString();
            string token = null;
            if (tokenHeader != null && tokenHeader.StartsWith("Bearer"))
            {
                token = tokenHeader.Substring("Bearer ".Length).Trim();
            }
            var result = new MsgResult();
            result.data = _accountService.GetRefreshToken(token);
            return Ok(result);
        }

        [HttpPost("api/create_role")]
        public IActionResult CreateRole([FromBody] Role role)
        {
            var result = new MsgResult();   
            result.data = _accountService.CreateRole(role);
            result.code = result.data == null ? 500 : 200;
            return ReJson(result);
        }

        [HttpPost("api/up_role")]
        public IActionResult UpdateRole([FromBody] Role role)
        {
            var result = new MsgResult();
            result.data = _accountService.CreateRole(role);
            result.code = result.data == null ? 500 : 200;
            return ReJson(result);
        }

        [HttpPost("api/get_page_menu")]
        public IActionResult GetPageMenu([FromBody] PageSet page)
        {
            var result = new MsgResult();
            page.table = Menu.TK;
            var pageData = _accountService.GetPage<Menu>(page);
            result.data = pageData;
            return ReJson(result);
        }

        [HttpPost("api/create_menu")]
        public IActionResult CreateMenu([FromBody] Menu menu)
        {
            var result = new MsgResult();
            result.data = _accountService.CreateMenu(menu);
            result.code = result.data == null ? 500 : 200;
            return ReJson(result);
        }

        [HttpPost("api/create_role_menu")]
        public IActionResult CreateRole([FromBody] RoleMenu roleMenu)
        {
            var result = new MsgResult();
            result.data = _accountService.CreateRoleMenu(roleMenu);
            result.code = result.data == null ? 500 : 200;
            return ReJson(result);
        }
    }
}
