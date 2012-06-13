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

namespace CogaenEditorControls.Controls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class FloatTextBox : TextBox
    {
        #region Increment
        public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register("Increment", typeof(float),
        typeof(FloatTextBox), new FrameworkPropertyMetadata(1.0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIncrementChanged));

        public float Increment
        {
            get
            {
                return (float)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
            }
        }

        private static void OnIncrementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is FloatTextBox)
            {
                FloatTextBox tx = sender as FloatTextBox;
                tx.Increment = (float)eventArgs.NewValue;
            }
        }


        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            float iValue;
            if (float.TryParse(this.Text, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out iValue))
            {
                KeyStates alt = Keyboard.GetKeyStates(Key.LeftAlt);
                KeyStates leftCtrl = Keyboard.GetKeyStates(Key.LeftCtrl);
                KeyStates rightCtrl = Keyboard.GetKeyStates(Key.RightCtrl);
                float alter = 1.0f;
                if (alt == KeyStates.Down)
                    alter = 10.0f;
                if (leftCtrl == KeyStates.Down || rightCtrl == KeyStates.Down)
                    alter = 0.1f;
                if (e.Delta > 0)
                    iValue += Increment * alter;
                else if (e.Delta < 0)
                    iValue -= Increment * alter;
                this.Text = iValue.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
        }
        #endregion

        #region events
        public delegate void ValueChangedEventHandler(object sender, EventArgs e);
        public event ValueChangedEventHandler ValueChanged;

        private void OnValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }
        #endregion

        public FloatTextBox()
        {
            InitializeComponent();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            e.Handled = true;
            int selectionPos = this.SelectionStart;
            foreach (TextChange tc in e.Changes)
            {
                this.Text = makeFloat(this.Text, tc);
            }
            this.SelectionStart = selectionPos;
            OnValueChanged();
            //base.OnTextChanged(e);
        }

        private string makeFloat(string s, TextChange tc)
        {
            // removing is free
            if (tc.AddedLength <= 0)
                return s;
            // check new segment for non float characters
            bool newComma = false;
            int newCommaPos = -1;
            for (int i = tc.Offset; i < s.Length; ++i)
            {
                if (s[i] == '.')
                {
                    newComma = true;
                    newCommaPos = i;
                    break;
                }
            }

            StringBuilder sb = new StringBuilder();
            int start = 0;
            if (s[start] == '-')
            {
                sb.Append(s[start++]);

            }
            for (int i = start; i < s.Length; ++i)
            {
                if (s[i] >= '0' && s[i] <= '9')
                {
                    sb.Append(s[i]);
                }
                else if (s[i] == '.' && (i == newCommaPos || !newComma))
                {
                    sb.Append(s[i]);
                }
            }
            return sb.ToString();
        }

        private void trim()
        {
            float i;
            if (Single.TryParse(this.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out i))
            {
                this.Text = i.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
        }

        private void TextBox_Validate(object sender, RoutedEventArgs e)
        {
            trim();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                trim();
        }

    }
}
