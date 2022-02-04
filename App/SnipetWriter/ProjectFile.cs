using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnipetWriter
{
    class ProjectFile
    {
        public string ProjectName { get; set; }
        public List<snipet> snipets { get; set; }

        public ProjectFile()
        {
            //used for testing this.thisList = new List<string>();
            this.snipets = new List<snipet>();
        }
    }
}
