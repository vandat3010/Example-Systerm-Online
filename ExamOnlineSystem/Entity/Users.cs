using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Entity
{
    public class Users
    {
        
        public int Id { get; set; }
        
        public string UserName { get; set; }
        
        public string Name { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        [NotMapped]
        public HttpPostedFileBase AvatarUpload { get; set; }
        public long ResetPasswordCode { get; set; }
        
        public DateTime? Birthday { get; set; }
        
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public int STT { get; set; }

    }
}
