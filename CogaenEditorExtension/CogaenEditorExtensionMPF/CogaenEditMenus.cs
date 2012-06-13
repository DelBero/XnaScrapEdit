using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;

namespace CogaenEditExtension
{
    public sealed class CogaenEditMenus
    {
        internal static readonly Guid guidIronPythonProjectCmdSet = new Guid(GuidList.guidCogaenEditExtensionCmdSetString);
        internal static readonly CommandID SetAsMain = new CommandID(GuidList.guidCogaenEditExtensionCmdSet, 0x3001);
    }
}
