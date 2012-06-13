using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CogaenEditor2;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.Win32;
using System.IO;


namespace CogaenEditor2.Commands
{
    

    public partial class ConnectCommand : CommandBase
    {
        public ConnectCommand(String commandName, String iconName, String iconSmall)
            : base(commandName, iconName, iconSmall)
        {
            App app = (App)Application.Current;
            app.MessageHandler.Connection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Connection_PropertyChanged);
        }

        void Connection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changed(sender, e);
        }
    }

    public partial class DisconnectCommand : CommandBase
    {

        public DisconnectCommand(String commandName, String iconName, String iconSmall)
            : base(commandName, iconName, iconSmall)
        {
            App app = (App)Application.Current;
            app.MessageHandler.Connection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Disconnection_PropertyChanged);
        }

        void Disconnection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changed(sender, e);
        }
    }

    //public partial class ImportComponentsCommand : CommandBase
    //{
    //    public ImportComponentsCommand(String commandName, String iconName, String iconSmall)
    //        : base(commandName, iconName, iconSmall)
    //    {
    //        App app = (App)Application.Current;
    //        app.MessageHandler.Connection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ImportComponents_PropertyChanged);
    //    }

    //    void ImportComponents_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //    {
    //        changed(sender, e);
    //    }
    //}
}
