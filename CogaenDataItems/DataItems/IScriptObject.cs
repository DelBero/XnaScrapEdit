using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using CogaenDataItems.Manager;
using System.Xml;

namespace CogaenDataItems.DataItems
{
    // TODO make this an abstract class
    [Serializable]
    public abstract class IScriptObject: INotifyPropertyChanged
    {
        #region Properties
        protected bool m_selected = false;
        public bool Selected
        {
            get { return m_selected; }
            set
            {
                m_selected = value;
                OnPropertyChanged("Selected");
            }
        }

        protected System.Windows.Point m_position;

        public virtual System.Windows.Point Position
        {
            get { return m_position; }
            set
            {
                m_position = value;
                OnPropertyChanged("Position");
            }
        }

        private uint m_zOrder = 0;

        public uint ZOrder
        {
            get { return m_zOrder; }
            set
            {
                m_zOrder = value;
                OnPropertyChanged("ZOrder");
            }
        }

        protected IObjectBuilder m_parentObjectBuilder;

        public IObjectBuilder ParentObjectBuilder
        {
            get { return m_parentObjectBuilder; }
            set { m_parentObjectBuilder = value; }
        }
        #endregion


        #region abstract prperties
        public abstract System.Windows.Point Dimension
        {
            get;
            set;
        }
        #endregion

        #region abstract methods
        public abstract void serialize(BinaryWriter bw);
        public abstract void serializeToXml(XmlDocument doc, XmlElement parent);

        public abstract void deserialize(BinaryReader br);
        public abstract void deserializeFromXml(XmlElement parent);
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
