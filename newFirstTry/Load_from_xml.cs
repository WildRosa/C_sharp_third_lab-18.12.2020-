using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace newFirstTry
{
    public class Load_from_xml
    {
        //загружаем информацию их xml файла
        public string Load(string xmlPath, string datatype)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            XmlNode root = xmlDocument.DocumentElement;
            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    if (root.ChildNodes[i].Name==datatype)
                    {
                        return root.ChildNodes[i].InnerText;
                    }
                    continue;
                }
            }
                return "";
        }
    }
}
