using FluentData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DAL
{
    public abstract class BaseContext
    {
        protected static IDbContext MasterDBContext()
        {
            return new DbContext().ConnectionString(ConfigurationManager.ConnectionStrings["ExamContext"].ToString(), new SqlServerProvider());
        }
        protected static IDbContext SlaveDBContext()
        {
            return new DbContext().ConnectionString(ConfigurationManager.ConnectionStrings["SlaveConnect"].ToString(), new SqlServerProvider());
        }

        public static string ConnectString = ConfigurationManager.ConnectionStrings["ExamContext"].ToString();

        public object Users { get; internal set; }
    }
}
