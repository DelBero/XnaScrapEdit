using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System.IO;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VSConstants = Microsoft.VisualStudio.VSConstants;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using Microsoft.VisualStudio.Shell.Interop;
using System.Globalization;
using Microsoft.VisualStudio.Project;

namespace CogaenEditExtension
{
    public class CogaenEditFilter: FolderNode
    {

        #region memeber
        #region commands
        public override bool CanExecuteCommand
        {
            get
            {
                return true;
            }
        }

        public override int MenuCommandId
        {
            get { return VsMenus.IDM_VS_CTXT_FOLDERNODE; }
        }
        #endregion

        private bool m_export;

        public bool Export
        {
            get { return m_export; }
            set { m_export = value; }
        }

        private Microsoft.VisualStudio.Shell.Url baseUri;
        /// <summary>
        /// Gets the Base Uniform Resource Identifier (URI).
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "URI")]
        public Microsoft.VisualStudio.Shell.Url BaseURI
        {
            get
            {
                //if (baseUri == null && this.buildProject != null)
                //{
                //    string path = System.IO.Path.GetDirectoryName(this.buildProject.FullPath);
                //    // Uri/Url behave differently when you have trailing slash and when you dont
                //    if (!path.EndsWith("\\", StringComparison.Ordinal) && !path.EndsWith("/", StringComparison.Ordinal))
                //        path += "\\";
                //    baseUri = new Url(path);
                //}

                //Debug.Assert(baseUri != null, "Base URL should not be null. Did you call BaseURI before loading the project?");
                baseUri = new Url(System.IO.Path.GetDirectoryName(Caption));
                return baseUri;
            }
        }


        /// <summary>
        /// overwrites of the generic hierarchyitem.
        /// </summary>
        [System.ComponentModel.BrowsableAttribute(false)]
        public override string Caption
        {
            get
            {
                // Use LinkedIntoProjectAt property if available
                string caption = this.ItemNode.GetMetadata(ProjectFileConstants.LinkedIntoProjectAt);
                if (caption == null || caption.Length == 0)
                {
                    // Otherwise use filename
                    caption = this.ItemNode.GetMetadata(ProjectFileConstants.Include);
                    caption = Path.GetFileName(caption);
                }
                return caption;
            }
        }

        public override Guid ItemTypeGuid
        {
            get { return VSConstants.GUID_ItemType_VirtualFolder; }
        }
        #endregion

        #region overridden fields
        public override string Url
        {
            get
            {
                return this.GetMkDocument();
            }
        }
        #endregion

        public CogaenEditFilter(string name, ProjectNode proj, string rleativePath, ProjectElement element)
            : base(proj, rleativePath, element)
        {
            m_export = false;
        }

        #region methods
        #endregion

        #region overriden methods

        /// <summary>
        /// Gets the moniker for the project node. That is the full path of the project file.
        /// </summary>
        /// <returns>The moniker for the project file.</returns>
        public override string GetMkDocument()
        {
            //Debug.Assert(!String.IsNullOrEmpty(this.m_filename));
            //Debug.Assert(this.BaseURI != null && !String.IsNullOrEmpty(this.BaseURI.AbsoluteUrl));
            return Path.Combine(this.ProjectMgr.ProjectFolder, Caption);
        }

        public override object GetIconHandle(bool open)
        {
            return this.ProjectMgr.ImageHandler.GetIconHandle(open ? (int)CogaenEditProject.ImageName.OpenFolder : (int)CogaenEditProject.ImageName.Folder);
        }

        public override int SetEditLabel(string label)
        {
            if (String.Compare(Path.GetFileName(this.Url.TrimEnd('\\')), label, StringComparison.Ordinal) == 0)
            {
                // Label matches current Name
                return VSConstants.S_OK;
            }

            string newPath = Path.Combine(new DirectoryInfo(this.Url).Parent.FullName, label);

            // Verify that No Directory/file already exists with the new name among current children
            for (HierarchyNode n = Parent.FirstChild; n != null; n = n.NextSibling)
            {
                if (n != this && String.Compare(n.Caption, label, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return ShowFileOrFolderAlreadExistsErrorMessage(newPath);
                }
            }

            try
            {
                this.OnPropertyChanged(this, (int)__VSHPROPID.VSHPROPID_Caption, 0);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Couldn't rename Folder {0}", e.Message));
            }
            return VSConstants.S_OK;
        }

        #region commands
        /// <summary>
        /// Handles the menuitems
        /// </summary>		
        protected override int QueryStatusOnNode(Guid guidCmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
        {
            if (guidCmdGroup == Microsoft.VisualStudio.Shell.VsMenus.guidStandardCommandSet97)
            {
                switch ((VsCommands)cmd)
                {
                    //case VsCommands.AddNewItem:
                    case VsCommands.AddExistingItem:
                    case VsCommands.NewFolder:
                    case VsCommands.AddClass:
                    case VsCommands.Rename:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;
                }
            }

            //else if (guidCmdGroup == PythonMenus.guidIronPythonProjectCmdSet)
            //{
            //    if (cmd == (uint)PythonMenus.SetAsMain.ID)
            //    {
            //        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
            //        return VSConstants.S_OK;
            //    }
            //}
            return base.QueryStatusOnNode(guidCmdGroup, cmd, pCmdText, ref result);
        }

        /// <summary>
        /// Handles command execution.
        /// </summary>
        /// <param name="cmdGroup">Unique identifier of the command group</param>
        /// <param name="cmd">The command to be executed.</param>
        /// <param name="nCmdexecopt">Values describe how the object should execute the command.</param>
        /// <param name="pvaIn">Pointer to a VARIANTARG structure containing input arguments. Can be NULL</param>
        /// <param name="pvaOut">VARIANTARG structure to receive command output. Can be NULL.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cmdexecopt")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "n")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "pva")]
        protected override int ExecCommandOnNode(Guid cmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {

            if (cmdGroup == VsMenus.guidStandardCommandSet97)
            {
                HierarchyNode nodeToAddTo = this.GetDragTargetHandlerNode();
                switch ((VsCommands)cmd)
                {
                    case VsCommands.AddNewItem:
                    case VsCommands.AddExistingItem:
                        return VSConstants.S_OK;
                    case VsCommands.AddClass:
                        if (this.ProjectMgr is CogaenEditProject)
                        {
                            return (this.ProjectMgr as CogaenEditProject).AddNewScript(this);
                        }
                        else
                            return VSConstants.S_FALSE;
                    case VsCommands.NewFolder:
                        if (this.ProjectMgr is CogaenEditProject)
                        {
                            return (this.ProjectMgr as CogaenEditProject).AddNewFilter(this);
                        }
                        return VSConstants.S_FALSE;
                }
            }
            return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
        }
        #endregion
        #endregion

        #region helper
        /// <summary>
        /// Show error message if not in automation mode, otherwise throw exception
        /// </summary>
        /// <param name="newPath">path of file or folder already existing on disk</param>
        /// <returns>S_OK</returns>
        private int ShowFileOrFolderAlreadExistsErrorMessage(string newPath)
        {
            //A file or folder with the name '{0}' already exists on disk at this location. Please choose another name.
            //If this file or folder does not appear in the Solution Explorer, then it is not currently part of your project. To view files which exist on disk, but are not in the project, select Show All Files from the Project menu.
            string errorMessage = ("A Folder with that name already exists");
            if (!Utilities.IsInAutomationFunction(this.ProjectMgr.Site))
            {
                string title = null;
                OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                VsShellUtilities.ShowMessageBox(this.ProjectMgr.Site, title, errorMessage, icon, buttons, defaultButton);
                return VSConstants.S_OK;
            }
            else
            {
                throw new InvalidOperationException(errorMessage);
            }
        }
        #endregion
    }
}
