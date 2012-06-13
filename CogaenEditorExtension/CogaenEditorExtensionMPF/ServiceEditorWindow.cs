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
using CogaenEditorControls.GUI_Elements;
using CogaenDataItems.DataItems;
using CogaenDataItems.Manager;
using System.Windows.Controls;
using CBeroEdit;
using CogaenDataItems.Manager.Interfacese;


namespace CogaenEditExtension
{
    [Guid("7350C145-45C7-4C03-82E3-EE59BBFA7A72")]
    public class ServiceEditorWindow : ToolWindowPane
    {
        private CogaenEditExtensionPackage m_package;
        private UserControl m_editor;
        private ContentControl m_content = new ContentControl();

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public ServiceEditorWindow() :
            base(null)
        {
            base.Caption = "";
            base.Content = m_content;
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

        public void Reinit(CogaenEditExtensionPackage package, IEditorGui editor, String caption, String serviceName)
        {
            m_package = package;
            m_editor = editor.GetControl();
            editor.Reinit(serviceName, CogaenEditExtensionPackage.Connection);
            m_content.Content = m_editor;
            // Set the window title reading it from the resources.
            base.Caption = caption;
        }
    }
}
