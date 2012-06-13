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
    public partial class IntegerTextBox : TextBox
    {
        #region Increment
        public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register("Increment", typeof(int),
        typeof(IntegerTextBox), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIncrementChanged));

        public int Increment
        {
            get
            {
                return (int)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
            }
        }

        private static void OnIncrementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is IntegerTextBox)
            {
                IntegerTextBox tx = sender as IntegerTextBox;
                tx.Increment = (int)eventArgs.NewValue;
            }
        }


        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int iValue;
            if (int.TryParse(this.Text, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out iValue))
            {
                KeyStates alt = Keyboard.GetKeyStates(Key.LeftAlt);
                int alter = 1;
                if (alt == KeyStates.Down)
                    alter = 10;
                if (e.Delta > 0)
                    iValue += Increment * alter;
                else if (e.Delta < 0)
                    iValue -= Increment * alter;
                this.Text = iValue.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat);
            }
        }
        #endregion

        public IntegerTextBox()
        {
            InitializeComponent();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            e.Handled = true;
            int selectionPos = this.SelectionStart;
            foreach (TextChange tc in e.Changes)
            {
                this.Text = makeInt(this.Text, tc);
            }
            this.SelectionStart = selectionPos;
            //base.OnTextChanged(e);
        }

        private string makeInt(string s, TextChange tc)
        {
            // removing is free
            if (tc.AddedLength <= 0)
                return s;

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
            }
            return sb.ToString();
        }

        private void trim()
        {
            int i;
            if (int.TryParse(this.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out i))
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
