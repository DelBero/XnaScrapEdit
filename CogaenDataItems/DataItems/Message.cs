using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;

namespace CogaenDataItems.DataItems
{
    // Just for desgin
    public class MessageList : INotifyPropertyChanged
    {
        #region member
        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private ObservableCollection<GameMessage> m_messages = new ObservableCollection<GameMessage>();

        public ObservableCollection<GameMessage> Messages
        {
            get { return m_messages; }
            set { m_messages = value; }
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

    [Serializable]
    public class GameMessage : INotifyPropertyChanged
    {
        #region events
        public delegate void MessageChangedEventHandler(GameMessage sender, EventArgs e);
        public event MessageChangedEventHandler Changed;

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }
        #endregion

        #region member

        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { 
                m_name = value;
                OnPropertyChanged("Name");
            }
        }

        private Parameter m_parameter = new Parameter();

        public Parameter Parameter
        {
            get { return m_parameter; }
            set { m_parameter = value; OnPropertyChanged("Parameter"); }
        }

        private bool m_manualUpdate = true;

        public bool ManualUpdate
        {
            get { return m_manualUpdate; }
            set { m_manualUpdate = value; OnPropertyChanged("ManualUpdate"); }
        }

        private LiveGameObject m_target = null;

        public LiveGameObject Target
        {
            get { return m_target; }
            set { m_target = value; OnPropertyChanged("Target"); }
        }

        #endregion

        #region CDtors
        public GameMessage() 
        {
            m_parameter.Changed += new DataItems.Parameter.ParameterChangedEventHandler(m_parameter_Changed);
        }

        public GameMessage(string name)
        {
            m_name = name;
            m_parameter.Changed += new DataItems.Parameter.ParameterChangedEventHandler(m_parameter_Changed);
        }

        public GameMessage(GameMessage msg)
        {
            m_name = msg.Name;
            m_parameter = msg.m_parameter.copy();
            m_parameter.Changed += new DataItems.Parameter.ParameterChangedEventHandler(m_parameter_Changed);
        }

        ~GameMessage()
        {
            if (m_parameter != null)
            {
                m_parameter.Changed -= new DataItems.Parameter.ParameterChangedEventHandler(m_parameter_Changed);
            }
        }
        #endregion

        public GameMessage copy()
        {
            return new GameMessage(this);
        }

        public override string ToString()
        {
            return m_name;
        }

        private void toXml(StringBuilder sb, Parameter p)
        {
            sb.Append("<Root>");
            // write MsgId
            sb.Append("<idParameter Id=\"");
            sb.Append(Name);
            sb.Append("\" />");
            toXmlRec(sb, p);
            sb.Append("</Root>");
        }

        private void toXmlRec(StringBuilder sb, Parameter p)
        {
            foreach (Parameter parameter in p.Params)
            {
                sb.Append("<");
                sb.Append(ParameterTypeName.TypeToString(parameter.Type));
                sb.Append(" Name=\"");
                sb.Append(parameter.Name);
                sb.Append("\" Value=\"");
                sb.Append(parameter.Values);
                sb.Append("\">");
                toXmlRec(sb, parameter);
                sb.Append("</");
                sb.Append(ParameterTypeName.TypeToString(parameter.Type));
                sb.Append(">");
            }
        }

        public String toXml(String gameobjectName)
        {
            StringBuilder sb = new StringBuilder();
            toXml(sb, m_parameter);
            return sb.ToString();
        }

        void m_parameter_Changed(object sender, Parameter.ParameterChangedEventArgs e)
        {
            if (this.ManualUpdate)
                OnChanged(e);
        }

        #region de/serialzation
        public void serialize(System.IO.BinaryWriter bw)
        {
            //bw.Write(m_name.ToString());
            //bw.Write(ParameterTypeName.TypeToString(m_type));
            //bw.Write(ParameterTypeName.SemanticToString(m_semantic));
            //bw.Write(m_values);
            //bw.Write(m_count);
            //bw.Write(m_subParams.Count);
            //foreach (Parameter p in m_subParams)
            //{
            //    p.serialize(bw);
            //}
        }

        public void deserialize(System.IO.BinaryReader br)
        {
            //m_name = br.ReadString();
            //m_type = ParameterTypeName.TypeFromString(br.ReadString());
            //m_semantic = ParameterTypeName.SemanticFromString(br.ReadString());
            //m_values = br.ReadString();
            //m_count = br.ReadInt32();
            //int paramCount = br.ReadInt32();
            //for (int i = 0; i < paramCount; ++i)
            //{
            //    Parameter p = new Parameter();
            //    p.deserialize(br);
            //    m_subParams.Add(p);
            //}
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
