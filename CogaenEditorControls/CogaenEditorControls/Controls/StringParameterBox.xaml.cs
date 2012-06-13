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

namespace CogaenEditorControls
{
    /// <summary>
    /// Interaction logic for StringParameterBox.xaml
    /// </summary>
    public partial class StringParameterBox : UserControl
    {
        #region member
        string m_oldValue = "";
        //bool m_valid = true;

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string),
        typeof(StringParameterBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                if (m_oldValue != value)
                {
                    m_oldValue = value;
                    SetValue(ValueProperty, value);
                    m_textBox.Text = value;
                    if (ParameterChanged != null)
                        ParameterChanged(this, DataContext);
                }
            }
        }
        #endregion

        #region events
        public delegate void ParameterChangedEventHandler(object sender, object data /*the datacontext*/);
        public event ParameterChangedEventHandler ParameterChanged;
        #endregion

        public StringParameterBox()
        {
            InitializeComponent();

            m_textBox.KeyDown += new KeyEventHandler(StringParameterBox_KeyDown);
            m_textBox.TextChanged += new TextChangedEventHandler(m_textBox_TextChanged);
        }



        #region dependency property callbacks
        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is StringParameterBox)
            {
                StringParameterBox tx = sender as StringParameterBox;
                tx.Value = eventArgs.NewValue.ToString();
            }
        }
        #endregion

        private void StringParameterBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Value = m_textBox.Text;
                m_textBox.FontStyle = FontStyles.Normal;
            }
        }

        void m_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_textBox.FontStyle = FontStyles.Italic;
        }
    }
}
