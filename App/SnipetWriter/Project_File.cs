using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnipetWriter
{
    class Project_File
    {
        public string ProjectName { get; set; }
        public List<snippet> snippets { get; set; }

        public Project_File()
        {
            this.snippets = new List<snippet>();
        }
    }
}
