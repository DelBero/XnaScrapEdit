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
using System.Collections.ObjectModel;
using CogaenEditorControls.Controls;
using System.Collections;

namespace CogaenEditorControls
{
    /// <summary>
    /// Interaction logic for ComboParameterBox.xaml
    /// </summary>
    public partial class ComboParameterBox : UserControl
    {
        #region member
        private bool m_selectText = false;
        private int m_pos;
        #endregion

        #region events
        public delegate void ParameterChangedEventHandler(object sender, object data /*the datacontext*/);
        public event ParameterChangedEventHandler ParameterChanged;
        #endregion

        public static readonly DependencyProperty ValuesProperty =
        DependencyProperty.Register("Values", typeof(ICollection),
        typeof(ComboParameterBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValuesChanged));

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string),
        typeof(ComboParameterBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public ICollection Values
        {
            get
            {
                return (ICollection)GetValue(ValuesProperty);
            }
            set
            {
                SetValue(ValuesProperty, value);
                if (ParameterChanged != null)
                    ParameterChanged(this, DataContext);
            }
        }

        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
                m_textBox.Text = value;
            }
        }

        public ComboParameterBox()
        {
            InitializeComponent();
        }

        #region dependency property callbacks
        private static void OnValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is ComboParameterBox)
            {
                ComboParameterBox tx = sender as ComboParameterBox;
                tx.Values = eventArgs.NewValue as ICollection;
            }
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is ComboParameterBox)
            {
                ComboParameterBox tx = sender as ComboParameterBox;
                tx.Value = eventArgs.NewValue as string;
            }
        }
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_selectText && m_pos < m_textBox.Text.Length)
            {
                m_selectText = false;
                m_textBox.SelectionStart = m_pos;
                m_textBox.SelectionLength = m_textBox.Text.Length - m_pos;
                return;
            }
            if (m_textBox.Text.Length == 0)
            {
                m_selectText = false;
                m_pos = 0;
                return;
            }
            // find match
            if (Values != null)
            {
                foreach (object _entry in Values)
                {
                    string entry = _entry.ToString();
                    if (entry.StartsWith(m_textBox.Text))
                    {
                        m_pos = m_textBox.Text.Length;
                        m_selectText = true;
                        m_textBox.Text = entry;
                        m_textBox.FontStyle = FontStyles.Italic;
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxPopup.IsOpen = true;
            m_list.DataContext = this.Values;
        }

        private void m_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Value = m_textBox.Text;
                m_textBox.FontStyle = FontStyles.Normal;
            }
        }

        private void m_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Value = m_list.SelectedItem.ToString();
            e.Handled = true;
            ComboBoxPopup.IsOpen = false;
        }

        private void ComboBoxPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            ComboBoxPopup.IsOpen = false;
        }
    }
}
