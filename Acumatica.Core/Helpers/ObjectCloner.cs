using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Helpers
{
    public static class ObjectCloner
    {
        public static T Clone<T>(T obj) where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var ms = new System.IO.MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                ms.Position = 0;
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
