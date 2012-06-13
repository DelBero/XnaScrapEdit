using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using CogaenDataItems.Exporter;
using System.Xml;

namespace CogaenDataItems.DataItems
{
    [Serializable]
    public class Element : INotifyPropertyChanged, IParameterContainer
    {
        public enum ElementSemantic
        {
            // 3d
            POSITION2D = 1,
            ORIENTATION2D = 2,
            POSITION3D = 4,
            ORIENTATION3D = 8,
            DIMENSION2D = 16,
            DIMENSION3D = 32,
            MESH = 64,
            MATERIAL = 128,
            CAMERA = 256,
            // StateMachine
            STATE = 512,
            STATEMACHINE = 1024,
            // end
            NONE = 0
        }

        #region member
        private XmlElement m_elementNode = null;
        private XmlElement m_parameterNode = null;

        private string m_name;

        public string Name
        {
          get { return m_name; }
          set 
          { 
              m_name = value; 
              OnPropertyChanged("Name");
          }
        }
        private string m_id;

        public string Id
        {
          get { return m_id; }
          set
          { 
              m_id = value;
              OnPropertyChanged("Id");
          }
        }

        private ElementSemantic m_semantic = ElementSemantic.NONE;

        public ElementSemantic Semantic
        {
            get { return m_semantic; }
            set { m_semantic = value; }
        }

        private GameObject m_parentGameObject = null;

        public GameObject ParentGameObject
        {
            get { return m_parentGameObject; }
            set 
            { 
                m_parentGameObject = value;
                OnPropertyChanged("ParentGameObject");
            }
        }

        private LiveGameObject m_parentLiveGameObject = null;

        public LiveGameObject ParentLiveGameObject
        {
            get { return m_parentLiveGameObject; }
            set
            {
                m_parentLiveGameObject = value; 
                foreach (GameMessage msg in Messages)
                {
                    msg.Target = this.ParentLiveGameObject;
                }
                OnPropertyChanged("ParentLiveGameObject");
            }
        }

        private ObservableCollection<Parameter> m_parameter = new ObservableCollection<Parameter>();

        public ObservableCollection<Parameter> Parameters
        {
            get { return m_parameter; }
            set { m_parameter = value; }
        }

        private ObservableCollection<GameMessage> m_messages = new ObservableCollection<GameMessage>();

        public ObservableCollection<GameMessage> Messages
        {
            get { return m_messages; }
            set { m_messages = value; }
        }


        private bool m_export = true;

        public bool Export
        {
            get { return m_export; }
            set { m_export = value; }
        }

        // display
        private Point m_dimension = new Point(60, 32);

        public Point Dimension
        {
            get { return m_dimension; }
            set 
            { 
                m_dimension = value;
                OnPropertyChanged("Dimension");
            }
        }

        #endregion

        #region CDtorrs
        public Element() { }
        public Element(string name)
        {
            m_name = name;
        }

        public Element(string name, string id)
        {
            m_name = name;
            m_id = id;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <returns></returns>
        public Element(Element copy)
        {
            m_name = copy.Name;
            m_id = copy.m_id;
            m_dimension = copy.m_dimension;
            m_semantic = copy.m_semantic;
            foreach (Parameter p in copy.m_parameter)
            {
                Parameter newParameter = p.copy();
                newParameter.ParentComponent = this;
                m_parameter.Add(newParameter);
            }

            foreach (GameMessage msg in copy.Messages)
            {
                GameMessage m = msg.copy();
                m_messages.Add(m);
            }
        }

        #endregion

        public override string ToString()
        {
            return m_name + "("+m_id+")";
            //return base.ToString();
        }

        public Parameter getParameter(String name)
        {
            Parameter result = null;
            foreach (Parameter p in Parameters)
            {
                if (p.Name == name)
                {
                    result = p;
                    break;
                }
                else
                    getParameter(name, p, out result);
            }
            return result;
        }

        private void getParameter(String name, Parameter parameter, out Parameter result)
        {
            result = null;
            foreach (Parameter p in parameter.Params)
            {
                if (p.Name == name)
                {
                    result = p;
                    return;
                }
                else
                {
                    getParameter(name, p, out result);
                }
            }
        }

        public string[] getParameters()
        {
            string[] s = new string[m_parameter.Count];
            int i = 0;
            foreach(Parameter p in m_parameter) {
                s[i++] = p.ToString();
            }

            return s;
        }

        // removes the component from the parent gameobject
        public void Remove()
        {
            if (ParentGameObject != null)
            {
                ParentGameObject.Remove(this);
            }
        }

        #region scriptexport
        public String exportScript(IScriptExporter exporter)
        {
            StringBuilder script = new StringBuilder();

            script.Append(exporter.beginComponent(m_name));

            foreach (Parameter p in m_parameter)
            {
                script.Append(p.exportScript(exporter));
            }

            script.Append(exporter.endComponent());

            return script.ToString();
        }
        #endregion


        #region de/serialization
        public void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(m_name.ToString());
            bw.Write(m_id.ToString());
            bw.Write(m_parameter.Count);
            foreach (Parameter p in m_parameter)
            {
                p.serialize(bw);
            }
        }

        public void serializeToXml(XmlDocument doc, XmlElement parent)
        {
            XmlElement elementNode = doc.CreateElement("Element");
            //Attributes
            XmlAttribute nameAttrib = doc.CreateAttribute("name");
            nameAttrib.Value = m_name;
            XmlAttribute idAttrib = doc.CreateAttribute("id");
            idAttrib.Value = m_id.ToString();

            elementNode.SetAttributeNode(nameAttrib);
            elementNode.SetAttributeNode(idAttrib);

            XmlElement parametersNode = doc.CreateElement("Parameters");
            elementNode.AppendChild(parametersNode);
            // Parameter
            foreach (Parameter p in m_parameter)
            {
                p.serializeToXml(doc, parametersNode);
            }

            parent.AppendChild(elementNode);
        }

        public void deserialize(System.IO.BinaryReader br)
        {
            Name = br.ReadString();
            Id = br.ReadString();
            //m_semantic = getComponentSemantic(Id);
            int paramCount = br.ReadInt32();
            for (int i = 0; i < paramCount; ++i)
            {
                Parameter newParam = new Parameter();
                newParam.Parent = DataItems.Parameter.m_defaultParameter;
                newParam.ParentComponent = this;
                newParam.deserialize(br);
                m_parameter.Add(newParam);
            }
        }

        public void deserializeFromXml(XmlElement parent)
        {
            m_elementNode = parent;
            foreach (XmlAttribute attrib in parent.Attributes)
            {
                if (attrib.Name == "name")
                    this.m_name = attrib.Value;
                else if (attrib.Name == "id")
                    this.m_id = attrib.Value;
            }

            m_parameterNode = Helper.XmlHelper.getNodeByName(m_elementNode.ChildNodes, "Parameters") as XmlElement;
            // Parameter
            if (m_parameterNode != null)
            {
                foreach (XmlNode parameter in m_parameterNode.ChildNodes)
                {
                    if (parameter is XmlElement && parameter.Name == "Parameter")
                    {
                        Parameter p = new Parameter();
                        p.deserializeFromXml(parameter as XmlElement);
                        m_parameter.Add(p);
                    }
                }
            }
        }

        #endregion

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        
    }
}
