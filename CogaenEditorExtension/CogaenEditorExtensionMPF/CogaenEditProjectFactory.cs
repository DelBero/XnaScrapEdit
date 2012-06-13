using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.Build;
using Microsoft.VisualStudio.Project;
using Microsoft.Build.Tasks;
using MSBuild = Microsoft.Build.Evaluation;
using MSBuildExecution = Microsoft.Build.Execution;

namespace CogaenEditExtension
{
    [Guid(XnaScrapProjectFactoryGuidString)]
    public class CogaenEditProjectFactory: IVsProjectFactory
    {
        public const string XnaScrapProjectFactoryGuidString =
            "BBE28D35-C6AE-4371-951A-69E045C5C035";
        public static readonly Guid XnaScrapProjectFactoryGuid =
            new Guid(XnaScrapProjectFactoryGuidString);

        #region member
        private Microsoft.VisualStudio.Shell.Package package;

        protected Microsoft.VisualStudio.Shell.Package Package
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// The msbuild engine that we are going to use.
        /// </summary>
        private MSBuild.ProjectCollection buildEngine;

        /// <summary>
        /// The msbuild project for the project file.
        /// </summary>
        private MSBuild.Project buildProject;

        private System.IServiceProvider site;

        protected System.IServiceProvider Site
        {
            get
            {
                return this.site;
            }
        }
        #endregion

        public CogaenEditProjectFactory(Microsoft.VisualStudio.Shell.Package package)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
            this.package = package;
            this.site = package;

            // Please be aware that this methods needs that ServiceProvider is valid, thus the ordering of calls in the ctor matters.
            this.buildEngine = Utilities.InitializeMsBuildEngine(this.buildEngine, this.site);
        }

        public int CanCreateProject(string pszFilename, uint grfCreateFlags, out int pfCanCreate)
        {
            pfCanCreate = 1;
            return VSConstants.S_OK;
            //throw new NotImplementedException();
        }

        public int Close()
        {
            return VSConstants.S_OK;
            //throw new NotImplementedException();
        }

        public int CreateProject(string pszFilename, string pszLocation, string pszName, uint grfCreateFlags, ref Guid iidProject, out IntPtr ppvProject, out int pfCanceled)
        {
            //throw new NotImplementedException();
            pfCanceled = 0;
            try
            {
                this.buildProject = Utilities.ReinitializeMsBuildProject(this.buildEngine, pszFilename, this.buildProject);

                CogaenEditProject project = new CogaenEditProject(this.Package, (IOleServiceProvider)((IServiceProvider)this.Package).GetService(typeof(IOleServiceProvider)), this.buildProject);
                project.BuildEngine = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection;
                
                project.Load(pszFilename, pszLocation, pszName, grfCreateFlags, ref iidProject, out pfCanceled);
                ppvProject = Marshal.GetIUnknownForObject(project);
                
            }
            catch(Exception e)
            {
                pfCanceled = 1;
                ppvProject = IntPtr.Zero;
            }
            return VSConstants.S_OK;
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            return VSConstants.S_OK;
            //throw new NotImplementedException();
        }
    }
}
