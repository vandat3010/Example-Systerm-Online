using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamOnlineSystem.Common
{
    [Serializable]
    public class UserLogin
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}