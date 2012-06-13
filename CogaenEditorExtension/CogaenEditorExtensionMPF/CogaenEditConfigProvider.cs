using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio;

namespace CogaenEditExtension
{
    /// <summary>
    /// Enables the Any CPU Platform form name for CogaenEdit Projects
    /// </summary>
    [ComVisible(true), CLSCompliant(false)]
    public class CogaenEditConfigProvider : ConfigProvider
    {
        #region ctors
        public CogaenEditConfigProvider(ProjectNode manager)
			: base(manager)
		{
		}
		#endregion
		#region overridden methods
		public override int GetPlatformNames(uint celt, string[] names, uint[] actual)
		{
			if(names != null)
				names[0] = "Any CPU";

			if(actual != null)
				actual[0] = 1;

			return VSConstants.S_OK;
		}

		public override int GetSupportedPlatformNames(uint celt, string[] names, uint[] actual)
		{
			if(names != null)
				names[0] = "Any CPU";

			if(actual != null)
				actual[0] = 1;

			return VSConstants.S_OK;
		}
		#endregion
    }
}
