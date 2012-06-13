using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System.Xml;
using CogaenDataItems.Exporter;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using CogaenEditExtension;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VSConstants = Microsoft.VisualStudio.VSConstants;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using System.Windows.Forms;
using Microsoft.VisualStudio.Project;
using System.Drawing;
using Microsoft.Windows.Design.Host;
using Microsoft.VisualStudio.Project.Automation;
using Microsoft.Build.Tasks;
using MSBuild = Microsoft.Build.Evaluation;

namespace CogaenEditExtension
{
    public class CogaenEditProject : ProjectNode
    {
        #region nested types
        public enum ImageName
        {
            OfflineWebApp = 0,
            WebReferencesFolder = 1,
            OpenReferenceFolder = 2,
            ReferenceFolder = 3,
            Reference = 4,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SDL")]
            SDLWebReference = 5,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DISCO")]
            DISCOWebReference = 6,
            Folder = 7,
            OpenFolder = 8,
            ExcludedFolder = 9,
            OpenExcludedFolder = 10,
            ExcludedFile = 11,
            DependentFile = 12,
            MissingFile = 13,
            WindowsForm = 14,
            WindowsUserControl = 15,
            WindowsComponent = 16,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "XML")]
            XMLSchema = 17,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "XML")]
            XMLFile = 18,
            WebForm = 19,
            WebService = 20,
            WebUserControl = 21,
            WebCustomUserControl = 22,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ASP")]
            ASPPage = 23,
            GlobalApplicationClass = 24,
            WebConfig = 25,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HTML")]
            HTMLPage = 26,
            StyleSheet = 27,
            ScriptFile = 28,
            TextFile = 29,
            SettingsFile = 30,
            Resources = 31,
            Bitmap = 32,
            Icon = 33,
            Image = 34,
            ImageMap = 35,
            XWorld = 36,
            Audio = 37,
            Video = 38,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CAB")]
            CAB = 39,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "JAR")]
            JAR = 40,
            DataEnvironment = 41,
            PreviewFile = 42,
            DanglingReference = 43,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "XSLT")]
            XSLTFile = 44,
            Cursor = 45,
            AppDesignerFolder = 46,
            Data = 47,
            Application = 48,
            DataSet = 49,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PFX")]
            PFX = 50,
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SNK")]
            SNK = 51,

            ImageLast = 51
        }
        #endregion

        #region member
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
            get
            {
                return VsMenus.IDM_VS_CTXT_PROJNODE;
            }
        }
        #endregion
        private Package package;
        internal static int ImageOffset;
        private static ImageList ceImageList;
        private VSLangProj.VSProject vsProject = null;
        private Microsoft.VisualStudio.Designer.Interfaces.IVSMDCodeDomProvider codeDomProvider;
        private DesignerContext designerContext;

        private Dictionary<uint, HierarchyNode> m_NodeDictionary = new Dictionary<uint, HierarchyNode>();

        public Dictionary<uint, HierarchyNode> NodeDictionary
        {
            get { return m_NodeDictionary; }
        }

        private uint m_itemIds = 0;

        private bool m_dirty = false;

        public bool Dirty
        {
            get { return m_dirty; }
            set { m_dirty = value; }
        }

        /// <summary>
        /// Python specific project images
        /// </summary>
        public static ImageList CogaenEditImageList
        {
            get
            {
                return ceImageList;
            }
            set
            {
                ceImageList = value;
            }
        }

        /// <summary>
        /// Retreive the CodeDOM provider
        /// </summary>
        protected internal Microsoft.VisualStudio.Designer.Interfaces.IVSMDCodeDomProvider CodeDomProvider
        {
            get
            {
                //if (codeDomProvider == null)
                    //codeDomProvider = new VSMDPythonProvider(this.VSProject);
                return codeDomProvider;
            }
        }
        protected internal Microsoft.Windows.Design.Host.DesignerContext DesignerContext
        {
            get
            {
                if (designerContext == null)
                {
                    designerContext = new DesignerContext();
                    //Set the RuntimeNameProvider so the XAML designer will call it when items are added to
                    //a design surface. Since the provider does not depend on an item context, we provide it at 
                    //the project level.
                    //designerContext.RuntimeNameProvider = new PythonRuntimeNameProvider();
                }
                return designerContext;
            }
        }

        /// <summary>
        /// Get the VSProject corresponding to this project
        /// </summary>
        protected internal VSLangProj.VSProject VSProject
        {
            get
            {
                if (vsProject == null)
                    vsProject = new OAVSProject(this);
                return vsProject;
            }
        }

        public override Guid ProjectGuid
        {
            get
            {
                return typeof(CogaenEditProjectFactory).GUID;
            }
        }

        public override string ProjectType
        {
            get
            {
                return "CogaenEditProject";
            }
        }


        //
        // 
        //
        //
        //
        //
        //
        //

        public override int ImageIndex
        {
            get
            {
                return (int)CogaenEditProject.ImageName.Application;
            }
        }

        public static ServiceProvider ServiceProvider { get; set; }

        private TaskProvider taskProvider;

        //private MSBuild.Project buildProject;

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
                baseUri = new Url(System.IO.Path.GetDirectoryName(FileName));
                return baseUri;
            }
        }

        private ImageHandler imageHandler;
        /// <summary>
        /// Gets an ImageHandler for the project node.
        /// </summary>
        public ImageHandler ImageHandler
        {
            get
            {
                if (null == imageHandler)
                {
                    imageHandler = new ImageHandler(typeof(CogaenEditProject).Assembly.GetManifestResourceStream("CogaenEditExtension.Resources.imagelis.bmp"));
                }
                return imageHandler;
            }
        }

        private bool m_exportFilters = true;

        public bool ExportFilters
        {
            get { return m_exportFilters; }
            set
            {
                m_exportFilters = value;
                Dirty = true;
                OnPropertyChanged("ExportFilters");
            }
        }

        private IScriptExporter m_exporter = null;

        public IScriptExporter Exporter
        {
            get { return m_exporter; }
            set
            {
                m_exporter = value;
                Dirty = true;
                OnPropertyChanged("Exporter");
            }
        }

        private bool canFileNodesHaveChilds = false;
        /// <summary>
        /// Gets or sets the ability of a project filenode to have child nodes (sub items).
        /// Example would be C#/VB forms having resx and designer files.
        /// </summary>
        protected internal bool CanFileNodesHaveChilds
        {
            get
            {
                return canFileNodesHaveChilds;
            }
            set
            {
                canFileNodesHaveChilds = value;
            }
        }


        private static Guid addComponentLastActiveTab = VSConstants.GUID_SolutionPage;

        private static uint addComponentDialogSizeX = 0;

        private static uint addComponentDialogSizeY = 0;

        #endregion

        public enum CogaenEditImageName
        {
            CeFile = 0,
            CeProject = 1,
        }

        #region overridden fields
        public override string Url
        {
            get
            {
                return this.GetMkDocument();
            }
        }
        #endregion

        #region CDtors
        static CogaenEditProject()
        {
            CogaenEditImageList = Utilities.GetImageList(typeof(CogaenEditProject).Assembly.GetManifestResourceStream("CogaenEditExtension.Resources.imagelis.bmp"));
        }
        
        public CogaenEditProject()
            : base()
        {
            ProjectMgr = this;
            Init();
        }

        public CogaenEditProject(Package pkg, IOleServiceProvider site, MSBuild.Project buildProject)
        {
            this.package = pkg;
            SetSite(site);
            this.BuildProject = buildProject;
            this.CanFileNodesHaveChilds = true;
            this.OleServiceProvider.AddService(typeof(VSLangProj.VSProject), new OleServiceProvider.ServiceCreatorCallback(this.CreateServices), false);
            this.SupportsProjectDesigner = true;

            //Store the number of images in ProjectNode so we know the offset of the python icons.
            ImageOffset = this.ImageHandler.ImageList.Images.Count;
            foreach (Image img in CogaenEditImageList.Images)
            {
                this.ImageHandler.AddImage(img);
            }

            InitializeCATIDs();
        }

        /// <summary>
        /// Provide mapping from our browse objects and automation objects to our CATIDs
        /// </summary>
        private void InitializeCATIDs()
        {
            //// The following properties classes are specific to python so we can use their GUIDs directly
            //this.AddCATIDMapping(typeof(PythonProjectNodeProperties), typeof(PythonProjectNodeProperties).GUID);
            //this.AddCATIDMapping(typeof(PythonFileNodeProperties), typeof(PythonFileNodeProperties).GUID);
            //this.AddCATIDMapping(typeof(OAIronPythonFileItem), typeof(OAIronPythonFileItem).GUID);
            //// The following are not specific to python and as such we need a separate GUID (we simply used guidgen.exe to create new guids)
            //this.AddCATIDMapping(typeof(FolderNodeProperties), new Guid("A3273B8E-FDF8-4ea8-901B-0D66889F645F"));
            //// This one we use the same as python file nodes since both refer to files
            //this.AddCATIDMapping(typeof(FileNodeProperties), typeof(PythonFileNodeProperties).GUID);
            //// Because our property page pass itself as the object to display in its grid, we need to make it have the same CATID
            //// as the browse object of the project node so that filtering is possible.
            //this.AddCATIDMapping(typeof(GeneralPropertyPage), typeof(PythonProjectNodeProperties).GUID);

            // We could also provide CATIDs for references and the references container node, if we wanted to.
        }
        #endregion

        #region methods

        private void Init()
        {
            //this.ID = VSConstants.VSITEMID_ROOT;
            //this.tracker = new TrackDocumentsHelper(this);
        }

        public void SetBuildProject(MSBuild.Project buildProject)
        {
            this.BuildProject = buildProject;
        }

        /// <summary>
        /// Creates the services exposed by this project.
        /// </summary>
        private object CreateServices(Type serviceType)
        {
            object service = null;
            if (typeof(SVSMDCodeDomProvider) == serviceType)
            {
                service = this.CodeDomProvider;
            }
            else if (typeof(System.CodeDom.Compiler.CodeDomProvider) == serviceType)
            {
                service = this.CodeDomProvider.CodeDomProvider;
            }
            else if (typeof(DesignerContext) == serviceType)
            {
                service = this.DesignerContext;
            }
            else if (typeof(VSLangProj.VSProject) == serviceType)
            {
                service = this.VSProject;
            }
            else if (typeof(EnvDTE.Project) == serviceType)
            {
                service = this.GetAutomationObject();
            }
            return service;
        }

        public virtual int AddNewFilter(HierarchyNode parent)
        {
            string new_name = "NewFilter";
            int marker = 1;
            string name = new_name + marker.ToString();
            while (!Utilities.IsNameUniqueInFirstLevelOfNode(this, name) && marker < 100)
            {
                ++marker;
                name = new_name + marker.ToString();
            }
            if (marker < 100)
            {
                CogaenEditFilter filter = new CogaenEditFilter(name, this.ProjectMgr, null, null);
                filter.Export = true;
                parent.AddChild(filter);

                IVsUIHierarchyWindow uiWindow = UIHierarchyUtilities.GetUIHierarchyWindow(this.ProjectMgr.Site, SolutionExplorer);
                // This happens in the context of adding a new folder.
                // Since we are already in solution explorer, it is extremely unlikely that we get a null return.
                // If we do, the newly created folder will not be selected, and we will not attempt the rename
                // command (since we are selecting the wrong item).                        
                if (uiWindow != null)
                {
                    // we need to get into label edit mode now...
                    // so first select the new guy...
                    int result = uiWindow.ExpandItem(this.ProjectMgr, filter.ID, EXPANDFLAGS.EXPF_SelectItem);
                    ErrorHandler.ThrowOnFailure(result);
                    // them post the rename command to the shell. Folder verification and creation will
                    // happen in the setlabel code...
                    IVsUIShell shell = this.ProjectMgr.Site.GetService(typeof(SVsUIShell)) as IVsUIShell;

                    Debug.Assert(shell != null, "Could not get the ui shell from the project");
                    if (shell == null)
                    {
                        return VSConstants.E_FAIL;
                    }

                    object dummy = null;
                    Guid cmdGroup = VsMenus.guidStandardCommandSet97;
                    ErrorHandler.ThrowOnFailure(shell.PostExecCommand(ref cmdGroup, (uint)VsCommands.Rename, 0, ref dummy));
                }

                return VSConstants.S_OK;
            }
            else
            {
                return VSConstants.E_FAIL;
            }
        }

        public virtual int AddNewScript(HierarchyNode parent)
        {
            string new_name = "NewScript";
            int marker = 1;
            string name = new_name + marker.ToString() + ".ctl";
            while (!Utilities.IsNameUniqueInFirstLevelOfNode(this, name) && marker < 100)
            {
                ++marker;
                name = new_name + marker.ToString() + ".ctl";
            }
            if (marker < 100)
            {
                System.IO.File.Create(ProjectMgr.ProjectFolder + "\\" + name);
                CogaenEditFile file = new CogaenEditFile(name, this.ProjectMgr, null);
                parent.AddChild(file);
                IVsUIHierarchyWindow uiWindow = UIHierarchyUtilities.GetUIHierarchyWindow(this.ProjectMgr.Site, SolutionExplorer);
                // This happens in the context of adding a new folder.
                // Since we are already in solution explorer, it is extremely unlikely that we get a null return.
                // If we do, the newly created folder will not be selected, and we will not attempt the rename
                // command (since we are selecting the wrong item).                        
                if (uiWindow != null)
                {
                    // we need to get into label edit mode now...
                    // so first select the new guy...
                    int result = uiWindow.ExpandItem(this.ProjectMgr, file.ID, EXPANDFLAGS.EXPF_SelectItem);
                    ErrorHandler.ThrowOnFailure(result);
                    // them post the rename command to the shell. Folder verification and creation will
                    // happen in the setlabel code...
                    IVsUIShell shell = this.ProjectMgr.Site.GetService(typeof(SVsUIShell)) as IVsUIShell;

                    Debug.Assert(shell != null, "Could not get the ui shell from the project");
                    if (shell == null)
                    {
                        return VSConstants.E_FAIL;
                    }

                    object dummy = null;
                    Guid cmdGroup = VsMenus.guidStandardCommandSet97;
                    ErrorHandler.ThrowOnFailure(shell.PostExecCommand(ref cmdGroup, (uint)VsCommands.Rename, 0, ref dummy));
                }
                return VSConstants.S_OK;
            }
            else
                return VSConstants.E_FAIL;
        }


        /// <summary>
        /// Returns the reference container node.
        /// </summary>
        /// <returns></returns>
        public IReferenceContainer GetContentContainer()
        {
            return this.FindChild(XnaContentReferenceContainerNode.ReferencesNodeVirtualName) as IReferenceContainer;
        }
        /// <summary>
        /// Override this method if you want to modify the behavior of the Add Reference dialog
        /// By example you could change which pages are visible and which is visible by default.
        /// </summary>
        /// <returns></returns>
        public virtual int AddContentReference()
        {
            CCITracing.TraceCall();

            IVsComponentSelectorDlg4 componentDialog;
            string strBrowseLocations = Path.GetDirectoryName(this.BaseURI.Uri.LocalPath);
            var tabInitList = new List<VSCOMPONENTSELECTORTABINIT>()
            {
                //new VSCOMPONENTSELECTORTABINIT {
                //    guidTab = VSConstants.GUID_COMPlusPage,
                //    varTabInitInfo = GetComponentPickerDirectories(),
                //},
                new VSCOMPONENTSELECTORTABINIT {
                    guidTab = VSConstants.GUID_COMClassicPage,
                },
                new VSCOMPONENTSELECTORTABINIT {
                    // Tell the Add Reference dialog to call hierarchies GetProperty with the following
                    // propID to enablefiltering out ourself from the Project to Project reference
                    //varTabInitInfo = (int)__VSHPROPID.VSHPROPID_ShowProjInSolutionPage,
                    guidTab = VSConstants.GUID_SolutionPage,
                },
                // Add the Browse for file page            
                new VSCOMPONENTSELECTORTABINIT {
                    varTabInitInfo = 0,
                    guidTab = VSConstants.GUID_BrowseFilePage,
                },
                //new VSCOMPONENTSELECTORTABINIT {
                //    guidTab = GUID_MruPage,
                //},
            };
            tabInitList.ForEach(tab => tab.dwSize = (uint)Marshal.SizeOf(typeof(VSCOMPONENTSELECTORTABINIT)));

            componentDialog = this.GetService(typeof(IVsComponentSelectorDlg)) as IVsComponentSelectorDlg4;
            try
            {
                // call the container to open the add reference dialog.
                if (componentDialog != null)
                {
                    // Let the project know not to show itself in the Add Project Reference Dialog page
                    this.ShowProjectInSolutionPage = false;
                    // call the container to open the add reference dialog.
                    string browseFilters = "Component Files (*.exe;*.dll)\0*.exe;*.dll\0";
                    ErrorHandler.ThrowOnFailure(componentDialog.ComponentSelectorDlg5(
                        (System.UInt32)(__VSCOMPSELFLAGS.VSCOMSEL_MultiSelectMode | __VSCOMPSELFLAGS.VSCOMSEL_IgnoreMachineName),
                        (IVsComponentUser)this,
                        0,
                        null,
                        SR.GetString(SR.AddReferenceDialogTitle, CultureInfo.CurrentUICulture),   // Title
                        "VS.AddReference",                          // Help topic
                        addComponentDialogSizeX,
                        addComponentDialogSizeY,
                        (uint)tabInitList.Count,
                        tabInitList.ToArray(),
                        ref addComponentLastActiveTab,
                        browseFilters,
                        ref strBrowseLocations,
                        this.TargetFrameworkMoniker.FullName));
                }
            }
            catch (COMException e)
            {
                Trace.WriteLine("Exception : " + e.Message);
                return e.ErrorCode;
            }
            finally
            {
                // Let the project know it can show itself in the Add Project Reference Dialog page
                this.ShowProjectInSolutionPage = true;
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Add Components to the Project.
        /// Used by the environment to add components specified by the user in the Component Selector dialog 
        /// to the specified project
        /// </summary>
        /// <param name="dwAddCompOperation">The component operation to be performed.</param>
        /// <param name="cComponents">Number of components to be added</param>
        /// <param name="rgpcsdComponents">array of component selector data</param>
        /// <param name="hwndDialog">Handle to the component picker dialog</param>
        /// <param name="pResult">Result to be returned to the caller</param>
        public override int AddComponent(VSADDCOMPOPERATION dwAddCompOperation, uint cComponents, IntPtr[] rgpcsdComponents, IntPtr hwndDialog, VSADDCOMPRESULT[] pResult)
        {
            if (rgpcsdComponents == null || pResult == null)
            {
                return VSConstants.E_FAIL;
            }

            //initalize the out parameter
            pResult[0] = VSADDCOMPRESULT.ADDCOMPRESULT_Success;
            for (int cCount = 0; cCount < cComponents; cCount++)
            {
                VSCOMPONENTSELECTORDATA selectorData = new VSCOMPONENTSELECTORDATA();
                IntPtr ptr = rgpcsdComponents[cCount];
                selectorData = (VSCOMPONENTSELECTORDATA)Marshal.PtrToStructure(ptr, typeof(VSCOMPONENTSELECTORDATA));
                IReferenceContainer references = null;
                if (selectorData.bstrProjRef.EndsWith(".contentproj"))
                    references = GetContentContainer();
                else
                    references = GetReferenceContainer();
                if (null == references)
                {
                    // This project does not support references or the reference container was not created.
                    // In both cases this operation is not supported.
                    return VSConstants.E_NOTIMPL;
                }
                if (null == references.AddReferenceFromSelectorData(selectorData))
                {
                    //Skip further proccessing since a reference has to be added
                    pResult[0] = VSADDCOMPRESULT.ADDCOMPRESULT_Failure;
                    return VSConstants.S_OK;
                }
            }
            return VSConstants.S_OK;
            //return base.AddComponent(dwAddCompOperation, cComponents, rgpcsdComponents, hwndDialog, pResult);
        }

        #region loading and saving
        public override void Load(string filename, string location, string name, uint flags, ref Guid iidProject, out int canceled)
        {
            // first load the MSBuild Project
            base.Load(filename, location, name, flags, ref iidProject, out canceled);
        }

        #region (de)serialisation
        #region save
        /// <summary>
        /// Save the Project file as xml
        /// </summary>
        /// <param name="filename"></param>
        public void save(String filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            #region basic
            XmlElement projectNode = xmlDoc.CreateElement("Project");
            xmlDoc.AppendChild(projectNode);
            XmlAttribute projectName = xmlDoc.CreateAttribute("Name");
            projectName.Value = Caption;//m_root.Name;
            projectNode.Attributes.Append(projectName);

            //XmlAttribute exportPathNode = xmlDoc.CreateAttribute("ExportPath");
            //exportPathNode.Value = ExportDirectory;
            //projectNode.Attributes.Append(exportPathNode);

            //XmlAttribute dirPathNode = xmlDoc.CreateAttribute("Path");
            //dirPathNode.Value = ProjectDirectory;
            //projectNode.Attributes.Append(dirPathNode);

            XmlAttribute exportFilterNode = xmlDoc.CreateAttribute("ExportFilter");
            exportFilterNode.Value = m_exportFilters.ToString();
            projectNode.Attributes.Append(exportFilterNode);

            //XmlAttribute configFileNode = xmlDoc.CreateAttribute("ConfigFile");
            //configFileNode.Value = m_configFile.ToString();
            //projectNode.Attributes.Append(configFileNode);

            if (m_exporter != null)
            {
                XmlAttribute scriptType = xmlDoc.CreateAttribute("ScriptType");
                scriptType.Value = m_exporter.Extension;
                projectNode.Attributes.Append(scriptType);
            }

            #endregion
            #region files
            XmlElement files = xmlDoc.CreateElement("Files");
            HierarchyNode node = ProjectMgr.FirstChild;
            while (node != null)
            {
                saveProjectElement(xmlDoc, files, node);
                node = node.NextSibling;
            }
            projectNode.AppendChild(files);
            #endregion

            #region settings

            #endregion

            xmlDoc.Save(filename);

            Dirty = false;
        }

        public void save()
        {
            save(Caption + ".cep");
        }

        private void saveProjectElement(XmlDocument xmlDoc, XmlElement elementNode, HierarchyNode element)
        {
            if (element is CogaenEditFile)
            {
                CogaenEditFile template = element as CogaenEditFile;
                saveProjectTemplate(xmlDoc, elementNode, template);
            }
            else if (element is CogaenEditFilter)
            {
                CogaenEditFilter filter = element as CogaenEditFilter;
                saveProjectFilter(xmlDoc, elementNode, filter);
            }
        }

        private void saveProjectFilter(XmlDocument xmlDoc, XmlElement elementNode, CogaenEditFilter filter)
        {
            XmlElement filterNode = xmlDoc.CreateElement("Filter");
            XmlAttribute filterName = xmlDoc.CreateAttribute("Name");
            filterName.Value = filter.Caption;
            filterNode.Attributes.Append(filterName);
            XmlAttribute filterExport = xmlDoc.CreateAttribute("Export");
            filterExport.Value = filter.Export.ToString();
            filterNode.Attributes.Append(filterExport);

            HierarchyNode node = filter.FirstChild;
            while (node != null)
            {
                saveProjectElement(xmlDoc, filterNode, node);
                node = node.NextSibling;
            }
            elementNode.AppendChild(filterNode);
        }

        private void saveProjectTemplate(XmlDocument xmlDoc, XmlElement elementNode, CogaenEditFile template)
        {
            XmlElement file = xmlDoc.CreateElement("File");
            XmlAttribute filename = xmlDoc.CreateAttribute("Filename");
            filename.Value = template.Caption;
            file.Attributes.Append(filename);

            //XmlAttribute scriptname = xmlDoc.CreateAttribute("Script");
            //scriptname.Value = template.Script.Name;
            //file.Attributes.Append(scriptname);

            elementNode.AppendChild(file);
        }
        #endregion
        #region load
        //public void load(string filename, bool copy_content, string temp_path)
        //{
        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.Load(filename);
        //    String name = "unnamed";
        //    String exportDir = ".";
        //    String projectDir = ".";
        //    String configFile = "";
        //    bool bExportFilter = true;
        //    IScriptExporter exporter = null;

        //    //ProjectFilter root = new ProjectFilter("root");

        //    foreach (XmlNode node in xmlDoc.ChildNodes)
        //    {
        //        if (node is XmlElement)
        //        {
        //            XmlElement element = node as XmlElement;
        //            if (element.Name == "Project")
        //            {
        //                foreach (XmlAttribute attrib in element.Attributes)
        //                {
        //                    if (attrib.Name == "Name")
        //                    {
        //                        name = attrib.Value;
        //                        filename = name;
        //                    }
        //                    else if (attrib.Name == "ExportPath")
        //                    {
        //                        exportDir = attrib.Value;
        //                    }
        //                    else if (attrib.Name == "Path")
        //                    {
        //                        projectDir = attrib.Value;
        //                    }
        //                    else if (attrib.Name == "ExportFilter")
        //                    {
        //                        bool.TryParse(attrib.Value, out bExportFilter);
        //                    }
        //                    else if (attrib.Name == "ConfigFile")
        //                    {
        //                        configFile = attrib.Value;
        //                    }
        //                    else if (attrib.Name == "ScriptType")
        //                    {
        //                        //App app = App.Current as App;
        //                        //exporter = app.getExporter(attrib.Value);
        //                    }
        //                }
        //            }
        //            XmlNode fileNode = Helper.XmlTools.getNodeByName(element.ChildNodes,"Files");
        //            if (fileNode != null && fileNode is XmlElement)
        //            {
        //                loadFiles(fileNode as XmlElement, this, copy_content, temp_path);
        //            }
        //        }
        //    }


            //Project newProject = new Project(name);
            //newProject.Root = root;
            ////newProject.m_root = root;
            ////newProject.m_elements = newProject.m_root.Entries;
            //newProject.ExportDirectory = exportDir;
            //newProject.ProjectDirectory = projectDir;
            //newProject.ExportFilters = bExportFilter;
            //newProject.ConfigFile = configFile;
            //newProject.Exporter = exporter;

            //Directory.SetCurrentDirectory(newProject.ProjectDirectory);

            //List<String> files = new List<String>();
            //bool valid = validate(files);
        //}

        //private static void loadSub(XmlElement node, CogaenEditFilter root)
        //{
        //    foreach (XmlElement element in node.ChildNodes)
        //    {
        //        if (element.Name == "Files")
        //        {
        //            loadFiles(element, root);
        //        }
        //    }
        //}

        //private void loadFiles(XmlElement filesNode, HierarchyNode root, bool copy_content, string temp_path)
        //{
        //    foreach (XmlElement element in filesNode.ChildNodes)
        //    {
        //        if (element.Name == "Filter")
        //        {
        //            String name = "";
        //            bool export = true;

        //            foreach (XmlAttribute attrib in element.Attributes)
        //            {
        //                if (attrib.Name == "Name")
        //                {
        //                    name = attrib.Value;
        //                }
        //                else if (attrib.Name == "Export")
        //                {
        //                    bool.TryParse(attrib.Value, out export);
        //                }
        //            }
        //            CogaenEditFilter filter = new CogaenEditFilter(name, this, root);
        //            filter.Export = export;
        //            root.AddChild(filter);
        //            loadFiles(element, filter, copy_content, temp_path);
        //        }
        //        else if (element.Name == "File")
        //        {
        //            string filename = "";
        //            string scriptname = "";
        //            foreach (XmlAttribute attrib in element.Attributes)
        //            {
        //                if (attrib.Name == "Filename")
        //                {
        //                    filename = attrib.Value;
        //                }
        //                else if (attrib.Name == "Script")
        //                {
        //                    scriptname = attrib.Value;
        //                }
        //            }
        //            if (copy_content)
        //            {
        //                System.IO.File.Copy(System.IO.Path.Combine(temp_path, filename), System.IO.Path.Combine(this.ProjectFolder, filename));
        //            }
        //            CogaenEditFile template = new CogaenEditFile(filename, this, null);
        //            //template.Script = new ProjectScriptFile(scriptname, template);
        //            root.AddChild(template);
        //        }
        //    }
        //}
        #endregion
        #endregion
        #endregion
        #region copy paste

        #endregion
        #endregion

        #region overriden methods

        /// <summary>
        /// Determines whether a file is a code file.
        /// </summary>
        /// <param name="fileName">Name of the file to be evaluated</param>
        /// <returns>false by default for any fileName</returns>
        public override bool IsCodeFile(string fileName)
        {
            // just check the extension
            return (System.IO.Path.GetExtension(fileName).Equals(".ctl"));
            //HierarchyNode child = FindChild(fileName);
            //return child is CogaenEditFile;
        }

        /// <summary>
        /// Create a file node based on an msbuild item.
        /// </summary>
        /// <param name="item">msbuild item</param>
        /// <returns>FileNode added</returns>
        public override FileNode CreateFileNode(ProjectElement item)
        {
            return new CogaenEditFile("",this, item);
        }

        /// <summary>
        /// Create a file node based on a string.
        /// </summary>
        /// <param name="file">filename of the new filenode</param>
        /// <returns>File node added</returns>
        public override FileNode CreateFileNode(string file)
        {
            ProjectElement item = this.AddFileToMsBuild(file);
            return this.CreateFileNode(item);
        }

        /// <summary>
        /// To support virtual folders, override this method to return your own folder nodes
        /// </summary>
        /// <param name="path">Path to store for this folder</param>
        /// <param name="element">Element corresponding to the folder</param>
        /// <returns>A FolderNode that can then be added to the hierarchy</returns>
        protected internal override FolderNode CreateFolderNode(string path, ProjectElement element)
        {
            FolderNode folderNode = new CogaenEditFilter("",this, path, element);
            return folderNode;
        }

        protected override ConfigProvider CreateConfigProvider()
        {
            return new CogaenEditConfigProvider(this);
            //return base.CreateConfigProvider();
        }

        protected override NodeProperties CreatePropertiesObject()
        {
            return new CEProjectNodeProperties(this);
        }

        protected override void Reload()
        {
            base.Reload();

            ProcessContentReferences();
        }

        protected virtual XnaContentReferenceContainerNode CreateXnaContentReferenceContainer()
        {
            return new XnaContentReferenceContainerNode(this);
        }

        public IReferenceContainer GetXnaContentReferenceContainer()
        {
            return this.FindChild(XnaContentReferenceContainerNode.ReferencesNodeVirtualName) as IReferenceContainer;
        }

        protected internal void ProcessContentReferences()
        {
            IReferenceContainer container = GetXnaContentReferenceContainer();
            if (null == container)
            {
                // Process References
                XnaContentReferenceContainerNode referencesFolder = CreateXnaContentReferenceContainer();
                if (null == referencesFolder)
                {
                    // This project type does not support references or there is a problem
                    // creating the reference container node.
                    // In both cases there is no point to try to process references, so exit.
                    return;
                }
                this.AddChild(referencesFolder);
                container = referencesFolder;
            }

            // Load the referernces.
            container.LoadReferencesFromBuildProject(BuildProject);
        }

        #region commands
        protected override int ExecCommandOnNode(Guid cmdGroup, uint cmd, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (cmdGroup == GuidList.guidCogaenEditExtensionCmdSet)
            {
                if (cmd == PkgCmdIDList.cmdidAddContentRef)
                {
                    return AddContentReference();
                }
            }
            return base.ExecCommandOnNode(cmdGroup, cmd, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Handles command status on the project node. If a command cannot be handled then the base should be called.
        /// </summary>
        /// <param name="cmdGroup">A unique identifier of the command group. The pguidCmdGroup parameter can be NULL to specify the standard group.</param>
        /// <param name="cmd">The command to query status for.</param>
        /// <param name="pCmdText">Pointer to an OLECMDTEXT structure in which to return the name and/or status information of a single command. Can be NULL to indicate that the caller does not require this information.</param>
        /// <param name="result">An out parameter specifying the QueryStatusResult of the command.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        protected override int QueryStatusOnNode(Guid cmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
        {
            if (cmdGroup == GuidList.guidCogaenEditExtensionCmdSet)
            {
                if (cmd == PkgCmdIDList.cmdidAddContentRef)
                {
                    result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                    return VSConstants.S_OK;
                }
            }
            return base.QueryStatusOnNode(cmdGroup, cmd, pCmdText, ref result);
        }
        #endregion
        #endregion

        #region events and callbacks
        public virtual void OnOpenItem(string fullPathToFile)
        {

        }
        #endregion

        #region Property Changed
        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
