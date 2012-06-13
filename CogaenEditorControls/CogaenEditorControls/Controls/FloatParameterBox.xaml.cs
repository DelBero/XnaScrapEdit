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
using System.Globalization;
using CogaenEditorControls.Controls;

namespace CogaenEditorControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FloatParameterBox : UserControl
    {
        #region member
        string m_oldValue = "";
        bool m_valid = true;
        FloatTextBox[] m_textBoxes;

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string),
        typeof(FloatParameterBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

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
                    recompute();
                    if (ParameterChanged != null)
                        ParameterChanged(this,DataContext);
                }
            }
        }
        #endregion

        #region events
        public delegate void ParameterChangedEventHandler(object sender, object data /*the datacontext*/);
        public event ParameterChangedEventHandler ParameterChanged;
        #endregion
        public FloatParameterBox()
        {
            InitializeComponent();
        }

        #region dependency property callbacks
        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is FloatParameterBox)
            {
                FloatParameterBox tx = sender as FloatParameterBox;
                tx.Value = eventArgs.NewValue.ToString();
            }
        }

        #endregion

        private void recompute()
        {
            if (Value == "")
            {
                invalid();
                return;
            }

            string[] values = Value.Split(',');

            if (values.Length >= 1)
            {
                if (m_textBoxes == null || (m_textBoxes.Length != values.Length))
                {
                    //clear
                    if (m_textBoxes != null)
                    {
                        for (int i = 0; i < m_textBoxes.Length; ++i)
                        {
                            m_textBoxes[i].ValueChanged -= new FloatTextBox.ValueChangedEventHandler(FloatParameterBox_ValueChanged);
                        }
                    }
                    m_textBoxes = new FloatTextBox[values.Length];
                }

                for (int i = 0; i < values.Length; ++i)
                {
                    bool wasNull = false;
                    if (m_textBoxes[i] == null)
                    {
                        m_textBoxes[i] = new FloatTextBox();
                        wasNull = true;
                    }
                    m_textBoxes[i].Text = values[i];
                    if (wasNull)
                    {
                        m_textBoxes[i].ValueChanged += new FloatTextBox.ValueChangedEventHandler(FloatParameterBox_ValueChanged);
                    }
                }

                m_stackPanel.Children.Clear();
                foreach (FloatTextBox tb in m_textBoxes)
                {
                    m_stackPanel.Children.Add(tb);
                }
            }
        }

        void FloatParameterBox_ValueChanged(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (FloatTextBox tb in m_textBoxes)
            {
                if (tb != null)
                {
                    sb.Append(tb.Text);
                    sb.Append(',');
                }
            }
            sb.Remove(sb.Length-1,1);
            Value = sb.ToString();
        }

        private void invalid()
        {
            if (m_textBoxes != null)
            {
                m_textBoxes = null;
            }

            m_stackPanel.Children.Clear();
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Invalid Value";
            m_stackPanel.Children.Add(textBlock);
        }
    }
}
