using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemaFetcher
{
    class Program
    {
        const string Login = "admin";
        const string Password = "admin";

        static void Main(string[] args)
        {
            LoadAR303000Schema();
            LoadEP203000Schema();
            LoadEP301000Schema();
            LoadIN202000Schema();
            LoadPM301000Schema();
        }

        private static void LoadAR303000Schema()
        {
            AR303000.Screen screen = new AR303000.Screen();
            screen.CookieContainer = new System.Net.CookieContainer();
            screen.Login(Login, Password);

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AR303000.Content));
            using (System.IO.FileStream file = System.IO.File.Open(typeof(AR303000.Content).FullName + ".xml", System.IO.FileMode.OpenOrCreate))
            {
                var schema = screen.GetSchema();
                serializer.Serialize(file, schema);
            }
        }

        private static void LoadEP203000Schema()
        {
            EP203000.Screen screen = new EP203000.Screen();
            screen.CookieContainer = new System.Net.CookieContainer();
            screen.Login(Login, Password);

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(EP203000.Content));
            using (System.IO.FileStream file = System.IO.File.Open(typeof(EP203000.Content).FullName + ".xml", System.IO.FileMode.OpenOrCreate))
            {
                var schema = screen.GetSchema();
                serializer.Serialize(file, schema);
            }
        }

        private static void LoadEP301000Schema()
        {
            EP301000.Screen screen = new EP301000.Screen();
            screen.CookieContainer = new System.Net.CookieContainer();
            screen.Login(Login, Password);

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(EP301000.Content));
            using (System.IO.FileStream file = System.IO.File.Open(typeof(EP301000.Content).FullName + ".xml", System.IO.FileMode.OpenOrCreate))
            {   
                // This will allow us to determine if the expense claim is editable or not.
                screen.SetSchemaMode(EP301000.SchemaMode.Detailed);

                var schema = screen.GetSchema();
                serializer.Serialize(file, schema);
            }
        }

        private static void LoadIN202000Schema()
        {
            IN202000.Screen screen = new IN202000.Screen();
            screen.CookieContainer = new System.Net.CookieContainer();
            screen.Login(Login, Password);

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(IN202000.Content));
            using (System.IO.FileStream file = System.IO.File.Open(typeof(IN202000.Content).FullName + ".xml", System.IO.FileMode.OpenOrCreate))
            {
                var schema = screen.GetSchema();
                serializer.Serialize(file, schema);
            }
        }

        private static void LoadPM301000Schema()
        {
            PM301000.Screen screen = new PM301000.Screen();
            screen.CookieContainer = new System.Net.CookieContainer();
            screen.Login(Login, Password);

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(PM301000.Content));
            using (System.IO.FileStream file = System.IO.File.Open(typeof(PM301000.Content).FullName + ".xml", System.IO.FileMode.OpenOrCreate))
            {
                var schema = screen.GetSchema();
                serializer.Serialize(file, schema);
            }
        }
    }
}
