using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CogaenDataItems.Manager;
using System.Xml;

namespace CogaenDataItems.DataItems
{
    [Serializable]
    public class FunctionCall : IScriptObject
    {
        private System.Windows.Point m_dimension = new System.Windows.Point(20, 20);

        public override System.Windows.Point Dimension
        {
            get { return m_dimension; }
            set { m_dimension = value; }
        }


        public override void serialize(BinaryWriter bw)
        {

        }

        public override void deserialize(BinaryReader br)
        {

        }

        public override void serializeToXml(XmlDocument doc, XmlElement parent)
        {
        }

        public override void deserializeFromXml(XmlElement parent)
        {
        }
    }
}
