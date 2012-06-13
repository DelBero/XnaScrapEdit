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
        private string m_oldText;

        //public static readonly DependencyProperty ValueProperty =
        //DependencyProperty.Register("Value", typeof(float),
        //typeof(FloatTextBox), new FrameworkPropertyMetadata(0.0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        //public float Value
        //{
        //    get
        //    {
        //        return (float)GetValue(ValueProperty);
        //    }
        //    set
        //    {
        //        if (Value != value)
        //        {
        //            SetValue(ValueProperty, value);
        //            this.Text = Value.ToString();
        //        }
        //    }
        //}

        public static readonly DependencyProperty TrimProperty =
        DependencyProperty.Register("Trim", typeof(bool),
        typeof(FloatTextBox), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTrimChanged));

        public bool Trim
        {
            get
            {
                return (bool)GetValue(TrimProperty);
            }
            set
            {
                SetValue(TrimProperty, value);
            }
        }

        public FloatTextBox()
        {
            InitializeComponent();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            e.Handled = true;
            int selectionPos = this.SelectionStart;
            if (checkIfNumeric())
                m_oldText = this.Text;
            else
            {
                this.Text = m_oldText;
                --selectionPos;
                if (selectionPos < 0)
                    selectionPos = 0;
            }

            this.SelectionStart = selectionPos;
            //base.OnTextChanged(e);
        }

        private bool checkIfNumeric()
        {
            if (this.Text.Length == 0)
                return true;
            float i;
            if (Single.TryParse(this.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out i))
            {
                //Value = i;
                if (Trim)
                    this.Text = i.ToString(CultureInfo.GetCultureInfo("en-US").NumberFormat);
                return true;
            }
            else
            {
                return false;
            }
        }

        //private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        //{
        //    if (sender is FloatTextBox)
        //    {
        //        FloatTextBox tx = sender as FloatTextBox;
        //        tx.Value = (float)eventArgs.NewValue;
        //    }
        //}

        private static void OnTrimChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is FloatTextBox)
            {
                FloatTextBox tx = sender as FloatTextBox;
                tx.Trim = (bool)eventArgs.NewValue;
            }
        }

    }
}
