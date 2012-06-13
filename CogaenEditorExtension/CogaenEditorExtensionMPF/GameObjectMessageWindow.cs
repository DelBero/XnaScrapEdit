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

namespace CogaenEditExtension
{
    [Guid("51811DF4-5378-4BB3-9095-F1CE0E40E759")]
    public class GameObjectMessageWindow : ToolWindowPane
    {
        private GameObjectMessageControl myCtrl;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public GameObjectMessageWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Resources.GamObjectMessageWindowTitle;
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
            myCtrl = new GameObjectMessageControl();
            myCtrl.MessageSentEvent += new GameObjectMessageControl.MessegSentEventHandler(myCtrl_MessageSentEvent);
            base.Content = myCtrl;
            //myCtrl.DataContext = CogaenEditExtensionPackage.Data;
        }

        void myCtrl_MessageSentEvent(GameMessage message)
        {
            CogaenEditExtensionPackage.MessageHandler.sendMessage(message);
        }

        public void selectGameObject(GameObject gameobject)
        {
            myCtrl.DataContext = gameobject;
        }
    }
}
