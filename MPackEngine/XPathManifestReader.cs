using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml;

namespace MPackEngine
{
    public class XPathManifestReader
    {
        XPathNavigator _nav;
        XPathDocument _docNav;
        XPathNodeIterator _NodeIter;
        String _strExpression;
        XmlDocument _xmlDocument;

        public XPathManifestReader(string filePath)
        {
            _docNav = new XPathDocument(filePath);
            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(filePath);
            _nav = _docNav.CreateNavigator();
        }

        public string GetValue(string expression)
        {
            return _nav.SelectSingleNode(expression).Value;
        }

        public List<Tuple<string,string>> GetDependenApplicationExpression(string findExpression, string appNameExpression, string appVersionExpression)
        {
            var appNameIteratior = _nav.Select(appNameExpression);
            var appVersionIterator = _nav.Select(appVersionExpression);

            var result = new List<Tuple<string, string>>();

            while (appNameIteratior.MoveNext() && appVersionIterator.MoveNext())
            {
                var appName = appNameIteratior.Current.InnerXml;
                var appVersion = appVersionIterator.Current.InnerXml;

                result.Add(new Tuple<string, string>(appName, appVersion));
            }

            return result;
        }
    }
}
