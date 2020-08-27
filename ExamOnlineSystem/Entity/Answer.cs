using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    class Answer
    {
       public int Id { get; set; }
        public string TexAnswer { get; set; }
        public int IdQuestion { get; set; }
        public Boolean Result { get; set; }
    }
}
