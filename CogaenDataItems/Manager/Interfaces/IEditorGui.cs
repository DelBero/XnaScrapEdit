using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CogaenDataItems.Manager.Interfacese
{
    public interface IEditorGui
    {
        void Reinit(string serviceName, object connection);
        UserControl GetControl();
        Guid GetGuid();
    }
}
