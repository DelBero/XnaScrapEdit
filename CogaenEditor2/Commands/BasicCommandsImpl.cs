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
using CogaenEditor2.GUI.Windows;


namespace CogaenEditor2.Commands
{
    public partial class ConnectCommand : CommandBase
    {
        #region CommandBase
        public override bool CanExecute(object parameter)
        {
            App app = (App)Application.Current;
            return app.MessageHandler.Connection.CanConnect;
        }
        
        public override void Execute(object parameter)
        {
            ConnectionWindow conWnd = new ConnectionWindow();
            conWnd.Show();
        }
        #endregion
    }

    public partial class DisconnectCommand : CommandBase
    {
        #region CommandBase

        public override bool CanExecute(object parameter)
        {
            App app = (App)Application.Current;
            return !app.MessageHandler.Connection.CanConnect;
        }


        public override void Execute(object parameter)
        {
            App app = (App)Application.Current;
            app.MessageHandler.disconnect();
        }

        #endregion
    }

    //public partial class ImportComponentsCommand : CommandBase
    //{

    //    #region CommandBase

    //    public override bool CanExecute(object parameter)
    //    {
    //        return true;
    //    }


    //    public override void Execute(object parameter)
    //    {
    //        App app = (App)Application.Current;
    //        app.importComponentList();
    //    }
    //    #endregion
    //}
}
