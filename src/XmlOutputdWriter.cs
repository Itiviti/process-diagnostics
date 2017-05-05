using System.IO;
using System.Xml;

namespace ProcDiag
{
    public class XmlOutputdWriter : IWriter
    {
        XmlWriter _writtr;

        public XmlOutputdWriter(TextWriter writer)
        {
            _writtr = XmlWriter.Create(writer, new XmlWriterSettings() { WriteEndDocumentOnClose = true, Indent = true, OmitXmlDeclaration=true  });
            _writtr.WriteStartDocument();
            _writtr.WriteStartElement("output");
        }

 

        public void WriteLine(string line, string label)
        {
            _writtr.WriteStartElement(label);
            _writtr.WriteCData(line);
            _writtr.WriteEndElement();            
        }

        public void Dispose()
        {
            _writtr.WriteEndElement();           
            _writtr.Close();
        }

        public void WriteHint(string line)
        {
            _writtr.WriteComment(line);
        }

        public void WriteLine(string line)
        {
            WriteLine(line, "data");
        }
    }
}