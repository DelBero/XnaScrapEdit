using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CogaenEditorControls.Helper
{
    public class StringQueryItem : INotifyPropertyChanged
    {
        private String m_msg;
        private String m_text;
        private String m_caption;

        

        public String Message
        {
            get { return m_msg; }
            set 
            { 
                m_msg = value;
                OnPropertyChanged("Message");
            }
        }

        public String Text
        {
            get { return m_text; }
            set 
            { 
                m_text = value;
                OnPropertyChanged("Text");
            }
        }

        public String Caption
        {
            get { return m_caption; }
            set 
            { 
                m_caption = value;
                OnPropertyChanged("Caption");
            }
        }

        public StringQueryItem(String msg, String caption)
        {
            Message = msg;
            Caption = caption;
        }

        public StringQueryItem(String msg, String caption, String text)
        {
            Message = msg;
            Caption = caption;
            Text = text;
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
