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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using CogaenEditExtension;
using CogaenEditorControls.GUI_Elements;
using CogaenDataItems.DataItems;
using CogaenDataItems.Manager;
using CBeroEdit;
using CogaenDataItems.Manager.Interfacese;
using CogaenEditorConnect.Communication;

namespace CogaenEditExtension
{
    /// <summary>
    /// Interaction logic for LogWin.xaml
    /// </summary>
    public partial class LogWin : UserControl
    {

        public ConnectionLog Log
        {
            get { return ConnectionLog.Instance; }
        }

        public LogWin()
        {
            InitializeComponent();
        }
    }
}




namespace CogaenEditExtension
{
    [Guid("F5BCBBDA-F13D-4BDC-AF35-60CB2C13B06C")]
    public class LogWindow : ToolWindowPane
    {
        private LogWin m_log = new LogWin();
        private ContentControl m_content = new ContentControl();

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public LogWindow() :
            base(null)
        {
            base.Caption = "";
            base.Content = m_log;
            //base.Content = new RenderManagerEditor("CBero.Service.RenderManager", CogaenEditExtensionPackage.Connection);
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            //m_editor.DataContext = CogaenEditExtensionPackage.Data.LiveGameObjects;
        }

    }
}

