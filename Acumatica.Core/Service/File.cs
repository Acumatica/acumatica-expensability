using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Service
{
    public class File : IFile
    {
        public File(string name, byte[] contents)
        {
            this.Name = name;
            this.Contents = contents;
        }

        public string Name { get; set; }
        public byte[] Contents { get; set; }
    }
}
