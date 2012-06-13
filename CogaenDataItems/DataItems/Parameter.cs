using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using System.Collections;
using CogaenDataItems.Exporter;
//using System.Windows.Media.Media3D;
//using System.Windows.Media;
//using System.Windows;
//using System.Windows.Controls;

namespace CogaenDataItems.DataItems
{
    public enum ParameterType 
    {
         INTEGER,
         BOOL,
         DOUBLE,
         ID,
         STRING,
         COMPOUNDPARAMETER,
         SEQUENCEPARAMETER,
         GENERICPARAMETER,
         INVALID
    }

    public enum ParameterSemantic
    {
        // 3d
        POSITION2D,
        ORIENTATION2D,
        POSITION3D,
        ORIENTATION3D,
        DIMENSION2D,
        DIMENSION3D,
        MESH,
        TEXTURE,
        MATERIAL,
        SHADER,
        // StateMachine
        INITSTATE,
        TRANSITIONS,
        TRANSITION,
        FROM,
        TO,
        MSG,
        // end
        NONE
    }

    public class ParameterTypeName
    {
        private String m_name;
        private ParameterType m_type;

        public static ParameterType TypeFromString(String type) {
            if (type == "intParameter")
            {
                return ParameterType.INTEGER;
            }
            else if (type == "realParameter")
            {
                return ParameterType.DOUBLE;
            }
            else if (type == "idParameter")
            {
                return ParameterType.ID;
            }
            else if (type == "stringParameter")
            {
                return ParameterType.STRING;
            }
            else if (type == "booleanParameter" || type == "boolParameter")
            {
                return ParameterType.BOOL;
            }
            else if (type == "compoundParameter")
            {
                return ParameterType.COMPOUNDPARAMETER;
            }
            else if (type == "sequenceParameter")
            {
                return ParameterType.SEQUENCEPARAMETER;
            }
            else if (type == "genericParameter")
            {
                return ParameterType.GENERICPARAMETER;
            }
            return ParameterType.STRING;
        }

        public static String TypeToString(ParameterType type)
        {
            if (type == ParameterType.INTEGER)
            {
                return "intParameter";
            }
            else if (type == ParameterType.DOUBLE)
            {
                return "realParameter";
            }
            else if (type == ParameterType.ID)
            {
                return "idParameter";
            }
            else if (type == ParameterType.STRING)
            {
                return "stringParameter";
            }
            else if (type == ParameterType.BOOL)
            {
                return "boolParameter";
            }
            else if (type == ParameterType.COMPOUNDPARAMETER)
            {
                return "compoundParameter";
            }
            else if (type == ParameterType.SEQUENCEPARAMETER)
            {
                return "sequenceParameter";
            }
            else if (type == ParameterType.GENERICPARAMETER)
            {
                return "genericParameter";
            }
            return "invalid";
        }

        public static String SemanticToString(ParameterSemantic semantic)
        {
            if (semantic == ParameterSemantic.MESH)
            {
                return "mesh";
            }
            else if (semantic == ParameterSemantic.MATERIAL)
            {
                return "material";
            }
            else if (semantic == ParameterSemantic.ORIENTATION2D)
            {
                return "orientation2d";
            }
            else if (semantic == ParameterSemantic.ORIENTATION3D)
            {
                return "orientation3d";
            }
            else if (semantic == ParameterSemantic.POSITION2D)
            {
                return "position2d";
            }
            else if (semantic == ParameterSemantic.POSITION3D)
            {
                return "position3d";
            }
            else if (semantic == ParameterSemantic.DIMENSION2D)
            {
                return "dimension2d";
            }
            else if (semantic == ParameterSemantic.DIMENSION3D)
            {
                return "dimension3d";
            }
            else if (semantic == ParameterSemantic.SHADER)
            {
                return "shader";
            }
            else if (semantic == ParameterSemantic.TEXTURE)
            {
                return "texture";
            }
            else if (semantic == ParameterSemantic.INITSTATE)
            {
                return "initstate";
            }
            else if (semantic == ParameterSemantic.TRANSITION)
            {
                return "transition";
            }
            else if (semantic == ParameterSemantic.TRANSITIONS)
            {
                return "transitions";
            }
            else if (semantic == ParameterSemantic.FROM)
            {
                return "from";
            }
            else if (semantic == ParameterSemantic.TO)
            {
                return "to";
            }
            else if (semantic == ParameterSemantic.MSG)
            {
                return "msg";
            }
            return "invalid";
        }

        public static ParameterSemantic SemanticFromString(String semantic)
        {
            if (semantic == "mesh")
            {
                return ParameterSemantic.MESH;
            }
            else if (semantic == "material")
            {
                return ParameterSemantic.MATERIAL;
            }
            else if (semantic == "orientation2d")
            {
                return ParameterSemantic.ORIENTATION2D;
            }
            else if (semantic == "orientation3d")
            {
                return ParameterSemantic.ORIENTATION3D;
            }
            else if (semantic == "position2d")
            {
                return ParameterSemantic.POSITION2D;
            }
            else if (semantic == "position3d")
            {
                return ParameterSemantic.POSITION3D;
            }
            else if (semantic == "dimension2d")
            {
                return ParameterSemantic.DIMENSION2D;
            }
            else if (semantic == "dimension3d")
            {
                return ParameterSemantic.DIMENSION3D;
            }
            else if (semantic == "shader")
            {
                return ParameterSemantic.SHADER;
            }
            else if (semantic == "texture")
            {
                return ParameterSemantic.TEXTURE;
            }
            else if (semantic == "initstate")
            {
                return ParameterSemantic.INITSTATE;
            }
            else if (semantic == "transition")
            {
                return ParameterSemantic.TRANSITION;
            }
            else if (semantic == "transitions")
            {
                return ParameterSemantic.TRANSITIONS;
            }
            else if (semantic == "from")
            {
                return ParameterSemantic.FROM;
            }
            else if (semantic == "to")
            {
                return ParameterSemantic.TO;
            }
            else if (semantic == "msg")
            {
                return ParameterSemantic.MSG;
            }
            return ParameterSemantic.NONE;
        }

        public ParameterTypeName(String name, ParameterType type)
        {
            m_name = name;
            m_type = type;
        }

        public override string ToString()
        {
            return m_name;
        }

        public ParameterType getType()
        {
            return m_type;
        }
    }

    [Serializable]
    public class Parameter : INotifyPropertyChanged
    {
        public static Parameter m_defaultParameter = new Parameter("_default_", ParameterType.INVALID,"",0);
        public static  ParameterTypeName INTEGER =  new ParameterTypeName("Integer",ParameterType.INTEGER);
        public static  ParameterTypeName DOUBLE = new ParameterTypeName("Double", ParameterType.DOUBLE);
        public static  ParameterTypeName BOOL = new ParameterTypeName("Bool", ParameterType.BOOL);
        public static  ParameterTypeName ID = new ParameterTypeName("Id", ParameterType.ID);
        public static ParameterTypeName COMPUND = new ParameterTypeName("Compound", ParameterType.COMPOUNDPARAMETER);
        public static ParameterTypeName SEQUENCE = new ParameterTypeName("SEQUENCE", ParameterType.SEQUENCEPARAMETER);
        public static  ParameterTypeName STRING = new ParameterTypeName("String", ParameterType.STRING);

        #region events
        public class ParameterChangedEventArgs : EventArgs
        {
            private Parameter m_parameter;

            public Parameter Parameter
            {
                get { return m_parameter; }
            }
            public ParameterChangedEventArgs(Parameter parameter)
            {
                m_parameter = parameter;
            }
        }

        public delegate void ParameterChangedEventHandler(object sender, ParameterChangedEventArgs e);
        public event ParameterChangedEventHandler Changed;

        protected virtual void OnChanged(ParameterChangedEventArgs e)
        {
            if (this.Parent != null)
                this.Parent.OnChanged(e);
            if (Changed != null)
            {
                Changed(this, e);
            }
        }
        #endregion

        #region member

        private string m_name;
        private ParameterType m_type;
        private int m_count; // -1 means arbitrary
        private String m_values = "";
        private Parameter m_parent = m_defaultParameter;
        private bool m_editable = true;
        private ICollection m_semanticValues = null;

        public bool Editable
        {
            get { return m_editable; }
            set { m_editable = value; }
        }
        private IParameterContainer m_parentComponent;

        public IParameterContainer ParentComponent
        {
            get { return m_parentComponent; }
            set
            {
                m_parentComponent = value; 
            }
        }

        private ParameterSemantic m_semantic = ParameterSemantic.NONE;

        public ParameterSemantic Semantic
        {
            get { return m_semantic; }
            set { m_semantic = value; }
        }

        private ObservableCollection<Parameter> m_subParams = new ObservableCollection<Parameter>();

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        
        public virtual ParameterType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }
        
        public int Count
        {
            get { return m_count; }
            set { m_count = value; }
        }

        public String Values
        {
            get { return m_values; }
            set
            {
                m_values = value;
                if (ParentComponent != null && ParentComponent is Element)
                {
                    Element el = ParentComponent as Element;
                    if (el.ParentGameObject != null && el.ParentGameObject.ParentObjectBuilder != null)
                {
                    el.ParentGameObject.ParentObjectBuilder.Dirty = true;
                }
                }
                OnChanged(new ParameterChangedEventArgs(this));
                OnPropertyChanged("Values");
            }
        }

        public ICollection SemanticValues
        {
            get { return m_semanticValues; }
            set
            {
                m_semanticValues = value;
                OnPropertyChanged("SemanticValues");
            }
        }

        public Parameter Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public ObservableCollection<Parameter> Params
        {
            get { return m_subParams; }
            set { m_subParams = value; }
        }

        public bool Clonable
        {
            get { return Parent.Type == ParameterType.SEQUENCEPARAMETER; }
        }

        public bool HasValue
        {
            get { return Type != ParameterType.COMPOUNDPARAMETER && Type != ParameterType.SEQUENCEPARAMETER; }
        }
        #endregion

        #region CDtors
        public Parameter() 
        {
        }

        public Parameter(Parameter copy)
        {
            this.m_name = copy.m_name;
            this.m_parent = copy.m_parent;
            this.m_type = copy.m_type;
            this.m_values = copy.m_values;
            this.m_semantic = copy.m_semantic;
            this.m_count = copy.m_count;
            this.m_editable = copy.m_editable;
            this.m_semanticValues = copy.m_semanticValues;
            foreach (Parameter p in copy.m_subParams)
            {
                Parameter newP = new Parameter(p);
                newP.Parent = this;
                this.m_subParams.Add(newP);
            }
        }

        public Parameter(string name, ParameterType type, String values,int count)
        {
            m_name = name;
            m_type = type;
            m_count = count;
            m_values = values;
            m_editable = true;
        }
        #endregion

        /// <summary>
        /// Used to init a Parameter without raising a cahnged event
        /// </summary>
        /// <param name="values"></param>
        public void init(String values)
        {
            m_values = values;
        }

        /// <summary>
        /// Generates a deep copy of the parameter.
        /// </summary>
        /// <returns></returns>
        public Parameter copy()
        {
            return new Parameter(this);

        }

        public override string ToString()
        {
            return m_name;
        }

        // duplicates a Sequence parameter
        public void duplicate()
        {
            if (Parent != null)
            {
                if (Parent.Type == ParameterType.SEQUENCEPARAMETER)
                {
                    Parameter newParam = new Parameter(this);
                    int index = this.Parent.Params.IndexOf(this);
                    this.Parent.Params.Insert(index + 1, newParam);
                }
            }
        }

        public void Remove()
        {
            if (Parent != null)
            {
                if (Parent.Type == ParameterType.SEQUENCEPARAMETER)
                {
                    Parent.Params.Remove(this);
                }
            }
        }

        #region drag 'n'drop
        //public static void PreviewDragOver(Parameter p, DragEventArgs e)
        //{
        //    if (p != null)
        //    {
        //        if (p.Semantic == ParameterSemantic.MESH)
        //        {
        //            Pair<String, MeshGeometry3D> mesh = e.Data.GetData(typeof(Pair<String, MeshGeometry3D>)) as Pair<String, MeshGeometry3D>;
        //            if (mesh != null)
        //            {
        //                e.Effects = DragDropEffects.Copy;
        //                e.Handled = true;
        //                return;
        //            }
        //            MeshResource res = e.Data.GetData(typeof(MeshResource)) as MeshResource;
        //            if (res != null)
        //            {
        //                e.Effects = DragDropEffects.Copy;
        //                e.Handled = true;
        //                return;
        //            }
        //        }
        //        else if (p.Semantic == ParameterSemantic.TEXTURE)
        //        {
        //            Pair<String, Drawing> texture = e.Data.GetData(typeof(Pair<String, Drawing>)) as Pair<String, Drawing>;
        //            if (texture != null)
        //            {
        //                e.Effects = DragDropEffects.Copy;
        //                e.Handled = true;
        //                return;
        //            }
        //        }
        //    }
        //}

        //public static void Drop(Parameter p, DragEventArgs e)
        //{
        //    if (p != null)
        //    {
        //        Pair<String, MeshGeometry3D> mesh = e.Data.GetData(typeof(Pair<String, MeshGeometry3D>)) as Pair<String, MeshGeometry3D>;
        //        if (mesh != null)
        //        {
        //            if (p.Semantic == ParameterSemantic.MESH)
        //                p.Values = mesh.Key;
        //            e.Handled = true;
        //            return;
        //        }
        //        MeshResource res = e.Data.GetData(typeof(MeshResource)) as MeshResource;
        //        if (res != null)
        //        {
        //            if (p.Semantic == ParameterSemantic.MESH)
        //                p.Values = res.Name;
        //            e.Handled = true;
        //            return;
        //        }
        //    }
        //}
        #endregion

        #region scriptexport
        public String exportScript(IScriptExporter exporter)
        {
            StringBuilder script = new StringBuilder();
            if (m_type == ParameterType.COMPOUNDPARAMETER || m_type == ParameterType.SEQUENCEPARAMETER)
            {
                script.Append(exporter.beginParameter(m_name));
            }
            else
            {
                script.Append(exporter.setParameterValue(m_name, m_values, Type == ParameterType.ID || Type == ParameterType.STRING));
            }

            //script += exporter.beginParameter(m_subParamName);
            foreach (Parameter p in m_subParams)
            {
                script.Append(p.exportScript(exporter));
            }

            if (m_type == ParameterType.COMPOUNDPARAMETER || m_type == ParameterType.SEQUENCEPARAMETER)
            {
                script.Append(exporter.endParameter());
            }
            //script += exporter.endParameter();
            return script.ToString();
        }
        #endregion

        #region de/serialzation
        public void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(m_name.ToString());
            bw.Write(ParameterTypeName.TypeToString(m_type));
            bw.Write(ParameterTypeName.SemanticToString(m_semantic));
            bw.Write(m_values);
            bw.Write(m_count);
            bw.Write(m_subParams.Count);
            foreach (Parameter p in m_subParams)
            {
                p.serialize(bw);
            }
        }

        public void serializeToXml(XmlDocument doc, XmlElement parent)
        {
            XmlElement parameterNode = doc.CreateElement("Parameter");

            // Attributes
            XmlAttribute nameAttrib = doc.CreateAttribute("name");
            nameAttrib.Value = m_name;

            XmlAttribute typeAttrib = doc.CreateAttribute("type");
            typeAttrib.Value = m_type.ToString();

            XmlAttribute semanticAttrib = doc.CreateAttribute("semantic");
            semanticAttrib.Value = m_semantic.ToString();

            parameterNode.SetAttributeNode(nameAttrib);
            parameterNode.SetAttributeNode(typeAttrib);
            parameterNode.SetAttributeNode(semanticAttrib);

            XmlElement valueNode = doc.CreateElement("Values");
            valueNode.InnerText = m_values;
            parameterNode.AppendChild(valueNode);

            XmlElement subParameterNode = doc.CreateElement("Subparamters");
            foreach (Parameter p in m_subParams)
            {
                p.serializeToXml(doc, subParameterNode);
            }

            parent.AppendChild(parameterNode);
        }

        public void deserialize(System.IO.BinaryReader br)
        {
            m_name = br.ReadString();
            m_type = ParameterTypeName.TypeFromString(br.ReadString());
            m_semantic = ParameterTypeName.SemanticFromString(br.ReadString());
            //if (ParentComponent is Element)
            //    m_semantic = Parameter.getParameterSemantic(ParentComponent as Element, this);
            m_values = br.ReadString();
            m_count = br.ReadInt32();
            int paramCount = br.ReadInt32();
            for (int i = 0; i < paramCount; ++i)
            {
                Parameter p = new Parameter();
                p.deserialize(br);
                p.Parent = this;
                m_subParams.Add(p);
            }
        }

        public void deserializeFromXml(XmlElement parent)
        {
            foreach (XmlAttribute attrib in parent.Attributes)
            {
                if (attrib.Name == "name")
                    this.m_name = attrib.Value;
                else if (attrib.Name == "type")
                {
                    Enum.TryParse(attrib.Value, out m_type);
                }
                else if (attrib.Name == "semantic")
                {
                    Enum.TryParse(attrib.Value, out m_semantic);
                }
            }

            XmlNode valuesNode = Helper.XmlHelper.getNodeByName(parent.ChildNodes, "Values");
            if (valuesNode != null && valuesNode.FirstChild != null)
                m_values = valuesNode.FirstChild.Value;

            XmlNode subParametersNode = Helper.XmlHelper.getNodeByName(parent.ChildNodes, "Subparameters");
            if (subParametersNode != null)
            {
                foreach (XmlNode subp in subParametersNode.ChildNodes)
                {
                    if (subp is XmlElement && subp.Name == "Parameter")
                    {
                        Parameter p = new Parameter();
                        p.deserializeFromXml(subp as XmlElement);
                        m_subParams.Add(p);
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
