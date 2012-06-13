using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using CogaenDataItems.Exporter;
using CogaenDataItems.Manager;

namespace CogaenDataItems.DataItems
{
    [Serializable]
    public class MacroCall : IScriptObject
    {
        private System.Windows.Point m_dimension = new System.Windows.Point(20, 20);

        public override System.Windows.Point Dimension
        {
            get { return m_dimension; }
            set { m_dimension = value; }
        }

        private IObjectBuilder m_macro;

        public IObjectBuilder Macro
        {
            get { return m_macro; }
            set 
            { 
                m_macro = value;
                copyParameterList();
            }
        }

        private ObservableCollection<Parameter> m_parameters = new ObservableCollection<Parameter>();

        public ObservableCollection<Parameter> Parameters
        {
            get { return m_parameters; }
            set { m_parameters = value; }
        }

        private void copyParameterList() 
        {
            foreach (Parameter p in m_macro.Parameters)
            {
                Parameter newParameter = new Parameter(p);
                m_macro.AddParameter(p);
            }
        }



        #region scriptexport
        public String exportScript(IScriptExporter exporter)
        {
            String script = "";
            KeyValuePair<String,String>[] values = new KeyValuePair<String,String>[m_parameters.Count];
            int i = 0;
            foreach (Parameter p in m_parameters)
            {
                values[i] = new KeyValuePair<String,String>(p.Name,p.Values);
                ++i;
            }
            script += exporter.callMacro(m_macro.RegisteredName,values);
            script += exporter.endCallMacro();

            return script;
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
