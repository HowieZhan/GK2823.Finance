using Dapper;
using Dapper.Contrib.Extensions;
using GK2823.BizLib.Shared;
using GK2823.ModelLib.Admin.API;
using GK2823.ModelLib.Shared;
using GK2823.UtilLib.Helpers;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GK2823.BizLib.Admin.Services
{
    public class AccountService: BaseService
    {
        private SSOUser GetUserInfo(string userId)
        {
            var obj = new
            {
                UserId = userId,
            };
            var item = _dBService.FYJSystemDB.QueryFirstOrDefault<SSOUser>("select fid as id,UserName as user_name,userID as user_Id from Admin where userID=@UserId", obj);
            return item;
        }

        private SSOUser GetUserInfo(string loginName,string loginPwd)
        {
            var obj = new
            {
                UserName = loginName,
                Password = SecurityHelper.CreatePwd(loginPwd)
            };
            var item = _dBService.FYJSystemDB.QueryFirstOrDefault<SSOUser>("select fid as id,UserName as user_name,userID as user_id from Admin where UserName=@UserName and Password=@Password", obj);
            return item;
        }
        public string CheckLogin(string loginName,string loginPwd)
        {
            string token = string.Empty;
            var item = GetUserInfo(loginName, loginPwd);
            if (item!=null)
            {
               token= JwtHelper.CreatJwtToken(item.user_id + "|"+ item.user_name,3600);
            }
            return token;
        }

        public SSOUser GetClientAppInfo(string token)
        {
            var strToken = JwtHelper.OpenJwtToken(token);
            var item = this.GetUserInfo(strToken.Split('|')[0]);          
            switch(_appSettings.Value.appName)
            {
                case "管理API":
                    item.others = new Dictionary<string, object>();
                    var menus = this.GetMenuByRole();
                   
                    item.others.Add("menu", menus);
                    break;
            }
            return item;
        }

        public string GetRefreshToken(string token)
        {
            var gkInfo = JwtHelper.OpenJwtToken(token);           
            token = JwtHelper.CreatJwtToken(gkInfo);
            return token;
        }
        #region 角色 
        public int CreateRole(Role role)
        {
            role.code = this.CreateSeedNo(SeedTarget.Admin.ST_ROLE);
           return  _dBService.AdminDB.InsertAsync<Role>(role).Result;
        }
        public bool UpdateRole(Role role)
        {
            var _role = _dBService.AdminDB.QueryFirstOrDefault<Role>("select * from role where code=@code",new { code=role.code});
            _role.name = role.name;
            return _dBService.AdminDB.UpdateAsync<Role>(_role).Result;
        }

       
        /// <summary>
        /// 是否删除关联菜单，默认否
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="withMenu">是否删除关联菜单，默认否</param>
        /// <returns></returns>
        public MsgResult DeleteRole(List<string> roleCodes,bool withMenu=false)
        {
            var result = new MsgResult();
          
            var role = _dBService.AdminDB.QueryFirstOrDefault<Role>($"select * from role where code in({CreateInSQL(roleCodes)})");
            var menu = _dBService.AdminDB.GetAll<RoleMenu>().Where(p => roleCodes.Contains(p.role_code)).ToList();
            if (withMenu)
            {               
                using (var trans = _dBService.AdminDB.BeginTransaction())
                {
                    try
                    {
                        _dBService.AdminDB.Delete<Role>(role);
                        _dBService.AdminDB.Delete<List<RoleMenu>>(menu);
                        trans.Commit();
                    }
                    catch(Exception ex)
                    {
                        result.message = ex.Message;
                        trans.Rollback();
                    }
                }
                return result;
            }
            else
            {
                if(menu.Count()>0)
                {
                    result.code = 500;
                    result.message = "有关联菜单，不允许删除";
                    return result;
                }
                else
                {
                    _dBService.AdminDB.Delete<Role>(role);
                    return result;
                }
            }
        }

        #endregion

        #region menu
        public int CreateMenu(Menu menu)
        {
            menu.code = this.CreateSeedNo(SeedTarget.Admin.ST_MENU);
            return _dBService.AdminDB.InsertAsync<Menu>(menu).Result;
        }

        public int CreateRoleMenu(RoleMenu roleMenu)
        {
            return _dBService.AdminDB.InsertAsync<RoleMenu>(roleMenu).Result;
        }

        public List<VWRoleMenu> GetMenuByRole()
        {
            var strRoles = this.GetRolesByToken();
            var item = _dBService.AdminDB.GetAll<VWRoleMenu>().Where(p => strRoles.Contains(p.role_code)).DistinctBy(p=>p.menu_code).ToList();
            var _item = new List<VWRoleMenu>();
            item.ForEach(p =>
            {
                if(string.IsNullOrEmpty(p.parent_code))
                {
                    _item.Add(p);
                }
                else
                {
                    var e = _item.Where(k => k.menu_code == p.parent_code).FirstOrDefault();
                    if (e != null)
                    {
                        if(e.children==null)
                        {
                            e.children = new List<VWRoleMenu>();
                            e.children.Add(p);
                        }
                        else
                        {
                            e.children.Add(p);
                        }
                    }
                }
            });
            return _item;
        }

        public List<string> GetRolesByToken()
        {
            var tokenHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            string token = null;
            if (tokenHeader != null && tokenHeader.StartsWith("Bearer"))
            {
                token = tokenHeader.Substring("Bearer ".Length).Trim();
            }
            var userId = JwtHelper.OpenJwtToken(token).Split('|')[0];
            //var roles = _dBService.AdminDB.GetAll<UserRole>().Where(p => p.user_id == userId).Select(p=>p.role_code).ToList();
            var roles = _dBService.AdminDB.Query<UserRole>("select * from user_roles where user_id=@userId", new
            {
                userId = userId
            }).ToList().Select(p => p.role_code).ToList();
            return roles;
        }
        #endregion
    }
}
