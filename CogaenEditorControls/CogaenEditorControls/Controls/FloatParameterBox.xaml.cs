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
        float[] m_fValues;
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

        public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register("Increment", typeof(Single),
        typeof(FloatParameterBox), new FrameworkPropertyMetadata(1.0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIncrementChanged));

        public Single Increment
        {
            get
            {
                return (Single)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
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

        private static void OnIncrementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is FloatParameterBox)
            {
                FloatParameterBox tx = sender as FloatParameterBox;
                tx.Increment = (Single)eventArgs.NewValue;
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
                if (m_fValues == null || m_fValues.Length != values.Length)
                {
                    m_fValues = new float[values.Length];
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
                    if (float.TryParse(values[i], System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out m_fValues[i]))
                    {
                        if (m_textBoxes[i].Text != m_fValues[i].ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat))
                            m_textBoxes[i].Text = m_fValues[i].ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat);
                        if (wasNull)
                        {
                            m_textBoxes[i].KeyDown += new KeyEventHandler(FloatParameterBox_KeyDown);
                            m_textBoxes[i].MouseWheel += new MouseWheelEventHandler(FloatParameterBox_MouseWheel);
                        }
                        m_textBoxes[i].FontStyle = FontStyles.Normal;
                        m_textBoxes[i].Foreground = new SolidColorBrush(Colors.Black);
                        m_textBoxes[i].ToolTip = null;
                    }
                    else
                    {
                        m_textBoxes[i].FontStyle = FontStyles.Italic;
                        m_textBoxes[i].Foreground = new SolidColorBrush(Colors.Red);
                        m_textBoxes[i].ToolTip = "This value is invalid. Please enter a valid float value.";
                        m_valid = false;
                    }
                }

                m_stackPanel.Children.Clear();
                foreach (TextBox tb in m_textBoxes)
                {
                    m_stackPanel.Children.Add(tb);
                }
            }
        }

        private void invalid()
        {
            if (m_textBoxes != null)
            {
                foreach (TextBox tb in m_textBoxes)
                {
                    if (tb != null)
                    {
                        tb.KeyDown -= new KeyEventHandler(FloatParameterBox_KeyDown);
                        tb.MouseWheel += new MouseWheelEventHandler(FloatParameterBox_MouseWheel);
                    }
                }
                m_textBoxes = null;
            }

            m_stackPanel.Children.Clear();
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Invalid Value";
            m_stackPanel.Children.Add(textBlock);
        }

        private void updatevalues()
        {
            StringBuilder sb = new StringBuilder();
            m_valid = true;
            foreach (FloatTextBox tb in m_textBoxes)
            {
                if (tb != null)
                {
                    float value;
                    // check if value is valid
                    if (!float.TryParse(tb.Text, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out value))
                    {
                        tb.FontStyle = FontStyles.Italic;
                        tb.Foreground = new SolidColorBrush(Colors.Red);
                        tb.ToolTip = "This value is invalid. Please enter a valid float value.";
                        m_valid = false;
                    }

                    sb.Append(tb.Text);
                    sb.Append(",");
                }
            }
            sb.Remove(sb.Length - 1, 1);
            if (m_valid)
                Value = sb.ToString();
        }

        private void FloatParameterBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is FloatTextBox)
            {
                FloatTextBox tb = sender as FloatTextBox;
                float fValue;
                if (float.TryParse(tb.Text, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out fValue))
                {
                    KeyStates leftShift = Keyboard.GetKeyStates(Key.LeftShift);
                    KeyStates rightShift = Keyboard.GetKeyStates(Key.RightShift);
                    KeyStates leftCtrl = Keyboard.GetKeyStates(Key.LeftCtrl);
                    KeyStates rightCtrl = Keyboard.GetKeyStates(Key.RightCtrl);
                    float alter = 1.0f;
                    if (leftShift == KeyStates.Down || rightShift == KeyStates.Down)
                        alter = 10.0f;
                    if (leftCtrl == KeyStates.Down || rightCtrl == KeyStates.Down)
                        alter = 0.1f;
                    if (e.Delta > 0)
                        fValue += Increment * alter;
                    else if (e.Delta < 0)
                        fValue -= Increment * alter;
                    tb.Text = fValue.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat);
                    updatevalues();
                }
            }
        }

        private void FloatParameterBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                updatevalues();
            }
        }

        
    }
}
