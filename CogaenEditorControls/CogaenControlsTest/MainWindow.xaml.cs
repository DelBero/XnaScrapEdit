using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CogaenControlsTest.TestClasses;
using System.Threading;
using System.Globalization;
using System.Collections.ObjectModel;
using CogaenDataItems.DataItems;
using CogaenEditorControls.Controls;
using System.ComponentModel;

namespace CogaenControlsTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ParameterTest testParam;
        ComboBoxTester comboTester;
        ColorChooser colorChooser;
        float m_f1 = 1.5f;

        public class ColorHolder : INotifyPropertyChanged
        {
            private Color col = Color.FromArgb(255, 123, 24, 56);
            public Color Col
            {
                get { return col; }
                set { col = value; OnPropertyChanged("Col"); }
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

        private ColorHolder m_colholder = new ColorHolder();
        public ColorHolder ColHolder
        {
            get { return m_colholder; }
        }

        public MainWindow()
        {
            InitializeComponent();

            testParam = new ParameterTest("1,2,3,4");
            m_FloatParameterBox.DataContext = testParam;
            m_FloatParameterText.DataContext = testParam;

            comboTester = new ComboBoxTester();
            comboTester.Values.Add("Mesh1");
            comboTester.Values.Add("Torus");
            comboTester.Values.Add("Sphere");

            m_comboBox.Values = comboTester.Values;

            this.DataContext = ColHolder;
            //ColorChooser.DataContext = ColHolder;
            //ColorDefiner.DataContext = ColHolder;
            //ColorChooser.Color = Color.FromArgb(255, 200, 204, 128);

            //System.Timers.Timer t = new System.Timers.Timer();
            //t.Interval = 1000.0;
            //t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            ////t.Start();

            m_stateMachineControl.DataContext = new StateMachine();
        }

        void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_f1 += 0.3f;
            testParam.Value = m_f1.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat) + ",2,3,4";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] values = testParam.Value.Split(',');
            float[] floats = new float[values.Length];
            for(int i = 0; i < values.Length; ++i)
            {
                float.TryParse(values[i], System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out floats[i]);
            }
            floats[0] += 0.3f;

            StringBuilder sb = new StringBuilder();
            foreach (float f in floats)
            {
                sb.Append(f.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat));
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            testParam.Value = sb.ToString();
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.ColHolder.Col = Color.FromArgb(255,255,0,0);
        }

    }
}
