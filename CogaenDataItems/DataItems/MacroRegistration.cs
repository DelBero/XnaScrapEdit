using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CogaenDataItems.Manager;

namespace CogaenDataItems.DataItems
{
    [Serializable]
    public class MacroRegistration : IScriptObject
    {
        private System.Windows.Point m_dimension = new System.Windows.Point(20, 20);

        public override System.Windows.Point Dimension
        {
            get { return m_dimension; }
            set { m_dimension = value; }
        }

        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        private IObjectBuilder m_script;

        public IObjectBuilder Script
        {
            get { return m_script; }
            set { m_script = value; }
        }


        #region CDtors
        ~MacroRegistration()
        {
            if (m_script != null)
            {
                m_script.IsRegistered = false;
            }
        }
        #endregion

        #region serialization
        public override void serialize(BinaryWriter bw)
        {

        }

        public override void deserialize(BinaryReader br)
        {

        }

        public override void serializeToXml(System.Xml.XmlDocument doc, System.Xml.XmlElement parent)
        {
        }

        public override void deserializeFromXml(System.Xml.XmlElement parent)
        {
        }
        #endregion

        
    }
}
