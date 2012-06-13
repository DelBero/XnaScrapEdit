using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CogaenDataItems.DataItems
{
    public class Service : INotifyPropertyChanged, IParameterContainer
    {
        #region member
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set 
            { 
                m_name = value;
                OnPropertyChanged("Name");
            }
        }

        private String m_id;

        public String Id
        {
            get { return m_id; }
            set 
            { 
                m_id = value;
                OnPropertyChanged("Id");
            }
        }

        private Guid m_guid;

        public Guid Guid
        {
            get { return m_guid; }
        }

        private ObservableCollection<GameMessage> m_messages = new ObservableCollection<GameMessage>();

        public ObservableCollection<GameMessage> Messages
        {
            get { return m_messages; }
            set { m_messages = value; }
        }
        #endregion

        #region CDtors
        public Service(String name, String id, Guid guid)
        {
            m_name = name;
            m_id = id;
            m_guid = guid;
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


        public override string ToString()
        {
            return m_name;
        }
    }
}
