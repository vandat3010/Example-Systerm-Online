using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL
{
    public class UserContext : BaseContext
    {
        private static UserContext _instance;
        public static UserContext Instance()
        {
            if (null == _instance)
            {
                _instance = new UserContext();
            }
            return _instance;
        }
        public int Insert(Users obj)
        {
            using (var context = MasterDBContext())
            {
                var result = context.StoredProcedure("SP_INSERT_UPDATE_USER")
                    .Parameter("Id", null)
                    .Parameter("Name", obj.Name)
                    .Parameter("UserName", obj.UserName)
                    .Parameter("Email", obj.Email)
                    .Parameter("Password", obj.Password)
                    .Parameter("Avatar", obj.Avatar)
                    .Parameter("ResetPasswordCode", string.Empty)
                    .Parameter("Birthday", obj.Birthday)
                    .Parameter("CreateDate", DateTime.Now)
                    .Parameter("ModifyDate", DateTime.Now)
                    .Execute();
                return result;
            }
        }
        public int InsertForFacebook(Users obj)
        {
            if (IsExistUserName(obj.UserName))
            {
                return obj.Id;
            }
            else
            {
                Insert(obj);
                return obj.Id;
            }
        }
        public int Update(Users obj)
        {
            using (var context = MasterDBContext())
            {
               
                
                var cmd = context.StoredProcedure("SP_INSERT_UPDATE_USER")
                    .Parameter("Id", obj.Id)
                    .Parameter("Name", obj.Name)
                    .Parameter("UserName", obj.UserName)
                    .Parameter("Email", obj.Email)
                    .Parameter("Password", obj.Password)
                    .Parameter("Avatar", obj.Avatar)
                    .Parameter("ResetPasswordCode", obj.ResetPasswordCode)
                    .Parameter("Birthday", obj.Birthday)
                    .Parameter("CreateDate", DateTime.Now)
                    .Parameter("ModifyDate", DateTime.Now)
                    .Execute();
                return cmd;
            }
        }
        public int Delete(int id)
        {
            using(var content = MasterDBContext())
            {
                var cmd = content.StoredProcedure("Users_Delete")
                    .Parameter("Id", id)
                    .Execute();
                return cmd;
            }
        }
        public string Search(Users obj)
        {
            using(var content = MasterDBContext())
            {
                return content.StoredProcedure("SP_USER_SEARCH")
                    .Parameter("UserName", obj.UserName)
                    .Parameter("Name", obj.Name)
                    .QuerySingle<string>();
            }
        }
        public Users GetById(int Id)
        {
            using(var context = MasterDBContext())
            {
                return context.StoredProcedure("GetByIdUsers")
                    .Parameter("Id", Id)
                    .QuerySingle<Users>();
            }
        }
        public Users GetByUserName(string UserName)
        {
            using (var context = MasterDBContext())
            {
                return context.StoredProcedure("GetByUser_Name")
                    .Parameter("UserName", UserName)
                    .QuerySingle<Users>();
            }
        }
        public Users GetByEmail(string Email)
        {
            using (var context = MasterDBContext())
            {
                return context.StoredProcedure("GetByEmail")
                    .Parameter("Email", Email)
                    .QuerySingle<Users>();
            }
        }
        public Users GetByResetPasswordCode(long resetPasswordCode)
        {
            using(var context = MasterDBContext())
            {
                return context.StoredProcedure("GetByResetPasswordCode")
                    .Parameter("ResetPasswordCode", resetPasswordCode)
                    .QuerySingle<Users>();
            }
        }
        public Tuple<List<Users>, int> GetUserByPage(int pageIndex, int pageSize)
        { int toTalRecord = 0;
            List<Users> listUsers = new List<Users>();
            using (var context = MasterDBContext())
            {
                var cmd = context.StoredProcedure("Users_GetByPagging")
                    .Parameter("pageIndex", pageIndex)
                    .Parameter("pageSize", pageSize)
                    .ParameterOut("TotalRecord", FluentData.DataTypes.Int32);
                listUsers = cmd.QueryMany<Users>();
                toTalRecord = cmd.ParameterValue<int>("TotalRecord");
                int div = toTalRecord % pageSize;
                 int numPage = toTalRecord / pageSize;
                 if (div > 0) numPage++;
                Tuple<List<Users>, int> dataReturn = Tuple.Create(listUsers, numPage);
                return dataReturn;
            }
        }
        public List<Users> GetByAllUsers()
        {
            using(var context = MasterDBContext())
            {
                return context.StoredProcedure("GetByAllUsers")
                    .QueryMany<Users>();
            }
        }
        public bool Login(string userName, string password)
        {
            int result = 0;
            bool type;
            using (var context = MasterDBContext())
            {
                var cmd = context.StoredProcedure("User_Login")
                    .Parameter("UserName", userName)
                    .Parameter("Password", password)
                    .ParameterOut("Result", FluentData.DataTypes.Int32);
                type = cmd.QuerySingle<bool>();
                result = cmd.ParameterValue<int>("Result");
            }
            if (result == 1)
            {
                return type = true;
            }
            else
            {
                return type = false;
            }
        }
        public bool IsExistUserName(string username)
        {
            if (GetByUserName(username) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IsEmail(string email)
        {
            if (GetByEmail(email) == null)
            {
                return false;

            }
            else
            {
                return true;
            }
        }
        public bool IsResetPasswordCodeExist(long resetPasswordCode)
        {
            if(GetByResetPasswordCode(resetPasswordCode) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IsExistsId(int Id)
        {
            if(GetById(Id) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public UserContext Configuration { get; }

        public bool ValidateOnSaveEnabled { get; set; }
      
    }
}
