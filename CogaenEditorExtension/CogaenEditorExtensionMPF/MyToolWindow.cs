using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using CogaenEditExtension;
using System.Windows.Controls;

namespace CogaenEditExtension
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("d0548269-fd17-4a07-a272-764499acd936")]
    public class MyToolWindow : ToolWindowPane
    {
        private TabControl m_tab = new TabControl();
        private TabItem m_elemntsTab = new TabItem();
        private TabItem m_serviceTab = new TabItem();
        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public MyToolWindow() :
            base(null)
        {
            m_elemntsTab.Header = "Components";
            MyControl elementsCtrl = new MyControl();
            elementsCtrl.DataContext = CogaenEditExtensionPackage.Data;
            m_elemntsTab.Content = elementsCtrl;
            m_serviceTab.Header = "Services";
            ServiceListCtrl serviceLstCtrl = new ServiceListCtrl(CogaenEditExtensionPackage.MainInstance);
            serviceLstCtrl.DataContext = CogaenEditExtensionPackage.Data;
            m_serviceTab.Content = serviceLstCtrl;
            m_tab.Items.Add(m_serviceTab);
            m_tab.Items.Add(m_elemntsTab);
            this.Content = m_tab;

            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
        }

        public void SetElementsCtrl(UserControl ctrl)
        {
            m_elemntsTab.Content = ctrl;
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            ctrl.DataContext = CogaenEditExtensionPackage.Data;
        }

        public void SetServiceCtrl(UserControl ctrl)
        {
            m_serviceTab.Content = ctrl;
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            ctrl.DataContext = CogaenEditExtensionPackage.Data;
        }
    }
}
