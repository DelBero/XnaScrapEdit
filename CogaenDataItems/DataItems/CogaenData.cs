using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using CogaenDataItems.Helper;
using CogaenDataItems.Manager;

namespace CogaenDataItems.DataItems
{
    public class CogaenData : INotifyPropertyChanged
    {
        public enum DisplayMode{
            Subsystem,
            Service,
            Component
        };

        #region member

        private IObjectBuilder m_liveGameObjects;

        public IObjectBuilder LiveGameObjects
        {
            get { return m_liveGameObjects; }
            set { m_liveGameObjects = value; }
        }

        // Resources in the Engine
        private ResourceFolder m_resources = new ResourceFolder("Resources");

        public ResourceFolder Resources
        {
            get { return m_resources; }
        }

        private ObservableCollection<Subsystem> m_subsystems = new ObservableCollection<Subsystem>();

        public ObservableCollection<Subsystem> Subsystems
        {
            get { return m_subsystems; }
            set { m_subsystems = value; }
        }

        private ObservableCollection<Macro> m_macros = new ObservableCollection<Macro>();

        public ObservableCollection<Macro> Macros
        {
            get { return m_macros; }
            set { m_macros = value; }
        }

        private ObservableCollection<Service> m_services = new ObservableCollection<Service>();

        public ObservableCollection<Service> Services
        {
            get { return m_services; }
            set
            {
                m_services = value;
                OnPropertyChanged("Services");
            }
        }

        private ObservableCollection<Element> m_elements = new ObservableCollection<Element>();

        public ObservableCollection<Element> ElementsList
        {
            get { return m_elements; }
            set
            {
                m_elements = value;
                OnPropertyChanged("ElementsList");
            }
        }

        #endregion

        #region CDtors
        public CogaenData(IObjectBuilder liveGameObjectBuilder)
        {
            m_liveGameObjects = liveGameObjectBuilder;
        }
        #endregion

        /// <summary>
        /// Reset all
        /// </summary>
        public void Clear()
        {
            m_macros.Clear();
            m_resources = new ResourceFolder("Resources");
            m_services.Clear();
            m_subsystems.Clear();
            m_elements.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystemName"></param>
        /// <returns></returns>
        public Subsystem AddSubsystem(String subsystemName)
        {
            Subsystem ret = new Subsystem(subsystemName);
            m_subsystems.Add(ret);
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public LiveGameObject AddLiveGameObject(String name, String[] components)
        {
            LiveGameObject newGo = new LiveGameObject(name);
            ObservableCollection<Element> comps = getAllComponents();
            foreach (String comp in components)
            {
                Element component = comps.First();
                bool notFound = component != null;
                bool unknown = true;
                while (notFound)
                {
                    if (comp.Equals(component.Id)) 
                    {
                        newGo.Elements.Add(component);
                        notFound = false;
                        unknown = true;
                    }
                }
                if (unknown)
                {
                    // TODO add default component

                }
            }
            if (m_liveGameObjects != null)
                m_liveGameObjects.ScriptObjects.Add(newGo);
            return newGo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameobject"></param>
        /// <returns></returns>
        public LiveGameObject AddLiveGameObject(XmlNode gameobject)
        {
            // get name
            String gameObjectName = XmlHelper.getNodeAttributeValue(gameobject, "Name");

            LiveGameObject newGo = new LiveGameObject(gameObjectName);
            ObservableCollection<Element> comps = getAllComponents();
            
            // find elements child node
            XmlNode elementsNode = XmlHelper.getNodeByName(gameobject.ChildNodes, "Elements");

            if (elementsNode == null)
            {
                if (m_liveGameObjects != null)
                    m_liveGameObjects.ScriptObjects.Add(newGo);
                return newGo;
            }

            Exception e = null;
            List<string> errors = new List<string>();

            foreach (XmlNode compNode in elementsNode.ChildNodes)
            {
                String compName = XmlHelper.getNodeAttributeValue(compNode, "Name");

                // check existing components
                Element compInstance = null;
                foreach(Element comp in comps)
                {
                    if (compName.Length > 0 && compName.Equals(comp.Id))
                    {
                        compInstance = new Element(comp);
                        compInstance.ParentLiveGameObject = newGo;
                        newGo.Elements.Add(compInstance);
                        break;
                    }
                }
                if (compInstance != null)
                {
                    // check data
                    XmlNode dataNode = XmlHelper.getNodeByName(compNode.ChildNodes,"Data");
                    if (dataNode != null)
                    {
                        string[] values = new string[dataNode.ChildNodes.Count];
                        int index = 0;
                        foreach (XmlNode valueNode in dataNode.ChildNodes)
                        {
                            // <Value><text>actual value</text></Value>
                            if (valueNode.FirstChild != null)
                                values[index++] = valueNode.FirstChild.InnerText;
                        }

                        // is there data?
                        if (values.Length > 0)
                        {
                            int value = 0;
                            foreach (Parameter parameter in compInstance.Parameters)
                            {
                                if (parameter.Type == ParameterType.COMPOUNDPARAMETER || parameter.Type == ParameterType.SEQUENCEPARAMETER) // TODO this needs special treatment
                                {
                                    e = paraseCompoundParameterData(parameter, values, ref value, errors, compInstance);
                                    if (e != null)
                                        break;
                                    else
                                        continue;
                                }
                                StringBuilder sb = new StringBuilder();
                                // still enough left?
                                if (value + parameter.Count > values.Length)
                                {
                                    errors.Add("Error not enough ParameterData\nParameter " + parameter.Name + " in Element " + compInstance.Name);
                                    e = new Exception();
                                    break;
                                }
                                for (int i = 0; i < parameter.Count; ++i)
                                {
                                    if (values[value] != null)
                                        sb.Append(values[value++]);
                                    if (i < parameter.Count -1)
                                        sb.Append(",");
                                }
                                parameter.Values = sb.ToString();
                            }
                        }
                    }
                }
                else 
                {
                    // TODO implement something
                }
                
            }
            if (m_liveGameObjects != null)
                m_liveGameObjects.ScriptObjects.Add(newGo);
            if (e != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string err in errors)
                {
                    sb.AppendLine(err);
                }
                e = new Exception( sb.ToString() );
                throw e;
            }
            return newGo;
        }

        private Exception paraseCompoundParameterData(Parameter compound, string[] values, ref int value, List<string> errors, Element compInstance)
        {
            Exception e = null;
            foreach (Parameter parameter in compound.Params)
            {
                if (parameter.Type == ParameterType.COMPOUNDPARAMETER || parameter.Type == ParameterType.SEQUENCEPARAMETER) // TODO this needs special treatment
                {
                    e = paraseCompoundParameterData(parameter, values, ref value, errors, compInstance);
                    if (e != null)
                        return e;
                }
                StringBuilder sb = new StringBuilder();
                // still enough left?
                if (value + parameter.Count > values.Length)
                {
                    errors.Add("Error not enough ParameterData\nParameter " + parameter.Name + " in Element " + compInstance.Name);
                    e = new Exception();
                    break;
                }
                for (int i = 0; i < parameter.Count; ++i)
                {
                    if (values[value] != null)
                        sb.Append(values[value++]);
                    if (i < parameter.Count -1)
                        sb.Append(",");
                }
                parameter.Values = sb.ToString();
            }
            return e;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        public void RemoveLiveGameObject(LiveGameObject go)
        {
            if (m_liveGameObjects != null)
                m_liveGameObjects.ScriptObjects.Remove(go);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameobject"></param>
        /// <returns></returns>
        public Macro AddMacro(XmlNode macro)
        {
            // get name
            String macroName = "";
            foreach (XmlAttribute attrib in macro.Attributes)
            {
                if (attrib.Name == "Name")
                {
                    macroName = attrib.Value;
                }
            }
            Macro newMacro = new Macro(macroName, macroName);

            m_macros.Add(newMacro);
            return newMacro;
        }

        public ObservableCollection<Service> getAllServices()
        {
            ObservableCollection<Service> list = new ObservableCollection<Service>();
            foreach (Subsystem s in m_subsystems)
            {
                foreach (Service service in s.Services)
                {
                    list.Add(service);
                }
            }

            return list;
        }

        public ObservableCollection<Element> getAllComponents()
        {
            ObservableCollection<Element> list = new ObservableCollection<Element>();
            foreach (Subsystem s in m_subsystems)
            {
                foreach (Element c in s.Elements)
                {
                    list.Add(c);
                }
            }

            return list;
        }

        public void getAllComponents(ObservableCollection<Element> list)
        {
            foreach (Subsystem s in m_subsystems)
            {
                foreach (Element c in s.Elements)
                {
                    list.Add(c);
                }
            }
        }


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
