using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CogaenControlsTest.TestClasses
{
    public class ComboBoxTester
    {
        private ObservableCollection<string> m_strings = new ObservableCollection<string>();

        public ObservableCollection<string> Values
        {
            get { return m_strings; }
            set { m_strings = value; }
        }
    }
}
