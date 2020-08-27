using Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamOnlineSystem.Areas.Admin.Models
{
    public class ChangePasswordModel: Users
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel: Users
    {
        [Required (ErrorMessage ="Mời nhập tên đăng nhập")]
        [Display(Name = "User name")]
        

        public string UserName { get; set; }

        [Required(ErrorMessage = "Mời nhập tên mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        /*[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
*/
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel : Users
    {
        [Required(ErrorMessage = "Mời nhập tên đăng nhập")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mời nhập tên")]
        [Display(Name = "Name")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Mời nhập Email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Mời nhập Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]

        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
 
        [Compare("Password", ErrorMessage = "Password is not Correct!")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Mời nhập ngày sinh")]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }
    }
    public class ForgotPasswordModel: Users
    {
        [Required (ErrorMessage ="Email không đúng.")]
        [EmailAddress]
        public string Email { get; set; }

    }
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetPasswordCode { get; set; }
    }
}