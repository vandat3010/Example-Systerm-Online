using DAL;
using Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamOnlineSystem.Areas.Admin.Controllers
{
    public class UsersManagerController : Controller
    {
        // GET: Admin/Users
        private int pageSize = 10;
        [HttpGet]
        public ActionResult Index()
        {
            UserContext usercontext = new UserContext();
            Tuple<List<Users>, int> getDatas = usercontext.GetUserByPage( 1, this.pageSize);
            int PageSize = getDatas.Item2 / pageSize;
            int div = getDatas.Item2 % pageSize;
            if (div > 0) PageSize++;
            ViewData["ListUsers"] = Tuple.Create(getDatas.Item1, PageSize);
            return View();
        }
      public ActionResult Index(int pageIndex)
        {
            Users user = new Users();
            UserContext usercontext = new UserContext();
            Tuple<List<Users>, int> getDatas = usercontext.GetUserByPage(pageIndex, this.pageSize);
            int PageSize = getDatas.Item2 / pageSize;
            int div = getDatas.Item2 % pageSize;
            if (div > 0) PageSize++;
           /* string search = usercontext.Search(user);
            ViewBag.Search = search;*/
            ViewData["ListUsers"] = Tuple.Create(getDatas.Item1, PageSize);
            return View();
        }
        [HttpGet]
        public IEnumerable GetUserByPage(int page)
        {
            UserContext usercontext = new UserContext();
            return JsonConvert.SerializeObject(usercontext.GetUserByPage(page, this.pageSize));
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Users users, HttpPostedFileBase AvatarUpload) {
            var con = new UserContext();
            string namefile = Path.GetFileName(AvatarUpload.FileName);
            string path = Server.MapPath("/Content/Image" + "/" + namefile);
            users.Avatar = namefile;
            try
            {
                AvatarUpload.SaveAs(path);
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
            string SaveFolder = Path.Combine(Server.MapPath("/Content/Image"), namefile);
            AvatarUpload.SaveAs(SaveFolder);
           
           
            if (ModelState.IsValid)
            {
                if (con.IsEmail(users.Email))
                {
                    ModelState.AddModelError("", "Email dã tồn tại.");
                }
              else if (con.IsExistUserName(users.UserName))
                {
                    ModelState.AddModelError("", "UserName đã tồn tại.");
                }
                else
                {
                    
                    if(con.Insert(users) > 0)
                    {
                       
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm user thành công.");
                    }
                }
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult Edit(int Id)
        {
            UserContext userContext = new UserContext();
            ViewData["user"] = userContext.GetById(Id);
            return View();
        }
        [HttpPost]
        public ActionResult Edit(Users users, HttpPostedFileBase AvatarUpload)
        {
            var con = new UserContext();
            if (AvatarUpload == null) users.Avatar = null;
            else users.Avatar = AvatarUpload.FileName.ToString();
            string path = Server.MapPath("/Content/Image") + "\\" + users.Avatar;
            try
            {
                AvatarUpload.SaveAs(path);
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
            if (ModelState.IsValid)
            {
                if (con.IsExistsId(users.Id))
                {
                    if (con.Update(users) >= 0)
                    {
                        return RedirectToAction("Index", "UsersManager");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật thành công.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Người dùng không tồn tại.");
                }
            }
           
           
            
            return View("Index");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            UserContext usercon = new UserContext();
            usercon.Delete(id);
            return RedirectToAction("Index");
        }
       
    }
}