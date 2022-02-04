using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnipetWriter
{
    class Directory
    {
        public string lastAdress { get; set; }
        public List<string> projectPaths { get; set; }
        public int writerIndex { get; set; }
        public Directory ()
        {
            this.projectPaths = new List<string>();
        }
    }
}
