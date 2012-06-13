using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using CogaenDataItems.Manager;
using CogaenDataItems.Exporter;
using System.Xml;
using CogaenDataItems.Helper;

namespace CogaenDataItems.DataItems
{
    [Serializable]
    public class GameObject : IScriptObject
    {
        private XmlElement m_xmlNode = null;
        private XmlElement m_elementsNode = null;

        private ObservableCollection<Element> m_elements;

        public ObservableCollection<Element> Elements
        {
            get { return m_elements; }
            set 
            { 
                m_elements = value;
                OnPropertyChanged("Elements");
            }
        }
        private String m_name = "unnamed";
        
        private bool m_autoId = true;
        private bool m_showComponents = true;

        public bool ShowComponents
        {
            get { return m_showComponents; }
            set 
            { 
                m_showComponents = value;
                OnPropertyChanged("Dimension");
                OnPropertyChanged("ShowComponents");
            }
        }


        public bool AutoId
        {
            get { return m_autoId; }
            set 
            {
                if (m_autoId != value)
                {
                    setDirty();
                    m_autoId = value;
                    OnPropertyChanged("AutoId");
                }
            }
        }

        // drawing
        private Point m_dimension;

        public override Point Dimension
        {
            get
            {
                if (m_showComponents)
                {
                    return m_dimension;
                }
                else
                {
                    return new Point(m_dimension.X, m_yOffset * 3);
                }
            }
            set
            {
                m_dimension = value;
                //OnPropertyChanged("Dimension");
            }
        }


        private int m_yOffset = 20; // Component offset

        public String Name
        {
            get { return m_name; }
            set 
            {
                if (m_name.CompareTo(value) != 0)
                {
                    setDirty();
                    m_name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        #region consutrctors

        public GameObject(IObjectBuilder parent) 
        {
            m_elements = new ObservableCollection<Element>();
            m_parentObjectBuilder = parent;
        }

        public GameObject(String name)
        {
            m_name = name;
            m_elements = new ObservableCollection<Element>(); 

            // default size
            int size = m_name.Length * 6 + 30;
            m_dimension = new Point(Math.Min(250, size), m_yOffset*3);
        }

        /// <summary>
        /// Make a deep copy
        /// </summary>
        /// <param name="copy"></param>
        public GameObject(GameObject copy, IObjectBuilder parent)
        {
            m_name = copy.m_name;
            m_elements = new ObservableCollection<Element>();
            //App app = (App)App.Current;
            foreach (Element comp in copy.m_elements)
            {
                Element newComp = new Element(comp);
                newComp.ParentGameObject = this;
                m_elements.Add(newComp);
                //app._3dEditorParameter(this, newComp);
            }
            m_dimension = copy.m_dimension;
            m_autoId = copy.m_autoId;
            m_parentObjectBuilder = parent;
            Point newPos = new Point(copy.m_position.X +5,copy.m_position.Y + 5);
            m_position = newPos;
        }

        /// <summary>
        /// Make a deep copy
        /// </summary>
        /// <param name="copy"></param>
        public GameObject(LiveGameObject copy, IObjectBuilder parent)
        {
            m_name = copy.Name;
            m_elements = new ObservableCollection<Element>();
            //App app = (App)App.Current;
            foreach (Element comp in copy.Elements)
            {
                Element newComp = new Element(comp);
                newComp.ParentGameObject = this;
                m_elements.Add(newComp);
                //app._3dEditorParameter(this, newComp);
            }
            m_dimension = copy.Dimension;
            m_autoId = true;
            m_parentObjectBuilder = parent;
            Point newPos = new Point(copy.Position.X + 5, copy.Position.Y + 5);
            m_position = newPos;
        }

        #endregion

        public void Add(Element comp)
        {
            comp.ParentGameObject = this;
            setDirty();
            m_elements.Add(comp);
            if (m_elementsNode != null)
            {
                comp.serializeToXml(m_elementsNode.OwnerDocument, m_elementsNode);
            }
        }

        public void Remove(Element comp)
        {
            comp.ParentGameObject = null;
            setDirty();
            m_elements.Remove(comp);
            if (m_elementsNode != null)
            {
                XmlNode nodeToRemove = Helper.XmlHelper.getNodeByNameAttribute(m_elementsNode.ChildNodes, comp.Name);
                m_elementsNode.RemoveChild(nodeToRemove);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        private void setDirty()
        {
            if (ParentObjectBuilder != null)
                this.ParentObjectBuilder.Dirty = true;
        }

        #region drag 'n'drop
        public static void PreviewDragOver(GameObject pGo, DragEventArgs e)
        {
            if (pGo != null)
            {
                Element element = e.Data.GetData(typeof(Element)) as Element;
                if (element != null)
                {
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        public static void Drop(GameObject pGo, DragEventArgs e)
        {
            if (pGo != null)
            {
                Element element = e.Data.GetData(typeof(Element)) as Element;
                if (element != null)
                {
                    Element newComp = new Element(element);
                    if (pGo != null)
                    {
                        pGo.Add(newComp);
                        e.Handled = true;
                    }
                }
            }
        }
        #endregion

        #region scriptexport
        public String exportScript(IScriptExporter exporter)
        {
            StringBuilder script = new StringBuilder();
            if (m_autoId)
            {
                script.Append(exporter.beginGameObject());
            }
            else
            {
                script.Append(exporter.beginGameObject(m_name));
            }

            foreach (Element c in m_elements)
            {
                if (c.Export)
                {
                    script.Append(c.exportScript(exporter));
                }
            }

            script.Append(exporter.endGameObject());
            return script.ToString();
        }
        #endregion

        #region de/serialization

        public override void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(m_name.ToString());
            bw.Write(m_autoId);
            bw.Write(m_position.X);
            bw.Write(m_position.Y);
            bw.Write(m_dimension.X);
            bw.Write(m_dimension.Y);
            bw.Write(m_elements.Count);
            foreach (Element c in m_elements)
            {
                c.serialize(bw);
            }
        }

        public override void serializeToXml(XmlDocument doc, XmlElement parent)
        {
            XmlElement gameObjectNode = doc.CreateElement("GameObject");
            //Attributes
            XmlAttribute nameAttrib = doc.CreateAttribute("id");
            nameAttrib.Value = m_name;
            XmlAttribute autoidAttrib = doc.CreateAttribute("auto_id");
            autoidAttrib.Value = m_autoId.ToString();
            XmlAttribute pos_xAttrib = doc.CreateAttribute("pos_x");
            pos_xAttrib.Value = m_position.X.ToString();
            XmlAttribute pos_yAttrib = doc.CreateAttribute("pos_y");
            pos_yAttrib.Value = m_position.Y.ToString();
            //XmlAttribute dim_xAttrib = doc.CreateAttribute("dim_x");
            //dim_xAttrib.Value = m_dimension.X.ToString();
            //XmlAttribute dim_yAttrib = doc.CreateAttribute("dim_y");
            //dim_yAttrib.Value = m_dimension.Y.ToString();

            gameObjectNode.SetAttributeNode(nameAttrib);
            gameObjectNode.SetAttributeNode(autoidAttrib);
            gameObjectNode.SetAttributeNode(pos_xAttrib);
            gameObjectNode.SetAttributeNode(pos_yAttrib);
            //gameObjectNode.SetAttributeNode(dim_xAttrib);
            //gameObjectNode.SetAttributeNode(dim_yAttrib);

            XmlElement elementsNode = doc.CreateElement("Elements");
            if (m_elements.Count > 0)
                gameObjectNode.AppendChild(elementsNode);

            foreach (Element c in m_elements)
            {
                c.serializeToXml(doc, elementsNode);
            }

            parent.AppendChild(gameObjectNode);
        }

        public override void deserialize(System.IO.BinaryReader br)
        {
            Name = br.ReadString();
            m_autoId = br.ReadBoolean();
            Point p = new Point();
            p.X = br.ReadDouble();
            p.Y = br.ReadDouble();
            Position = p;
            p.X = br.ReadDouble();
            p.Y = br.ReadDouble();
            Dimension = p;
            int compCount = br.ReadInt32();
            for (int i = 0; i < compCount; ++i)
            {
                Element newComp = new Element();
                newComp.deserialize(br);
                newComp.ParentGameObject = this;
                m_elements.Add(newComp);
            }
        }

        public override void deserializeFromXml(XmlElement node)
        {
            m_xmlNode = node;
            m_elementsNode = XmlHelper.getNodeByName(node.ChildNodes, "Elements") as XmlElement;
            if (m_elementsNode == null)
                m_elementsNode = m_xmlNode.OwnerDocument.CreateElement("Elements");

            m_elements.Clear();

            Point p = new Point();
            Point d = new Point();
            foreach (XmlAttribute attrib in node.Attributes)
            {
                if (attrib.Name == "id")
                    this.m_name = attrib.Value;
                else if (attrib.Name == "auto_id")
                {
                    bool.TryParse(attrib.Value, out this.m_autoId);
                }
                else if (attrib.Name == "pos_x")
                {
                    double x;
                    if (double.TryParse(attrib.Value, out x))
                        p.X = x;
                }
                else if (attrib.Name == "pos_y")
                {
                    double y;
                    if (double.TryParse(attrib.Value, out y))
                        p.Y = y;
                }
                else if (attrib.Name == "dim_x")
                {
                    double x;
                    if (double.TryParse(attrib.Value, out x))
                        d.X = x;
                }
                else if (attrib.Name == "dim_y")
                {
                    double y;
                    if (double.TryParse(attrib.Value, out y))
                        d.Y = y;
                }
            }
            Position = p;
            //Dimension = d;

            if (m_elementsNode != null)
            {
                foreach (XmlNode element in m_elementsNode.ChildNodes)
                {
                    if (element is XmlElement && element.Name == "Element")
                    {
                        Element el = new Element();
                        el.deserializeFromXml(element as XmlElement);
                        m_elements.Add(el);
                        el.ParentGameObject = this;
                    }
                }
            }
        }

        #endregion

        //// Declare the event
        //public event PropertyChangedEventHandler PropertyChanged;

        //// Create the OnPropertyChanged method to raise the event
        //protected void OnPropertyChanged(string name)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(name));
        //    }
        //}
    }

    public class LiveGameObject : IScriptObject
    {
        #region members
        private String m_name;

        public String Name
        {
            get { return m_name; }
        }


        // drawing
        private bool m_showComponents = true;

        public bool ShowComponents
        {
            get { return m_showComponents; }
            set
            {
                m_showComponents = value;
                OnPropertyChanged("Dimension");
                OnPropertyChanged("ShowComponents");
            }
        }

        private int m_yOffset = 20; // Component offset

        private Point m_dimension;

        public override Point Dimension
        {
            get
            {
                if (m_showComponents)
                {
                    return m_dimension;
                }
                else
                {
                    return new Point(m_dimension.X, m_yOffset * 3);
                }
            }
            set
            {
                m_dimension = value;
                //OnPropertyChanged("Dimension");
            }
        }

        ObservableCollection<Element> m_elements = new ObservableCollection<Element>();

        public ObservableCollection<Element> Elements
        {
            get { return m_elements; }
        }
        #endregion

        public LiveGameObject(String name)
        {
            m_name = name;
        }

        public LiveGameObject(GameObject copy)
        {
            m_name = copy.Name;
            foreach (Element comp in copy.Elements)
            {
                m_elements.Add(comp);
            }
        }

        public LiveGameObject(LiveGameObject copy)
        {
            m_name = copy.Name;
            foreach (Element comp in copy.Elements)
            {                
                m_elements.Add(comp);
            }
        }

        #region de/serialization

        public override void serialize(System.IO.BinaryWriter bw)
        {
            //bw.Write(m_name.ToString());
            //bw.Write(m_autoId);
            //bw.Write(m_position.X);
            //bw.Write(m_position.Y);
            //bw.Write(m_dimension.X);
            //bw.Write(m_dimension.Y);
            //bw.Write(m_components.Count);
            //foreach (DataItems.Component c in m_components)
            //{
            //    c.serialize(bw);
            //}
        }

        public override void deserialize(System.IO.BinaryReader br)
        {
            //Name = br.ReadString();
            //m_autoId = br.ReadBoolean();
            //Point p = new Point();
            //p.X = br.ReadDouble();
            //p.Y = br.ReadDouble();
            //Position = p;
            //p.X = br.ReadDouble();
            //p.Y = br.ReadDouble();
            //Dimension = p;
            //int compCount = br.ReadInt32();
            //for (int i = 0; i < compCount; ++i)
            //{
            //    DataItems.Component newComp = new Component();
            //    newComp.deserialize(br);
            //    newComp.ParentGameObject = this;
            //    m_components.Add(newComp);
            //}
        }

        public override void serializeToXml(XmlDocument doc, XmlElement parent)
        {
        }

        public override void deserializeFromXml(XmlElement parent)
        {
        }

        #endregion
    }
}
