using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VSConstants = Microsoft.VisualStudio.VSConstants;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using CogaenDataItems.Manager;
using CogaenEditExtension;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Project.Automation;
using Microsoft.Windows.Design.Host;

namespace CogaenEditExtension
{
    public class CogaenEditFile :
        FileNode
        , IPersistFileFormat
    {
        #region fields
        private static Guid m_classId = new Guid("{AC1C07AD-40AB-4C0A-8C6A-8A6BFFBED161}");

        private OAVSProjectItem vsProjectItem;
        //private SelectionElementValueChangedListener selectionChangedListener;
        //private OAIronPythonFileItem automationObject;
        private DesignerContext designerContext;

        private IObjectBuilder m_ObjectBuilder = null;

        public IObjectBuilder ObjectBuilder
        {
            get { return m_ObjectBuilder; }
            set { m_ObjectBuilder = value; }
        }

        private bool m_dirty;

        public bool Dirty
        {
            get { return m_dirty; }
            set { m_dirty = value; }
        }
        #endregion

        #region properties
        /// <summary>
        /// Returns bool indicating whether this node is of subtype "Form"
        /// </summary>
        public bool IsFormSubType
        {
            get
            {
                string result = this.ItemNode.GetMetadata(ProjectFileConstants.SubType);
                if (!String.IsNullOrEmpty(result) && string.Compare(result, ProjectFileAttributeValue.Form, true, CultureInfo.InvariantCulture) == 0)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// Returns the SubType of an Iron Python FileNode. It is 
        /// </summary>
        public string SubType
        {
            get
            {
                return this.ItemNode.GetMetadata(ProjectFileConstants.SubType);
            }
            set
            {
                this.ItemNode.SetMetadata(ProjectFileConstants.SubType, value);
            }
        }

        protected internal VSLangProj.VSProjectItem VSProjectItem
        {
            get
            {
                if (null == vsProjectItem)
                {
                    vsProjectItem = new OAVSProjectItem(this);
                }
                return vsProjectItem;
            }
        }

        protected internal Microsoft.Windows.Design.Host.DesignerContext DesignerContext
        {
            get
            {
                if (designerContext == null)
                {
                    designerContext = new DesignerContext();
                    //Set the EventBindingProvider for this XAML file so the designer will call it
                    //when event handlers need to be generated
                    //designerContext.EventBindingProvider = new PythonEventBindingProvider(this.Parent.FindChild(this.Url.Replace(".xaml", ".py")) as PythonFileNode);
                }
                return designerContext;
            }
        }
        #endregion

        #region cdtors
        internal CogaenEditFile(string name, ProjectNode proj, ProjectElement e)
            : base(proj,e)
        {
            //selectionChangedListener = new SelectionElementValueChangedListener(new ServiceProvider((IOleServiceProvider)root.GetService(typeof(IOleServiceProvider))), root);
            //selectionChangedListener.Init();
        }
        #endregion

        #region overridden properties

        //internal override object Object
        //{
        //    get
        //    {
        //        return this.VSProjectItem;
        //    }
        //}

        #endregion

        #region overridden methods
        protected override NodeProperties CreatePropertiesObject()
        {
            CEFileNodeProperties properties = new CEFileNodeProperties(this);
            //properties.OnCustomToolChanged += new EventHandler<HierarchyNodeEventArgs>(OnCustomToolChanged);
            //properties.OnCustomToolNameSpaceChanged += new EventHandler<HierarchyNodeEventArgs>(OnCustomToolNameSpaceChanged);
            return properties;
        }

        public override int Close()
        {
            //if (selectionChangedListener != null)
            //    selectionChangedListener.Dispose();
            return base.Close();
        }

        ///// <summary>
        ///// Returs an Iron Python FileNode specific object implmenting DTE.ProjectItem
        ///// </summary>
        ///// <returns></returns>
        //public override object GetAutomationObject()
        //{
        //    if (null == automationObject)
        //    {
        //        automationObject = new OAIronPythonFileItem(this.ProjectMgr.GetAutomationObject() as OAProject, this);
        //    }
        //    return automationObject;
        //}

        public override int ImageIndex
        {
            get
            {
                if (IsFormSubType)
                {
                    return (int)ProjectNode.ImageName.WindowsForm;
                }
                if (this.FileName.ToLower().EndsWith(".py"))
                {
                    return CogaenEditProject.ImageOffset + (int)CogaenEditProject.CogaenEditImageName.CeFile;
                }
                return base.ImageIndex;
            }
        }

        ///// <summary>
        ///// Open a file depending on the SubType property associated with the file item in the project file
        ///// </summary>
        //protected override void DoDefaultAction()
        //{
        //    FileDocumentManager manager = this.GetDocumentManager() as FileDocumentManager;
        //    Debug.Assert(manager != null, "Could not get the FileDocumentManager");

        //    Guid viewGuid = (IsFormSubType ? VSConstants.LOGVIEWID_Designer : VSConstants.LOGVIEWID_Code);
        //    IVsWindowFrame frame;
        //    manager.Open(false, false, viewGuid, out frame, WindowFrameShowAction.Show);
        //}

        protected override int ExecCommandOnNode(Guid guidCmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            Debug.Assert(this.ProjectMgr != null, "The PythonFileNode has no project manager");

            if (this.ProjectMgr == null)
            {
                throw new InvalidOperationException();
            }

            if (guidCmdGroup == CogaenEditMenus.guidIronPythonProjectCmdSet)
            {
                if (cmd == (uint)CogaenEditMenus.SetAsMain.ID)
                {
                    // Set the MainFile project property to the Filename of this Node
                    ((CogaenEditProject)this.ProjectMgr).SetProjectProperty("MainFile", this.GetRelativePath());
                    return VSConstants.S_OK;
                }
            }
            return base.ExecCommandOnNode(guidCmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Handles the menuitems
        /// </summary>		
        protected override int QueryStatusOnNode(Guid guidCmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
        {
            if (guidCmdGroup == Microsoft.VisualStudio.Shell.VsMenus.guidStandardCommandSet97)
            {
                switch ((VsCommands)cmd)
                {
                    case VsCommands.AddNewItem:
                    case VsCommands.AddExistingItem:
                    case VsCommands.ViewCode:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;
                    case VsCommands.ViewForm:
                        if (IsFormSubType)
                            result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;
                }
            }

            else if (guidCmdGroup == CogaenEditMenus.guidIronPythonProjectCmdSet)
            {
                if (cmd == (uint)CogaenEditMenus.SetAsMain.ID)
                {
                    result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                    return VSConstants.S_OK;
                }
            }
            return base.QueryStatusOnNode(guidCmdGroup, cmd, pCmdText, ref result);
        }

        #endregion

        #region methods
        public int OpenItem(bool newFile, bool openWith, uint editorFlags, ref Guid logicalView, out IVsWindowFrame frame, WindowFrameShowAction windowFrameAction)
        {
            int result = VSConstants.S_OK;
            frame = null;
            IVsRunningDocumentTable rdt = (this.ProjectMgr as CogaenEditProject).Site.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (rdt != null)
            {
                IVsHierarchy hier;
                uint itemid;
                IntPtr ptrDocData = IntPtr.Zero;
                uint docCookie;

                try
                {
                    result = rdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_EditLock, Caption, out hier, out itemid, out ptrDocData, out docCookie);
                    if (result != VSConstants.S_OK)
                    {
                        Guid editorType = Guid.Empty;
                        result = OpenItem(newFile, openWith, editorFlags, ref editorType, null, ref logicalView, ptrDocData, out frame, windowFrameAction);
                    }
                    else
                    {
                        ObjectBuilder = Marshal.GetObjectForIUnknown(ptrDocData) as ObjectBuilder;
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    if (ptrDocData != IntPtr.Zero)
                    {
                        Marshal.Release(ptrDocData);
                    }
                }
            }
            else
            {
                result = VSConstants.E_FAIL;
            }
            return result;
        }

        private int OpenItem(bool newFile, bool openWith, uint editorFlags, ref Guid editorType, string physicalView, ref Guid logicalView, IntPtr docDataExisting, out IVsWindowFrame frame, WindowFrameShowAction windowFrameAction)
        {
            frame = null;

            if (this.ProjectMgr == null || this.ProjectMgr.IsClosed)
            {
                return VSConstants.E_FAIL;
            }

            int result = VSConstants.S_OK;
            uint[] ppItemId = new uint[1];
            ppItemId[0] = this.ID;
            int open;
            IVsUIHierarchy project;
            string fullPath = GetMkDocument();

            // check if the file exists
            if (!System.IO.File.Exists(fullPath))
            {
                // TODO Inform clients that we have an invalid item (wrong icon)
                //// Inform clients that we have an invalid item (wrong icon)
                //this.Node.OnInvalidateItems(this.Node.Parent);

                return VSConstants.S_FALSE;
            }

            IVsUIShellOpenDocument sod = (this.ProjectMgr as CogaenEditProject).Site.GetService(typeof(IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
            if (sod != null)
            {
                try
                {
                    // try to get hold of the open file
                    sod.IsDocumentOpen(this.ProjectMgr
                        , this.ID
                        , GetMkDocument()
                        , ref logicalView /* LOGVIEWID_Code to show xml behind */
                        , (uint)__VSIDOFLAGS.IDO_ActivateIfOpen
                        , out project
                        , ppItemId
                        , out frame
                        , out open);

                    // file is not open
                    if (open == 0)
                    {
                        // TODO load doc data
                        Load(fullPath, 0, 0);
                        docDataExisting = Marshal.GetIUnknownForObject(ObjectBuilder);

                        if (openWith)
                        {
                            result = sod.OpenStandardEditor((uint)__VSOSEFLAGS.OSE_UseOpenWithDialog//(uint)__VSOSEFLAGS.OSE_ChooseBestStdEditor
                            , GetMkDocument()
                            , ref logicalView /* VSConstants.LOGVIEWID.Code_guid for xml behind*/
                            , Caption
                            , this.ProjectMgr
                            , this.ID
                                // TODO pass docData!!!!
                            , docDataExisting
                            , this.ProjectMgr.Site as Microsoft.VisualStudio.OLE.Interop.IServiceProvider
                            , out frame);
                        }
                        else
                        {
                            __VSOSEFLAGS openFlags = 0;
                            if (newFile)
                            {
                                openFlags |= __VSOSEFLAGS.OSE_OpenAsNewFile;
                            }

                            openFlags |= __VSOSEFLAGS.OSE_ChooseBestStdEditor;

                            if (editorType != Guid.Empty)
                            {
                                result = sod.OpenSpecificEditor(editorFlags
                                    , fullPath
                                    , ref editorType
                                    , physicalView
                                    , ref logicalView
                                    , Caption
                                    , this.ProjectMgr
                                    , this.ID
                                    , docDataExisting
                                    , this.ProjectMgr.Site as Microsoft.VisualStudio.OLE.Interop.IServiceProvider
                                    , out frame);
                            }
                            else
                            {

                                result = sod.OpenStandardEditor((uint)openFlags
                                , GetMkDocument()
                                , ref logicalView /* VSConstants.LOGVIEWID.Code_guid for xml behind*/
                                , Caption
                                , this.ProjectMgr
                                , this.ID
                                , docDataExisting
                                , this.ProjectMgr.Site as Microsoft.VisualStudio.OLE.Interop.IServiceProvider
                                , out frame);
                            }
                        }
                        if (result != VSConstants.S_OK && result != VSConstants.S_FALSE && result != VSConstants.OLE_E_PROMPTSAVECANCELLED)
                        {
                            ErrorHandler.ThrowOnFailure(result);
                        }
                        // eventually show window
                        if (frame != null)
                        {
                            object var;

                            if (newFile)
                            {
                                ErrorHandler.ThrowOnFailure(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out var));
                                IVsPersistDocData persistDocData = (IVsPersistDocData)var;
                                ErrorHandler.ThrowOnFailure(persistDocData.SetUntitledDocPath(fullPath));
                            }

                            var = null;
                            ErrorHandler.ThrowOnFailure(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocCookie, out var));
                            this.DocCookie = (uint)(int)var;

                            if (windowFrameAction == WindowFrameShowAction.Show)
                            {
                                ErrorHandler.ThrowOnFailure(frame.Show());
                            }
                            else if (windowFrameAction == WindowFrameShowAction.ShowNoActivate)
                            {
                                ErrorHandler.ThrowOnFailure(frame.ShowNoActivate());
                            }
                            else if (windowFrameAction == WindowFrameShowAction.Hide)
                            {
                                ErrorHandler.ThrowOnFailure(frame.Hide());
                            }
                        }
                    }
                    // file is already open
                    else
                    {
                        ObjectBuilder = Marshal.GetObjectForIUnknown(docDataExisting) as ObjectBuilder;
                        Marshal.Release(docDataExisting);

                        if (frame != null)
                        {
                            if (windowFrameAction == WindowFrameShowAction.Show)
                            {
                                ErrorHandler.ThrowOnFailure(frame.Show());
                            }
                            else if (windowFrameAction == WindowFrameShowAction.ShowNoActivate)
                            {
                                ErrorHandler.ThrowOnFailure(frame.ShowNoActivate());
                            }
                            else if (windowFrameAction == WindowFrameShowAction.Hide)
                            {
                                ErrorHandler.ThrowOnFailure(frame.Hide());
                            }
                        }
                    }
                }
                catch (COMException e)
                {
                    Trace.WriteLine("Exception e:" + e.Message);
                    result = e.ErrorCode;
                    CloseWindowFrame(ref frame);
                }
            }
            return result;
        }

        protected static void CloseWindowFrame(ref IVsWindowFrame windowFrame)
        {
            if (windowFrame != null)
            {
                try
                {
                    ErrorHandler.ThrowOnFailure(windowFrame.CloseFrame(0));
                }
                finally
                {
                    windowFrame = null;
                }
            }
        }

        public string GetRelativePath()
        {
            string relativePath = Path.GetFileName(this.ItemNode.GetMetadata(ProjectFileConstants.Include));
            HierarchyNode parent = this.Parent;
            while (parent != null && !(parent is ProjectNode))
            {
                relativePath = Path.Combine(parent.Caption, relativePath);
                parent = parent.Parent;
            }
            return relativePath;
        }

        internal OleServiceProvider.ServiceCreatorCallback ServiceCreator
        {
            get { return new OleServiceProvider.ServiceCreatorCallback(this.CreateServices); }
        }

        private object CreateServices(Type serviceType)
        {
            object service = null;
            if (typeof(EnvDTE.ProjectItem) == serviceType)
            {
                service = GetAutomationObject();
            }
            else if (typeof(DesignerContext) == serviceType)
            {
                service = this.DesignerContext;
            }
            return service;
        }
        #endregion

        #region IPersistFileFormat

        public int GetClassID(out Guid clsid)
        {
            clsid = m_classId;
            return VSConstants.S_OK;
        }


        public virtual int GetCurFile(out string name, out uint formatIndex)
        {
            name = this.Caption;
            formatIndex = 0;
            return VSConstants.S_OK;
        }

        public virtual int GetFormatList(out string formatlist)
        {
            formatlist = String.Empty;
            return VSConstants.S_OK;
        }

        public virtual int InitNew(uint formatIndex)
        {
            return VSConstants.S_OK;
        }

        public virtual int IsDirty(out int isDirty)
        {
            Dirty = ObjectBuilder.Dirty;
            isDirty = ObjectBuilder.Dirty? 1 : 0;
            //isDirty = IsFlavorDirty();

            return VSConstants.S_OK;
        }

        public virtual int Load(string fileName, uint mode, int readOnly)
        {
            //this.load(fileName, false, null);
            if (ObjectBuilder == null)
                ObjectBuilder = new ObjectBuilder(CogaenEditExtensionPackage.Data);
            try
            {
                ObjectBuilder.load(fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error while loading");
                return VSConstants.E_FAIL;
            }
            return VSConstants.S_OK;
        }

        public virtual int Save(string fileToBeSaved, int remember, uint formatIndex)
        {
            try
            {
                //save(fileToBeSaved);
                ObjectBuilder.serializeToXml(fileToBeSaved);
            }
            catch (Exception e)
            {
                return VSConstants.S_FALSE;
            }
            return VSConstants.S_OK;
        }


        public virtual int SaveCompleted(string filename)
        {
            // TODO: turn file watcher back on.
            return VSConstants.S_OK;
        }
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

        /// <summary>
        /// Default action for a file (double click or enter key)
        /// </summary>
        protected override void DoDefaultAction()
        {
            Guid logicalView = VSConstants.LOGVIEWID_Designer;
            IVsWindowFrame frame;
            OpenItem(false, false, 0, ref logicalView, out frame, WindowFrameShowAction.Show);
            //FileDocumentManager manager = this.GetDocumentManager() as FileDocumentManager;
            //Debug.Assert(manager != null, "Could not get the FileDocumentManager");

            //Guid viewGuid = (IsFormSubType ? VSConstants.LOGVIEWID_Designer : VSConstants.LOGVIEWID_Code);
            //IVsWindowFrame frame;
            //manager.Open(false, false, viewGuid, out frame, WindowFrameShowAction.Show);
        }
        #endregion
    }
}
