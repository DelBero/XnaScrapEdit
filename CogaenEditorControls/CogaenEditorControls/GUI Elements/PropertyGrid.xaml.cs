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
using System.ComponentModel;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for PropertyGrid.xaml
    /// </summary>
    public partial class PropertyGridControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SelectedObjectProperty;

        static PropertyGridControl()
        {
            SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGridControl), new UIPropertyMetadata(0, new PropertyChangedCallback(PropertyGridControl.OnSelectedObjectChanged)));
        }


        public PropertyGridControl()
        {
            InitializeComponent();
        }

        public object SelectedObject
        {
            get
            {
                return (object)GetValue(SelectedObjectProperty);
            }
            set
            {
                SetValue(SelectedObjectProperty, value);
                this.propertyGrid.SelectedObject = value;
            }
        }

        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGridControl)d).SelectedObject = e.NewValue;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
