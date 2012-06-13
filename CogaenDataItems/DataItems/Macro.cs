using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CogaenDataItems.DataItems
{
    [Serializable]
    public class Macro : INotifyPropertyChanged
    {
        private String m_name;
        private String m_id;

       

        public String Name
        {
            get { return m_name; }
            set 
            { 
                m_name = value;
                OnPropertyChanged("Name");
            }
        }

        public String Id
        {
            get { return m_id; }
            set 
            { 
                m_id = value;
                OnPropertyChanged("Id");
            }
        }


        #region CDtors
        public Macro(String name, String id)
        {
            m_name = name;
            m_id = id;
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
