using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int IdCategory { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
