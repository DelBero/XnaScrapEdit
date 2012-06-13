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

namespace CogaenEditorControls.GUI_Elements
{
    /// <summary>
    /// Interaction logic for GameObjectMessageControl.xaml
    /// </summary>
    public partial class GameObjectMessageControl : UserControl
    {
        #region events
        public delegate void MessegSentEventHandler(GameMessage sender);
        public event MessegSentEventHandler MessageSentEvent;

        private LiveGameObject m_internalCopy = null;
        #endregion

        public GameObjectMessageControl()
        {
            InitializeComponent();
        }

        private void MessageSent(GameMessage sender)
        {
            if (MessageSentEvent != null)
            {
                MessageSentEvent(sender as GameMessage);
            }
        }

        public void SelectGameObject(LiveGameObject liveGameObject)
        {
            //// we need to create an internal copy so we can set the information like the TargetId (gameobject)
            //// of the selected object. This is needed to actually send messages to the correct gameobject
            //m_internalCopy = new LiveGameObject(liveGameObject);
            DataContext = liveGameObject;
        }
    }

    
}
