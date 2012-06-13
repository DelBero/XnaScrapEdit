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


namespace CogaenEditExtension
{
    [Guid("58FE6F03-D92B-46CE-9FA7-5CAB4C152E16")]
    public class LiveEditor2DWindow : ToolWindowPane
    {

        private ObjectBuilderLiveControl myCtrl;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public LiveEditor2DWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Resources.LiveEditor2DWindowTitle;
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
            ObjectBuilderLiveControl ctrl = new ObjectBuilderLiveControl();
            ctrl.ObjectDeleted += new ObjectBuilderLiveControl.DeleteObjectEventHandler(ctrl_ObjectDeleted);
            myCtrl = ctrl;
            base.Content = myCtrl;
            if (CogaenEditExtensionPackage.Data.LiveGameObjects == null)
                CogaenEditExtensionPackage.Data.LiveGameObjects = new ObjectBuilder("LiveGameObjects", CogaenEditExtensionPackage.Data);
            myCtrl.DataContext = CogaenEditExtensionPackage.Data.LiveGameObjects;
        }

        protected override void Dispose(bool disposing)
        {
            if (myCtrl is ObjectBuilderLiveControl)
            {
                ObjectBuilderLiveControl ctrl = myCtrl as ObjectBuilderLiveControl;
                ctrl.ObjectDeleted -= new ObjectBuilderLiveControl.DeleteObjectEventHandler(ctrl_ObjectDeleted);
            }
            base.Dispose(disposing);
        }

        void ctrl_ObjectDeleted(object sender, System.Collections.Generic.List<IScriptObject> go)
        {
            foreach (IScriptObject so in go)
            {
                if (so is LiveGameObject)
                    CogaenEditExtensionPackage.MessageHandler.deleteGameObject((so as LiveGameObject).Name);
            }
        }
    }
}
