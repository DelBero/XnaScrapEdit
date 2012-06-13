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

namespace CogaenEditorControls.Controls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class IntegerTextBox : TextBox
    {
        private string m_oldText;

        //public static readonly DependencyProperty ValueProperty =
        //DependencyProperty.Register("Value", typeof(int),
        //typeof(IntegerTextBox), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        //public int Value
        //{
        //    get
        //    {
        //        return (int)GetValue(ValueProperty);
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
        typeof(IntegerTextBox), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTrimChanged));

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

        public IntegerTextBox()
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
            int i;
            if (Int32.TryParse(this.Text, out i))
            {
                //Value = i;
                if (Trim)
                    this.Text = i.ToString();
                return true;
            }
            else
                return false;
        }

        //private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        //{
        //    if (sender is IntegerTextBox)
        //    {
        //        IntegerTextBox tx = sender as IntegerTextBox;
        //        tx.Value = (int)eventArgs.NewValue;
        //    }
        //}

        private static void OnTrimChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is IntegerTextBox)
            {
                IntegerTextBox tx = sender as IntegerTextBox;
                tx.Trim = (bool)eventArgs.NewValue;
            }
        }

    }
}
