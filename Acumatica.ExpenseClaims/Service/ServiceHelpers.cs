using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Service
{
    static class ServiceHelpers
    {
        public static System.ServiceModel.Channels.Binding GetDefaultBinding(string siteUrl)
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxBufferSize = int.MaxValue;
            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.AllowCookies = true;
            if(siteUrl.StartsWith("https", StringComparison.CurrentCultureIgnoreCase)) binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            return binding;
        }

        public static System.ServiceModel.EndpointAddress GetServiceAddress(string siteUrl, string screenId)
        {
            if (!siteUrl.EndsWith("/"))
            {
                siteUrl = siteUrl + "/";
            }

            return new System.ServiceModel.EndpointAddress(String.Format("{0}Soap/{1}.asmx", siteUrl, screenId));
        }

    }
}
