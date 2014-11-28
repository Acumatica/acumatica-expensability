using Acumatica.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    [DataContract]
    public class Attachment : ModelBase
    {
        private string _fileName;
        private byte[] _data;

        public Attachment()
        {
        }

        public Attachment(string fileName, byte[] data)
        {
            _fileName = fileName;
            _data = data;
        }

        [DataMember]
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                SetProperty(ref _fileName, value);
            }
        }

        [DataMember]
        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                SetProperty(ref _data, value);
            }
        }
    }
}
