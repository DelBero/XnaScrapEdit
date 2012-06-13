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
using CogaenDataItems.DataItems;

namespace CogaenEditExtension
{
    /// <summary>
    /// Interaction logic for ServiceListCtrl.xaml
    /// </summary>
    public partial class ServiceListCtrl : UserControl
    {
        private CogaenEditExtensionPackage m_package;
        public ServiceListCtrl(CogaenEditExtensionPackage package)
        {
            m_package = package;
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                FrameworkElement frameworkElement = sender as FrameworkElement;
                if (frameworkElement.DataContext is Service)
                {
                    Service service = frameworkElement.DataContext as Service;
                    m_package.ShowServiceEditor(service.Guid,service.Name);
                }
            }
            //m_package.ShowServiceEditor();
        }
    }
}
