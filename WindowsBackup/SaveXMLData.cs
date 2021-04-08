using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsBackup
{
    public class SaveXMLData
    {
        public static void SaveXML(object obj, string filename)
        {

            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            TextWriter writer = new StreamWriter(filename);

            serializer.Serialize(writer, obj);
            writer.Close();
        }
        
    }
}
