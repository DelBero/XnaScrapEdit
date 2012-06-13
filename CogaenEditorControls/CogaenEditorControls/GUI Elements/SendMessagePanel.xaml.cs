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
    /// Interaction logic for SendMessagePanel.xaml
    /// </summary>
    public partial class SendMessagePanel : UserControl
    {
        #region events
        public delegate void MessegSentEventHandler(GameMessage sender);
        public event MessegSentEventHandler MessageSentEvent;

        private void OnMessageSent(GameMessage sender)
        {
            MessageSentEvent(sender);
        }
        #endregion

        public SendMessagePanel()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
            Button button = sender as Button;
            if (button != null)
            {
                GameMessage msg = button.DataContext as GameMessage;
                if (msg != null)
                {
                    //sendMessage(msg);
                    OnMessageSent(msg);
                }
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Element oldContext = e.OldValue as Element;
            if (oldContext != null)
            {
                foreach (GameMessage msg in oldContext.Messages)
                {
                    msg.Changed -= new GameMessage.MessageChangedEventHandler(Message_Changed);
                }
            }

            Element newContext = e.NewValue as Element;
            if (newContext != null)
            {
                foreach (GameMessage msg in newContext.Messages)
                {
                    msg.Changed += new GameMessage.MessageChangedEventHandler(Message_Changed);
                }
            }
        }

        void Message_Changed(GameMessage sender, EventArgs e)
        {
            OnMessageSent(sender);
        }
    }
}
