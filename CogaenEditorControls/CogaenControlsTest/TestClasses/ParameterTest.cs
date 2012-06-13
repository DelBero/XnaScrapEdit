using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CogaenControlsTest.TestClasses
{
    class ParameterTest : INotifyPropertyChanged
    {
        #region member
        string value;

        public string Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
                System.Console.WriteLine("New Value: "+value.ToString());
            }
        }

        float increment = 0.4f;

        public float Increment
        {
            get { return increment; }
            set
            {
                increment = value;
                OnPropertyChanged("Increment");
            }
        }

        #endregion

        #region CDtors
        public ParameterTest(String value)
        {
            Value = value;
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
