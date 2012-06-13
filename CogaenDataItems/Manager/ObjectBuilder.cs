using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
//using System.Drawing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Globalization;
using CogaenDataItems.Manager;
using CogaenDataItems.DataItems;
using CogaenDataItems.Exporter;
using System.Xml;

//using CogaenEditor.Drawing;

namespace CogaenDataItems.Manager
{
    public class ObjectBuilder : INotifyPropertyChanged, IObjectBuilder
    {
        #region RootParameter
        internal class __RootParameter : Parameter
        {
            public override ParameterType Type
            {
                get { return ParameterType.SEQUENCEPARAMETER; }
                set {  }
            }
        }
        #endregion

        #region member
        private static CogaenData m_data;

        public static CogaenData Data
        {
            get { return ObjectBuilder.m_data; }
        }

        private ObservableCollection<IScriptObject> m_scriptObjects = new ObservableCollection<IScriptObject>();
        public ObservableCollection<IScriptObject> ScriptObjects
        {
            get { return m_scriptObjects; }
            set { m_scriptObjects = value; }
        }

        XmlDocument m_document = new XmlDocument();
        #region macro
        private bool m_isMacro = false;
        private bool m_isRegistered = false; // is the macro registered via a script
        private bool m_isLive = false; // is the macro registered in xnascrap
        private String m_registeredName = "";

        public String RegisteredName
        {
            get { return m_registeredName; }
            set { m_registeredName = value; }
        }
        //private ObservableCollection<Parameter> m_parameters = new ObservableCollection<Parameter>();

        public ObservableCollection<Parameter> Parameters
        {
            get { return m_rootParameter.Params; }
            set { m_rootParameter.Params = value; }
        }
        
        /// <summary>
        /// This parameter is used so that all Macro Parameter can correctly delete themselfes
        /// </summary>
        private __RootParameter m_rootParameter = new __RootParameter();

        public bool IsRegistered
        {
            get { return m_isRegistered; }
            set 
            { 
                m_isRegistered = value;
                OnPropertyChanged("IsRegistered");
            }
        }

        public bool IsMacro
        {
            get { return m_isMacro; }
            set
            {
                m_isMacro = value;
                OnPropertyChanged("IsMacro");
            }
        }

        public bool IsLive
        {
            get { return m_isLive; }
            set
            {
                m_isLive = value;
                OnPropertyChanged("IsLive");
            }
        }

        #endregion

        private String m_name;
        #region picking and moving
        private List<IScriptObject> m_activeObject = new List<IScriptObject>();
        public List<IScriptObject> ActiveObject
        {
            get { return m_activeObject; }
            private set
            {
                foreach (IScriptObject go in m_activeObject)
                {
                    go.Selected = false;
                }
                m_activeObject.Clear();
                m_activeObject.AddRange(value);
                foreach (IScriptObject go in m_activeObject)
                {
                    go.Selected = true;
                }
                OnPropertyChanged("ActiveObject");
                OnPropertyChanged("Selected");
            }
        }

        private Point m_pickupOffset = new Point();
        public Point PickupOffset
        {
            get { return m_pickupOffset; }
            set { m_pickupOffset = value; }
        }
        private Point m_offset = new Point(0, 0);
        public Point Offset
        {
            get { return m_offset; }
            set
            {
                m_offset = value;
                OnPropertyChanged("Offset");
            }
        }
        private float m_scaling = 1.0f;
        public float Scaling
        {
            get { return m_scaling; }
            set
            {
                m_scaling = value;
                if (m_scaling <= 0.0)
                {
                    m_scaling = 0.1f;
                }
                OnPropertyChanged("Scaling");
            }
        }
        #endregion
        private bool m_dirty = false;

        public bool Dirty
        {
            get { return m_dirty; }
            set 
            { 
                m_dirty = value;
                OnPropertyChanged("Dirty");
            }
        }

        public String Name
        {
            get { return m_name; }
            set 
            { 
                m_name = value;
                //m_editor3D.Name = m_name;
                OnPropertyChanged("Name");
            }
        }

        public bool Selected
        {
            get 
            {
                if (m_activeObject != null)
                {
                    return true;
                }
                return false;
            }
        }

        #region Visual
        private ModelVisual3D m_models = new ModelVisual3D();

        public ModelVisual3D Models
        {
            get { return m_models; }
        }
        private ModelVisual3D m_positions = new ModelVisual3D();

        public ModelVisual3D Positions
        {
            get { return m_positions; }
        }
        private ModelVisual3D m_lights = new ModelVisual3D();

        public ModelVisual3D Lights
        {
            get { return m_lights; }
        }

        //private Editor3D m_editor3D = new Editor3D();
        //public Editor3D Editor3D
        //{
        //    get { return m_editor3D; }
        //    set { m_editor3D = value; }
        //}
        #endregion
        #endregion
        #region CDtors
        public ObjectBuilder(CogaenData data)
        {
            m_data = data;
            Name = "unnamed";
        }

        public ObjectBuilder(String name, CogaenData data)
        {
            m_data = data;
            Name = name;
        }
        #endregion

        #region methods
        #region gui handling
        /// <summary>
        /// picking
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IScriptObject click(Point p, bool extendSelection, bool forceClear)
        {
            bool clearSelection = !extendSelection;
            if (clearSelection)
            {
                foreach (IScriptObject so in ActiveObject)
                {
                    Rect rect = new Rect((so.Position.X * Scaling) + Offset.X, (so.Position.Y * Scaling) + Offset.Y, so.Dimension.X * Scaling, so.Dimension.Y * Scaling);
                    if (rect.Contains(p))
                    {
                        clearSelection = false;
                    }
                }
            }

            if (clearSelection || (forceClear && !extendSelection))
            {
                foreach (IScriptObject so in ActiveObject)
                {
                    so.Selected = false;
                }
                ActiveObject.Clear();
            }
            foreach (IScriptObject so in m_scriptObjects)
            {
                bool inSelection = ActiveObject.Contains(so);
                Rect rect = new Rect((so.Position.X * Scaling) + Offset.X, (so.Position.Y * Scaling) + Offset.Y, so.Dimension.X * Scaling, so.Dimension.Y * Scaling);
                //System.Console.WriteLine("Checking Point (" + p + ") against Rect ("+rect+")");
                if (rect.Contains(p))
                {
                    // save for picking
                    m_pickupOffset.X = so.Position.X - p.X;
                    m_pickupOffset.Y = so.Position.Y - p.Y;
                    so.Selected = true;
                    //
                    SetAsHighest(so);
                    // don't select it twice
                    if (!inSelection)
                    {
                        ActiveObject.Add(so);
                    }
                    else if (extendSelection)
                    {
                        ActiveObject.Remove(so);
                        so.Selected = false;
                        return so;
                    }
                    return so;
                }
                else if (inSelection)
                    so.Selected = true;
                else
                    so.Selected = false;
            }
            return null;
        }

        /// <summary>
        /// Selects all GameObjects withan the specified region.
        /// </summary>
        /// <param name="selection">Region to select.</param>
        /// <returns>All GameObject in the region</returns>
        public List<IScriptObject> select(Rect selection)
        {
            //List<IScriptObject> ret = new List<IScriptObject>();
            ActiveObject.Clear();

            foreach (IScriptObject so in m_scriptObjects)
            {
                Rect rect = new Rect((so.Position.X * Scaling) + Offset.X, (so.Position.Y * Scaling) + Offset.Y, so.Dimension.X * Scaling, so.Dimension.Y * Scaling);
                //System.Console.WriteLine("Checking Point (" + p + ") against Rect ("+rect+")");
                if (rect.IntersectsWith(selection))
                {
                    so.Selected = true;
                    ActiveObject.Add(so);
                }
            }
            return ActiveObject;
        }

        /// <summary>
        /// moving
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool move(Point p)
        {
            Dirty = true;
            foreach (IScriptObject dragedObject in ActiveObject)
            {
                Point pos = dragedObject.Position;
                pos.X = dragedObject.Position.X + p.X / Scaling;
                pos.Y = dragedObject.Position.Y + p.Y / Scaling;
                dragedObject.Position = pos;
            }
            return ActiveObject.Count > 0;
        }


        #endregion
        public GameObject newGameObject(String name)
        {
            GameObject newGo = new GameObject(name);
            m_scriptObjects.Add(newGo);
            Dirty = true;
            return newGo;
        }

        public void addScriptObjectCopy(List<IScriptObject> scriptObjects)
        {
            foreach (IScriptObject so in scriptObjects)
            {
                if (so is GameObject)
                {
                    GameObject go = so as GameObject;
                    GameObject newGo = new GameObject(go, this);
                    m_scriptObjects.Add(newGo);
                }
                else if (so is LiveGameObject && !IsLive)
                {
                    LiveGameObject lgo = so as LiveGameObject;
                    GameObject newGo = new GameObject(lgo, this);
                    m_scriptObjects.Add(newGo);
                }
                else if (so is MacroCall)
                {

                }
                else if (so is MacroRegistration)
                {

                }
                else if (so is FunctionCall)
                {

                }
                else if (so is Function)
                {

                }
            }
            updateUi();
            Dirty = true;
        }

        public void deleteScriptObject(List<IScriptObject> scriptObjects)
        {
            if (scriptObjects.Count <= 0)
                return;

            Dirty = true;

            foreach(IScriptObject scriptObject in scriptObjects)
            {
                if (scriptObject is GameObject)
                {
                    GameObject gameObject = scriptObject as GameObject;
                    //if (gameObject.PositionObject != null)
                    //{
                    //    App app = Application.Current as App;
                    //    if (app != null)
                    //    {
                    //        app.Editor3D.removeModel(gameObject.PositionObject);
                    //    }
                    //}
                }
                else if (scriptObject is MacroCall)
                {

                }
                else if (scriptObject is MacroRegistration)
                {
                    MacroRegistration macroRegistration = scriptObject as MacroRegistration;
                    macroRegistration.Script.IsRegistered = false;
                    throw new Exception("Use events to handle deleting!!");
                    //App app = App.Current as App;
                    //(app.MainWindow as MainWindow).sortMacros();
                }
                else if (scriptObject is FunctionCall)
                {

                }
                else if (scriptObject is Function)
                {

                }
                else if (scriptObject is LiveGameObject)
                {
                    LiveGameObject lgo = scriptObject as LiveGameObject;
                    //throw new Exception("Use events to handle deleting!!");
                    //App app = Application.Current as App;
                    //app.MessageHandler.deleteGameObject(lgo.Name);
                }
                m_scriptObjects.Remove(scriptObject);
            }
            Dirty = true;
        }

        public void AddParameter(Parameter p)
        {
            Dirty = true;
            p.Parent = this.m_rootParameter;
            this.Parameters.Add(p);
        }

        public void RemoveParameter(Parameter p)
        {
            this.Parameters.Remove(p);
        }

        public void commitGameObject()
        {
            ActiveObject = null;
            throw new NotImplementedException("Commit GameObject is not implemented");
        }

        /// <summary>
        /// Sort the Objects so that they dont overlap
        /// </summary>
        public void sort()
        {
            int count = m_scriptObjects.Count;
            if (count == 0)
            {
                return;
            }

            Dirty = true;

            int square = (int)Math.Ceiling(Math.Sqrt((double)count));
            int iGameObjectIndex = 0;
            double x = 0.0, y = 0.0;
            double max_x = 0.0;
            for (int i = 0; i < square; ++i)
            {
                for (int j = 0; j < square; ++j)
                {
                    if (iGameObjectIndex < count)
                    {
                        IScriptObject so = m_scriptObjects[iGameObjectIndex];
                        so.Position = new Point(x,y);
                        y += so.Dimension.Y + 10;
                        if (max_x < so.Dimension.X)
                        {
                            max_x = so.Dimension.X;
                        }
                        ++iGameObjectIndex;
                    }
                }
                y = 0;
                x += max_x + 10;
                max_x = 0.0;
            }
        }

        /// <summary>
        /// Sort from top to bottom
        /// </summary>
        public void sortTopDown()
        {
            int count = m_scriptObjects.Count;
            if (count == 0)
            {
                return;
            }

            Dirty = true;

            int iGameObjectIndex = 0;
            double x = 0.0, y = 0.0;
            for (int i = 0; i < count; ++i)
            {
                if (iGameObjectIndex < count)
                {
                    IScriptObject so = m_scriptObjects[iGameObjectIndex];
                    so.Position = new Point(x, y);
                    y += so.Dimension.Y + 10;
                    ++iGameObjectIndex;
                }
            }
        }

        /// <summary>
        /// Order the Objects by Position
        /// </summary>
        public void order()
        {
            if (m_scriptObjects.Count > 0)
                Dirty = true;
            // TODO improve insert sort
            ObservableCollection<IScriptObject> newList = new ObservableCollection<IScriptObject>();
            foreach(IScriptObject so in m_scriptObjects)
            {
                foreach (IScriptObject so2 in newList)
                {
                    if (so.Position.Y > so2.Position.Y)
                    {
                        newList.Insert(newList.IndexOf(so2),so);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remove all GameObjects
        /// </summary>
        public void clear()
        {
            m_scriptObjects.Clear();
        }

        public override string ToString()
        {
            return base.ToString();
        }


        #region scriptexport
        public String exportScript(IScriptExporter exporter)
        {
            StringBuilder script = new StringBuilder();
            if (IsMacro)
            {
                script.Append( exporter.beginMacroRegistration());
                foreach (Parameter p in this.Parameters)
                {
                    script.Append(exporter.registerMacroParameter(p.Name, p.Type, p.Values.ToString(CultureInfo.GetCultureInfo("en-US"))));
                }
                script.Append(exporter.endMacroRegistration());
                script.Append(exporter.beginMacroBody());
            }
            else
            {
                script.Append(exporter.EntryPoint());
            }
            foreach (GameObject g in m_scriptObjects)
            {
                script.Append(g.exportScript(exporter));
            }
            if (IsMacro)
            {
                script.Append(exporter.endMacroBody());
            }
            else
            {
                script.Append(exporter.End());
            }
            return script.ToString();
        }

        private String exportAsMacro(IScriptExporter exporter)
        {
            String macro = "";
            macro += exporter.beginMacroRegistration();
            foreach (Parameter p in Parameters)
            {
                macro += exporter.registerMacroParameter(p.Name, p.Type, p.Values.ToString(CultureInfo.GetCultureInfo("en-US")));
            }
            macro += exporter.endRegisterMacro();
            macro += exporter.beginMacroBody();
            foreach (GameObject g in m_scriptObjects)
            {
                macro += g.exportScript(exporter);
            }
            macro += exporter.endMacroBody();
            return macro;
        }
        #endregion

        #region de/serialization
        public void save(string filename)
        {
            m_document.Save(filename);
        }

        public void load(string filename)
        {
            deserializeFromXml(filename);
            Dirty = false;
        }

        public void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(m_name);
            bw.Write(m_offset.X);
            bw.Write(m_offset.Y);
            bw.Write(m_isMacro);
            bw.Write(Parameters.Count);
            foreach (Parameter p in Parameters)
            {
                p.serialize(bw);
            }
            bw.Write(m_scriptObjects.Count);
            foreach (IScriptObject so in m_scriptObjects)
            {
                if (so is GameObject)
                {
                    bw.Write("GameObject");
                }
                else if (so is MacroCall)
                {
                    bw.Write("MacroCall");
                }
                else if (so is MacroRegistration)
                {
                    bw.Write("MacroRegistration");
                }
                else if (so is FunctionCall)
                {
                    bw.Write("FunctionCall");
                }
                else if (so is Function)
                {
                    bw.Write("Function");
                }
                so.serialize(bw);
            }
        }

        public void serializeToXml(string filename)
        {
            m_document.RemoveAll();
            XmlElement root = m_document.CreateElement("ObjectBuilder");
            m_document.AppendChild(root);
            // Attributes
            XmlAttribute nameAttrib = m_document.CreateAttribute("name");
            nameAttrib.Value = m_name;

            XmlAttribute offset_xAttrib = m_document.CreateAttribute("offset_x");
            offset_xAttrib.Value = m_offset.X.ToString();

            XmlAttribute offset_yAttrib = m_document.CreateAttribute("offset_y");
            offset_yAttrib.Value = m_offset.Y.ToString();

            XmlAttribute macroAttrib = m_document.CreateAttribute("is_macro");
            macroAttrib.Value = m_isMacro.ToString();


            root.SetAttributeNode(nameAttrib);
            root.SetAttributeNode(offset_xAttrib);
            root.SetAttributeNode(offset_yAttrib);
            root.SetAttributeNode(macroAttrib);

            XmlElement parameters = m_document.CreateElement("Parameters");
            root.AppendChild(parameters);
            // Parameter
            foreach (Parameter p in Parameters)
            {
                p.serializeToXml(m_document, parameters);
            }

            // ScriptObjects
            XmlElement objects = m_document.CreateElement("Objects");
            root.AppendChild(objects);
            foreach (IScriptObject so in m_scriptObjects)
            {
                so.serializeToXml(m_document, objects);
            }

            save(filename);
        }

        public void deserialize(System.IO.BinaryReader br)
        {
            Parameters.Clear();
            m_scriptObjects.Clear();

            Name = br.ReadString();
            Point p = new Point();
            p.X = br.ReadDouble();
            p.Y = br.ReadDouble();
            m_isMacro = br.ReadBoolean();
            int parameterCount = br.ReadInt32();
            for (int i = 0; i < parameterCount; ++i)
            {
                Parameter param = new Parameter();
                param.deserialize(br);
                AddParameter(param);
            }
            m_offset = p;
            int goCount = br.ReadInt32();
            for (int i = 0; i < goCount; ++i)
            {
                String type = br.ReadString();
                IScriptObject so = null;
                if (type == "GameObject")
                {
                    so = new GameObject(this);
                }
                else if (type == "MacroCall")
                {
                    so = new MacroCall();
                }
                else if (type == "MacroRegistration")
                {
                    so = new MacroRegistration();
                }
                else if (type == "FunctionCall")
                {
                    so = new FunctionCall();
                }
                else if (type == "Function")
                {
                    so = new Function();
                }
                if (so != null)
                {
                    so.deserialize(br);
                    so.ParentObjectBuilder = this;
                    m_scriptObjects.Add(so);
                }
            }
        }

        public void deserializeFromXml(string filename)
        {
            m_document.Load(filename);

            XmlNode objectBuilderNode = Helper.XmlHelper.getNodeByName(m_document.ChildNodes, "ObjectBuilder");
            if (objectBuilderNode == null)
                return;

            Parameters.Clear();
            m_scriptObjects.Clear();

            foreach (XmlAttribute attrib in objectBuilderNode.Attributes)
            {
                if (attrib.Name == "name")
                    this.m_name = attrib.Value;
                else if (attrib.Name == "offset_x")
                {
                    double x;
                    if (double.TryParse(attrib.Value, out x))
                        m_offset.X = x;
                }
                else if (attrib.Name == "offset_y")
                {
                    double y;
                    if (double.TryParse(attrib.Value, out y))
                        m_offset.Y = y;
                }
                else if (attrib.Name == "is_macro")
                {
                    bool.TryParse(attrib.Value, out this.m_isMacro);
                }
            }

            // Parameters
            XmlNode parameterNode = Helper.XmlHelper.getNodeByName(objectBuilderNode.ChildNodes, "Parameters");
            if (parameterNode != null)
            {
                foreach (XmlNode parameter in parameterNode.ChildNodes)
                {
                    if (parameter is XmlElement && parameter.Name == "Parameter")
                    {
                        XmlElement param = parameter as XmlElement;
                        Parameter p = new Parameter();
                        p.deserializeFromXml(param);
                        AddParameter(p);
                    }
                }
            }

            // Objects
            XmlNode objectsNode = Helper.XmlHelper.getNodeByName(objectBuilderNode.ChildNodes, "Objects");
            if (objectsNode != null)
            {
                foreach (XmlNode object_ in objectsNode.ChildNodes)
                {
                    if (object_ is XmlElement)
                    {
                        IScriptObject so = null;
                        XmlElement obj = object_ as XmlElement;
                        if (obj.Name == "GameObject")
                        {
                            so = new GameObject(this);
                            so.deserializeFromXml(obj);
                        }
                        else if (obj.Name == "MacroCall")
                        {
                            so = new MacroCall();
                            so.deserializeFromXml(obj);
                        }
                        else if (obj.Name == "MacroRegistration")
                        {
                            so = new MacroRegistration();
                            so.deserializeFromXml(obj);
                        }
                        else if (obj.Name == "FunctionCall")
                        {
                            so = new FunctionCall();
                            so.deserializeFromXml(obj);
                        }
                        else if (obj.Name == "Function")
                        {
                            so = new Function();
                            so.deserializeFromXml(obj);
                        }
                        if (so != null)
                            m_scriptObjects.Add(so);
                    }
                }
            }
        }

        #endregion

        public void updateUi()
        {
            OnPropertyChanged("ScriptObjects");
        }

        public void RecomputeZOrder()
        {
            uint i = (uint)ScriptObjects.Count;
            foreach (IScriptObject scriptObject in ScriptObjects)
            {
                scriptObject.ZOrder = --i;
            }
        }

        private void SetAsHighest(IScriptObject so)
        {
            uint i = (uint)ScriptObjects.Count;
            so.ZOrder = --i;
            foreach (IScriptObject scriptObject in ScriptObjects)
            {
                if (scriptObject != so)
                    scriptObject.ZOrder = --i;
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
