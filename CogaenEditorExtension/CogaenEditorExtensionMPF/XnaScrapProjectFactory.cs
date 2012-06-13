using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;

namespace ProjectTypeCS
{
    [Guid(XnaScrapProjectFactoryGuidString)]
    public class XnaScrapProjectFactory: IVsProjectFactory
    {
        public const string XnaScrapProjectFactoryGuidString =
            "BBE28D35-C6AE-4371-951A-69E045C5C035";
        public static readonly Guid XnaScrapProjectFactoryGuid =
            new Guid(XnaScrapProjectFactoryGuidString);

        public XnaScrapProjectFactory()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        int IVsProjectFactory.CanCreateProject(string pszFilename, uint grfCreateFlags, out int pfCanCreate)
        {
            pfCanCreate = VSConstants.S_OK;
            return VSConstants.S_OK;
            //throw new NotImplementedException();
        }

        int IVsProjectFactory.Close()
        {
            return VSConstants.S_OK;
            //throw new NotImplementedException();
        }

        int IVsProjectFactory.CreateProject(string pszFilename, string pszLocation, string pszName, uint grfCreateFlags, ref Guid iidProject, out IntPtr ppvProject, out int pfCanceled)
        {
            //throw new NotImplementedException();
            pfCanceled = VSConstants.S_FALSE;
            try
            {
                ppvProject = Marshal.GetIUnknownForObject(new XnaScrapProject());
            }
            catch(Exception e)
            {
                pfCanceled = VSConstants.S_OK;
                ppvProject = IntPtr.Zero;
            }
            return VSConstants.S_OK;
        }

        int IVsProjectFactory.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            return VSConstants.S_OK;
            //throw new NotImplementedException();
        }
    }
}
