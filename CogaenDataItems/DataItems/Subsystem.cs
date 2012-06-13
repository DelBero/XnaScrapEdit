using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CogaenDataItems.DataItems
{
    public class Subsystem : INotifyPropertyChanged
    {
        #region member
        private String m_name;

        private ObservableCollection<Service> m_services = new ObservableCollection<Service>();
        private ObservableCollection<Element> m_components = new ObservableCollection<Element>();

        public ObservableCollection<Service> Services
        {
            get { return m_services; }
            set 
            { 
                m_services = value;
                OnPropertyChanged("Services");
            }
        }

        public ObservableCollection<Element> Elements
        {
            get { return m_components; }
            set
            {
                m_components = value;
                OnPropertyChanged("Elements");
            }
        }

        public String Name
        {
            get { return m_name; }
            set 
            { 
                m_name = value;
                OnPropertyChanged("Name");
            }
        }
#endregion

        public Subsystem(String name)
        {
            m_name = name;
        }


        public void AddService(String serviceName, String serviceId, Guid guid)
        {
            m_services.Add(new Service(serviceName, serviceId, guid));
        }

        public void AddComponenet(String componentName, String componentId)
        {
            m_components.Add(new Element(componentName, componentId));
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


        public override string ToString()
        {
            return m_name;
        }
    }
}
