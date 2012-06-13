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
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CogaenDataItems.DataItems;
using CogaenEditorControls.GUI_Elements;

namespace CogaenEditor2.GUI.Windows
{
    /// <summary>
    /// Interaction logic for SendMessageWindow.xaml
    /// </summary>
    public partial class SendMessageWindow : Window
    {
        public SendMessageWindow()
        {
            InitializeComponent();

            m_sendMessagePanel.MessageSentEvent += new SendMessagePanel.MessegSentEventHandler(MessageSentEvent);
        }


        public void Show(object context)
        {
            if (context is Element)
            {
                Element element = context as Element;
                // copy the message
                ObservableCollection<GameMessage> messages = new ObservableCollection<GameMessage>();

                foreach (GameMessage message in element.Messages)
                {
                    GameMessage newMessage = message.copy(); // deep copy
                    newMessage.Target = element.ParentLiveGameObject;
                    newMessage.Changed += new GameMessage.MessageChangedEventHandler(Message_Changed);
                    messages.Add(newMessage);

                    // try to find parameters in the components parameters
                    foreach (Parameter parameter in newMessage.Parameter.Params)
                    {
                        checkParameter(element.Parameters, parameter);
                    }
                }

                this.m_sendMessagePanel.DataContext = messages;
            }
            else if (context is Service)
            {

            }
            this.Show();
        }

        private void sendMessage(GameMessage msg)
        {
            App app = App.Current as App;
            app.MessageHandler.sendMessage(msg);
        }


        private void Message_Changed(GameMessage sender, EventArgs e)
        {
            if (!sender.ManualUpdate)
            {
                sendMessage(sender);
            }
        }


        private void MessageSentEvent(GameMessage sender)
        {
            sendMessage(sender);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App app = App.Current as App;
            //app.updateLiveGameobjectData();
            ObservableCollection<GameMessage> messages = this.m_sendMessagePanel.DataContext as ObservableCollection<GameMessage>;
            if (messages != null)
            {
                foreach (GameMessage msg in messages)
                {
                    msg.ManualUpdate = true;
                }
            }
            this.Hide();
        }

        private bool checkParameter(ObservableCollection<Parameter> parameters, Parameter msgParameter)
        {
            foreach (Parameter param in parameters)
            {
                if (msgParameter.Name == param.Name /* && parameter.Count == param.Count*/)
                {
                    msgParameter.init(param.Values);
                    return true;
                }
                else
                {
                    if (checkParameter(param.Params, msgParameter))
                        return true;
                }
            }
            return false;
        }
    }
}
