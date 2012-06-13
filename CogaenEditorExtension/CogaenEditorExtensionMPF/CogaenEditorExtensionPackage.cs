using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using CogaenDataItems.DataItems;
using CogaenEditorConnect.Communication;
using System.Threading;
using System.Windows.Threading;
using CogaenEditExtension;
using CogaenEditExtension.Communication;
using CogaenEditExporterManager;
using CogaenEditorControls.GUI_Elements;
using CogaenDataItems.Manager;
using CogaenEditorControls;
using System.Windows.Controls;
using System.Collections.Generic;
using CBeroEdit;
using CogaenDataItems.Manager.Interfacese;
using EnvDTE80;
using EnvDTE;
using System.IO;
using System.Reflection;

namespace CogaenEditExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
#if ADD_PROJECT_TYPE
    //[ProvideProjectFactory(typeof(XnaScrapProjectFactory)
    //, "CogaenEdit"
    //, "Python Project (*.pyproj);*.pyproj"
    //, "pyproj"
    //, "pyproj"
    //, @".\\NullProject"
    //, LanguageVsTemplate = "CogaenEdit")]
#endif
    [ProvideProjectFactory(typeof(CogaenEditProjectFactory), "CogaenEdit", "CogaenEdit Project Files (*.ceproj);*.ceproj", "ceproj", "ceproj", ".\\NullPath", LanguageVsTemplate = "CogaenEdit")]
    //[ProvideProjectItem(typeof(CogaenEditProjectFactory), "CogaenEdit", ".\\NullPath", 500)]
    //[ProvideProjectItem("{FAE04EC0-301F-11d3-BF4B-00C04F79EFBC}", "CogaenEdit", ".\\NullPath", 500)]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0Exp")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    //[SingleFileGeneratorSupportRegistrationAttribute(typeof(CogaenEditProjectFactory))]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    //[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(MyToolWindow))]
    [ProvideToolWindow(typeof(GameObjectMessageWindow))]
    [ProvideToolWindow(typeof(LiveEditor2DWindow))]
    [ProvideToolWindow(typeof(LiveEditor3DWindow))]
    [ProvideToolWindow(typeof(ServiceEditorWindow))]
    [ProvideEditorExtension(typeof(EditorFactory), ".ctl", 50,
              ProjectGuid = "{A2FE74E1-B743-11d0-AE1A-00A0C90FFFC3}",
              TemplateDir = "Templates",
              NameResourceID = 105,
              DefaultName = "CogaenEditExtension")]
    //[ProvideEditorExtensionAttribute(typeof(EditorFactory), ".ctl", 50)]
    //[ProvideProjectItem(typeof(XnaScrapProjectFactory), "Cogaen and Xna Scrap", @"..\..\Templates\ProjectItems", 500)]
    //[ProvideKeyBindingTable(GuidList.guidCogaenEditExtensionEditorFactoryString, 102)]
    [ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.Designer_string)]
    //[ProvideEditorLogicalView(typeof(EditorFactory), "{7651a703-06e5-11d1-8ebd-00a0c90f26ea}")]
    [Guid(GuidList.guidCogaenEditExtensionPkgString)]
    //[ComVisible(true)]
    //[CLSCompliant(false)]
    public sealed class CogaenEditExtensionPackage : Package, IVsInstalledProduct, IOleComponent, IConnectionLogOutput
    {
        #region member
        private static CogaenData m_Data = new CogaenData(null);

        public static CogaenData Data
        {
            get { return CogaenEditExtensionPackage.m_Data; }
        }

        private static CogaenEditExporterManager.CogaenEditExporterManager m_exporterManager = new CogaenEditExporterManager.CogaenEditExporterManager();

        public static CogaenEditExporterManager.CogaenEditExporterManager ExporterManager
        {
            get { return CogaenEditExtensionPackage.m_exporterManager; }
        }

        private Dictionary<Guid, IEditorGui> m_ServiceEdiors = new Dictionary<Guid, IEditorGui>();

        public Dictionary<Guid, IEditorGui> ServiceEdiors
        {
            get { return m_ServiceEdiors; }
        }

        private static Connection m_connection;

        public static Connection Connection
        {
            get { return m_connection; }
        }

        private uint m_connectionRegisterHandle;

        private static ConnectionWindow m_conWin = null;

        private static MessageHandler m_messageHandler;

        public static MessageHandler MessageHandler
        {
            get { return CogaenEditExtensionPackage.m_messageHandler; }
        }

        private static CogaenEditExtensionPackage m_mainInstance;

        public static CogaenEditExtensionPackage MainInstance
        {
            get { return CogaenEditExtensionPackage.m_mainInstance; }
        }

        private IVsOutputWindowPane m_connectionLogPane;
        #endregion
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public CogaenEditExtensionPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

            m_messageHandler = new MessageHandler(this, m_Data,Dispatcher.CurrentDispatcher);
            m_connection = new Connection(m_messageHandler);
            m_messageHandler.Connection = m_connection;

            Data.LiveGameObjects = new ObjectBuilder("LiveGameObjects", CogaenEditExtensionPackage.Data);

            m_mainInstance = this;

            DummyInit.Init();

        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowElementsToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(MyToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        
        private void ShowGameObjectMessageWindow(object sender, EventArgs e)
        {
            ShowGameObjectMessageWindowinternal(sender, e);
        }

        private GameObjectMessageControl ShowGameObjectMessageWindowinternal(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(GameObjectMessageWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            if (window.Content is GameObjectMessageControl)
            {
                return window.Content as GameObjectMessageControl;
            }
            else
                return null;
        }

        private void ShowLive2DWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(LiveEditor2DWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void RefreshLiveGameObjects(object sender)
        {
            CogaenEditExtensionPackage.MessageHandler.updateLiveGameobjectData();
        }

        private void LiveGameObjectProperty(LiveGameObject gameobject)
        {
            if (gameobject != null)
            {
                GameObjectMessageControl control = ShowGameObjectMessageWindowinternal(null, EventArgs.Empty);
                if (control != null)
                {
                    control.SelectGameObject(gameobject);
                }
            }
        }

        public void ShowServiceEditor(Guid serviceGuid, string serviceName)
        {
            IEditorGui m_editor;
            if (m_ServiceEdiors.TryGetValue(serviceGuid, out m_editor))
            {
                ToolWindowPane window = this.FindToolWindow(typeof(ServiceEditorWindow), 0, true);
                if ((null == window) || (null == window.Frame))
                {
                    throw new NotSupportedException(Resources.CanNotCreateWindow);
                }
                ServiceEditorWindow editorWindow = (ServiceEditorWindow)window;
                editorWindow.Reinit(this, m_editor, serviceName, serviceName);

                IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            }
        }

        private void praseForEditors()
        {
            praseForEditors("./");
            // TODO add Registry Key
            RegistryKey hCogaenEditKey = Registry.LocalMachine.OpenSubKey("CogaenEdit");
            if (hCogaenEditKey != null)
            {
                object hEditorDirectory = hCogaenEditKey.GetValue("EditorGuiDirectory");
                if (hEditorDirectory != null)
                {
                    praseForEditors(hEditorDirectory as String);
                }
                hCogaenEditKey.Close();
            }
        }

        private void praseForEditors(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (Path.GetExtension(file).CompareTo(".dll") == 0)
                {
                    try
                    {
                        loadEditor(file);
                    }
                    catch (Exception) { }
                }
            }

            // TEST
            //m_ServiceEdiors.Add(CBeroEdit.EditorGuids.CBeroGuidEditor, new RenderManagerEditor("CBero.Service.RenderManager", Connection));
            // end TEST
        }

        private void loadEditor(string file)
        {
            IEditorGui editor;
            Guid guid;
            if (TryCreateInstance(file, Path.GetFullPath(file), out editor, out guid))
            {
                m_ServiceEdiors.Add(guid, editor);
            }
        }

        private static bool TryCreateInstance(string dllName, string fullPath, out IEditorGui editorGui, out Guid guid)
        {
            editorGui = null;
            guid = Guid.Empty;
            try
            {
                Assembly Library = Assembly.LoadFrom(fullPath);
                // look for a IScriptExporter
                Type[] types = Library.GetTypes();
                foreach (Type type in types)
                {
                    if (type.GetInterface(typeof(IEditorGui).ToString()) != null)
                    {
                        IEditorGui editor = Library.CreateInstance(type.ToString()) as IEditorGui;
                        editorGui = editor;
                        // this should be retrieved via reflection
                        guid = editor.GetGuid();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                ConnectionLog.LogError(0, e.Message);
                return false;
            }

            return false;
        }

        private IVsOutputWindowPane CreateOutputPane(string title,
                            bool visible, bool clearWithSolution)
        {
            IVsOutputWindow output =
                this.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            IVsOutputWindowPane pane = null;
            

            if (output != null)
            {
                // Create a new pane.
                output.CreatePane(
                    ref GuidList.guidConnectionLogGuid,
                    title,
                    Convert.ToInt32(visible),
                    Convert.ToInt32(clearWithSolution));

                // Retrieve the new pane.
                output.GetPane(ref GuidList.guidConnectionLogGuid, out pane);
            }

            return pane;
        }

        public void OutputConnectionLogMessage(string message)
        {
            if (m_connectionLogPane != null)
            {
                m_connectionLogPane.OutputString(message);
                m_connectionLogPane.OutputString("\n\n");
            }
        }

        //private IVsErrorList GetErrorList()
        //{
        //    IVsErrorList errorList =
        //    this.GetService(typeof(SVsErrorList)) as IVsErrorList;

            

        //    return errorList;
        //}

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            //Create Editor Factory. Note that the base Package class will call Dispose on it.
            base.RegisterEditorFactory(new EditorFactory(this));

            base.RegisterProjectFactory(new CogaenEditProjectFactory(this));
#if ADD_PROJECT_TYPE
#endif

            // Load ScriptExporter
            m_exporterManager.parseForExporter("./");

            praseForEditors();

            // Create our log
            m_connectionLogPane = CreateOutputPane("CogaenEdit", true, true);
            ConnectionLog.RegisterOutput(this);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                // Connect
                CommandID menuCommandID = new CommandID(GuidList.guidCogaenEditExtensionCmdSet, (int)PkgCmdIDList.cmdidConnect);
                MenuCommand menuItemCnct = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItemCnct );
                //Disconnect
                CommandID menuCommandIDDisc = new CommandID(GuidList.guidCogaenEditExtensionCmdSet, (int)PkgCmdIDList.cmdidDisconnect);
                MenuCommand menuItemDisc = new MenuCommand(MenuItemCallback, menuCommandIDDisc);
                mcs.AddCommand(menuItemDisc);
                //Add ContentReference
                CommandID menuCommandIDContent = new CommandID(GuidList.guidCogaenEditExtensionCmdSet, (int)PkgCmdIDList.cmdidAddContentRef);
                MenuCommand menuItemContent = new MenuCommand(MenuItemCallback, menuCommandIDContent);
                mcs.AddCommand(menuItemContent);
                // Run Script
                CommandID menuCommandIDRunScript = new CommandID(GuidList.guidCogaenEditExtensionCmdSet, (int)PkgCmdIDList.cmdidRunScript);
                MenuCommand menuItemRunScript = new MenuCommand(MenuItemCallback, menuCommandIDRunScript);
                mcs.AddCommand(menuItemRunScript);
                // Create the command for the tool window
                CommandID elementsWndCommandID = new CommandID(GuidList.guidCogaenEditExtensionCmdSet, (int)PkgCmdIDList.cmdidCogaenEdit);
                MenuCommand menuElementsWin = new MenuCommand(ShowElementsToolWindow, elementsWndCommandID);
                mcs.AddCommand( menuElementsWin );
                // command for the gameobjectmessagewindow
                CommandID gameobjectMessageWndCommandID = new CommandID(GuidList.guidCogaenEditExtensionCmdSet, (int)PkgCmdIDList.cmdidMessageWindow);
                MenuCommand menugameobjectMessageWin = new MenuCommand(ShowGameObjectMessageWindow, gameobjectMessageWndCommandID);
                mcs.AddCommand(menugameobjectMessageWin);
                // command for the 2d live editor
                CommandID live2dWndCommandID = new CommandID(GuidList.guidCogaenEditExtensionCmdSet, (int)PkgCmdIDList.cmdidLive2DWindow);
                MenuCommand live2DWin = new MenuCommand(ShowLive2DWindow, live2dWndCommandID);
                mcs.AddCommand(live2DWin);
            }


            // initialize the 2d live editor eventhandling
            ToolWindowPane window = this.FindToolWindow(typeof(LiveEditor2DWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            if (window.Content is ObjectBuilderLiveControl)
            {
                ObjectBuilderLiveControl objectbuilderWnd = window.Content as ObjectBuilderLiveControl;
                objectbuilderWnd.RegisterForRefresh(RefreshLiveGameObjects);
                objectbuilderWnd.RegisterForProperties(LiveGameObjectProperty);
                objectbuilderWnd.GameObjectSelectionChanged += new ObjectBuilderLiveControl.GameObjectSelectedEventHandler(OnGameObjectSelectionChanged);
            }

            // make our connection available
            IRunningObjectTable rot = GetService(typeof(IRunningObjectTable)) as IRunningObjectTable;
            if (rot != null)
            {
                //rot.Register(0, m_connection, , out m_connectionRegisterHandle);
            }
        }

        void OnGameObjectSelectionChanged(object sender, System.Collections.Generic.IList<IScriptObject> selection)
        {
            if (selection == null)
            {
                LiveGameObjectProperty(null);
            }
            if (selection.Count == 1 && selection[0] is LiveGameObject)
            {
                LiveGameObjectProperty(selection[0] as LiveGameObject);
            }
            else
            {
                LiveGameObjectProperty(null);
            }
            if (sender is UserControl)
            {
                UserControl ctrl = sender as UserControl;
                ctrl.Focus();
            }
        }
        #endregion


        #region IVsInstalledProduct Members
        /// <summary>
        /// This method is called during Devenv /Setup to get the bitmap to
        /// display on the splash screen for this package.
        /// </summary>
        /// <param name="pIdBmp">The resource id corresponding to the bitmap to display on the splash screen</param>
        /// <returns>HRESULT, indicating success or failure</returns>
        public int IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 300;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// This method is called to get the icon that will be displayed in the
        /// Help About dialog when this package is selected.
        /// </summary>
        /// <param name="pIdIco">The resource id corresponding to the icon to display on the Help About dialog</param>
        /// <returns>HRESULT, indicating success or failure</returns>
        public int IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 400;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// This methods provides the product official name, it will be
        /// displayed in the help about dialog.
        /// </summary>
        /// <param name="pbstrName">Out parameter to which to assign the product name</param>
        /// <returns>HRESULT, indicating success or failure</returns>
        public int OfficialName(out string pbstrName)
        {
            pbstrName = "CogaenEdit Extension";
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This methods provides the product description, it will be
        /// displayed in the help about dialog.
        /// </summary>
        /// <param name="pbstrProductDetails">Out parameter to which to assign the description of the package</param>
        /// <returns>HRESULT, indicating success or failure</returns>
        public int ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = "No Details";
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This methods provides the product version, it will be
        /// displayed in the help about dialog.
        /// </summary>
        /// <param name="pbstrPID">Out parameter to which to assign the version number</param>
        /// <returns>HRESULT, indicating success or failure</returns>
        public int ProductID(out string pbstrPID)
        {
            pbstrPID = "1.0";
            return VSConstants.S_OK;
        }

        #endregion

        #region IOleComponent Members
        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FDoIdle(uint grfidlef)
        {
            //if (null != libraryManager)
            //{
            //    libraryManager.OnIdle();
            //}
            return 0;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void Terminate()
        {
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////
        // Dispose
        protected override void Dispose(bool disposing)
        {
            m_connection.disconnect();
            if (m_conWin != null)
                m_conWin.Close();
            m_conWin = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            if (sender is MenuCommand)
            {
                MenuCommand cmd = sender as MenuCommand;
                if (cmd.CommandID.ID == PkgCmdIDList.cmdidConnect)
                {
                    if (m_conWin == null)
                        m_conWin = new ConnectionWindow(MessageHandler);
                    if (!m_conWin.IsVisible)
                        m_conWin.Show();
                }
                else if (cmd.CommandID.ID == PkgCmdIDList.cmdidDisconnect)
                {

                }
                else if (cmd.CommandID.ID == PkgCmdIDList.cmdidAddContentRef)
                {
                    
                }
            }
            else
            {
                IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
                Guid clsid = Guid.Empty;
                int result;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                           0,
                           ref clsid,
                           "CogaenEditExtension",
                           string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
                           string.Empty,
                           0,
                           OLEMSGBUTTON.OLEMSGBUTTON_OK,
                           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                           OLEMSGICON.OLEMSGICON_INFO,
                           0,        // false
                           out result));
            }
        }

    }

    #region version
    public class Version
    {
        private int m_major = 1;
        private int m_minor = 0;
        private int m_rev = 0;

        public Version()
        {
        }

        public Version(int major, int minor, int rev)
        {
            m_major = major;
            m_minor = minor;
            m_rev = rev;
        }

        public void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(m_major);
            bw.Write(m_minor);
            bw.Write(m_rev);
        }

        public void deserialize(System.IO.BinaryReader br)
        {
            m_major = br.ReadInt32();
            m_minor = br.ReadInt32();
            m_rev = br.ReadInt32();
        }
    }
    #endregion
}
