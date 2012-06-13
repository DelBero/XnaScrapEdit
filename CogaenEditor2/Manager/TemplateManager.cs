using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CogaenDataItems.Manager;
using CogaenDataItems.DataItems;

namespace CogaenEditor2.Manager
{
    public class TemplateManager : INotifyPropertyChanged
    {
        #region member
        private CogaenData m_data;
        private ObservableCollection<ObjectBuilder> m_templates = new ObservableCollection<ObjectBuilder>();
        private int m_selected = 0;

        public ObservableCollection<ObjectBuilder> Templates
        {
            get { return m_templates; }
            set { m_templates = value; }
        }

        public ObjectBuilder Template
        {
            get
            {
                if (m_selected >= 0 && m_selected < m_templates.Count)
                    return m_templates[m_selected];
                else
                    return null;
            }
            set
            {
                int index = m_templates.IndexOf(value);
                if (index > -1)
                {
                    Selected = index;
                }
            }
        }

        private ObservableCollection<ObjectBuilder> m_macros = new ObservableCollection<ObjectBuilder>();

        public ObservableCollection<ObjectBuilder> Macros
        {
            get { return m_macros; }
            set { m_macros = value; }
        }

        public int Selected
        {
            get { return m_selected; }
            set
            {
                m_selected = value;
                OnPropertyChanged("Selected");
            }
        }
        #endregion

        #region CDtors
        public TemplateManager(CogaenData data)
        {
            m_data = data;
        }
        #endregion
        public ObjectBuilder newTemplate(String name)
        {
            ObjectBuilder newObjBuild = new ObjectBuilder(name, m_data);
            m_templates.Add(newObjBuild);
            return newObjBuild;
        }

        public ObjectBuilder newMacro(String name)
        {
            ObjectBuilder newObjBuild = new ObjectBuilder(name, m_data);
            newObjBuild.IsMacro = true;
            m_templates.Add(newObjBuild);
            return newObjBuild;
        }

        public void remove(ObjectBuilder ob)
        {
            int index = m_templates.IndexOf(ob);
            if (index > 0)
            {
                Selected = index - 1;
            }
            m_templates.Remove(ob);
        }
        
        #region de/serialization

        public void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(m_templates.Count);
            foreach (ObjectBuilder g in m_templates)
            {
                g.serialize(bw);
            }
        }

        public void deserialize(System.IO.BinaryReader br)
        {
            int compCount = br.ReadInt32();
            for (int i = 0; i < compCount; ++i)
            {
                ObjectBuilder newOb = new ObjectBuilder(m_data);
                newOb.deserialize(br);
                m_templates.Add(newOb);
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
