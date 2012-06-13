using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CogaenEditor2.Commands;
using System.Collections.ObjectModel;

namespace CogaenEditor2.GUI.Menu
{
    public class MenuManager
    {
        #region commands
        static private RoutedUICommand m_Connect = new RoutedUICommand("Connect", "Connect", typeof(MenuManager));

        public static RoutedUICommand Connect
        {
            get { return MenuManager.m_Connect; }
            set { MenuManager.m_Connect = value; }
        }

        static private RoutedUICommand m_Disconnect = new RoutedUICommand("Disconnect", "Disconnect", typeof(MenuManager));

        public static RoutedUICommand Disconnect
        {
            get { return MenuManager.m_Disconnect; }
            set { MenuManager.m_Disconnect = value; }
        }

        static private RoutedUICommand m_SaveAll = new RoutedUICommand("Save All", "SaveAll", typeof(MenuManager));

        public static RoutedUICommand SaveAll
        {
            get { return MenuManager.m_SaveAll; }
            set { MenuManager.m_SaveAll = value; }
        }

        static private RoutedUICommand m_Rename = new RoutedUICommand("Rename", "Rename", typeof(MenuManager));

        public static RoutedUICommand Rename
        {
            get { return MenuManager.m_Rename; }
            set { MenuManager.m_Rename = value; }
        }

        static private RoutedUICommand m_deleteCommand = new RoutedUICommand("Delete", "Delete", typeof(MenuManager));

        public static RoutedUICommand Delete
        {
            get { return m_deleteCommand; }
            set { m_deleteCommand = value; }
        }

        static private RoutedUICommand m_exportCommand = new RoutedUICommand("Export", "Export", typeof(MenuManager));

        public static RoutedUICommand Export
        {
            get { return m_exportCommand; }
            set { m_exportCommand = value; }
        }


        static private RoutedUICommand m_sendMessageCommand = new RoutedUICommand("Send Message", "SendMessage", typeof(MenuManager));

        public static RoutedUICommand SendMessageCommand
        {
            get { return m_sendMessageCommand; }
            set { m_sendMessageCommand = value; }
        }

        static private RoutedUICommand m_resetOffsetCommand = new RoutedUICommand("Reset Offset", "ResetOffset", typeof(MenuManager));

        public static RoutedUICommand ResetOffsetCommand
        {
            get { return m_resetOffsetCommand; }
            set { m_resetOffsetCommand = value; }
        }

        static private RoutedUICommand m_resetZoomCommand = new RoutedUICommand("Reset Zoom", "ResetZoom", typeof(MenuManager));

        public static RoutedUICommand ResetZoomCommand
        {
            get { return m_resetZoomCommand; }
            set { m_resetZoomCommand = value; }
        }

        static private RoutedUICommand m_sortGameObjectsCommand = new RoutedUICommand("Sort Gameobjects", "SortGameobjects", typeof(MenuManager));

        public static RoutedUICommand SortGameObjectsCommand
        {
            get { return m_sortGameObjectsCommand; }
            set { m_sortGameObjectsCommand = value; }
        }

        static private RoutedUICommand m_sortGameObjectsTopDownCommand = new RoutedUICommand("Sort Gameobjects Top-Down", "SortGameobjectsTopDown", typeof(MenuManager));

        public static RoutedUICommand SortGameObjectsTopDownCommand
        {
            get { return m_sortGameObjectsTopDownCommand; }
            set { m_sortGameObjectsTopDownCommand = value; }
        }
        
        static private RoutedUICommand m_importComponents = new RoutedUICommand("Import Componenents", "ImportComponenents", typeof(MenuManager));

        public static RoutedUICommand ImportComponents
        {
            get { return m_importComponents; }
            set { m_importComponents = value; }
        }

        static private RoutedUICommand m_convertLive2Script = new RoutedUICommand("Convert to Script", "ConvertLive2Script", typeof(MenuManager));

        public static RoutedUICommand ConvertLive2Script
        {
            get { return m_convertLive2Script; }
            set { m_convertLive2Script = value; }
        }

        #region config
        static private RoutedUICommand m_showConfig = new RoutedUICommand("Show Config", "ShowConfig", typeof(MenuManager));

        public static RoutedUICommand ShowConfig
        {
            get { return m_showConfig; }
            set { m_showConfig = value; }
        }

        static private RoutedUICommand m_saveConfig = new RoutedUICommand("Save Config", "SaveConfig", typeof(MenuManager));

        public static RoutedUICommand SaveConfig
        {
            get { return m_saveConfig; }
            set { m_saveConfig = value; }
        }

        static private RoutedUICommand m_loadConfig = new RoutedUICommand("Load Config", "LoadConfig", typeof(MenuManager));

        public static RoutedUICommand LoadConfig
        {
            get { return m_loadConfig; }
            set { m_loadConfig = value; }
        }

        static private RoutedUICommand m_resetConfig = new RoutedUICommand("Reset Config", "ResetConfig", typeof(MenuManager));

        public static RoutedUICommand ResetConfig
        {
            get { return m_resetConfig; }
            set { m_resetConfig = value; }
        }
        #endregion

        #region project
        static private RoutedUICommand m_newProjectCommand = new RoutedUICommand("New Project", "NewProject", typeof(MenuManager));

        public static RoutedUICommand NewProjectCommand
        {
            get { return m_newProjectCommand; }
            set { m_newProjectCommand = value; }
        }

        static private RoutedUICommand m_loadProjectCommand = new RoutedUICommand("Load Project", "LoadProject", typeof(MenuManager));

        public static RoutedUICommand LoadProjectCommand
        {
            get { return m_loadProjectCommand; }
            set { m_loadProjectCommand = value; }
        }

        static private RoutedUICommand m_closeProjectCommand = new RoutedUICommand("Close Project", "CloseProject", typeof(MenuManager));

        public static RoutedUICommand CloseProjectCommand
        {
            get { return m_closeProjectCommand; }
            set { m_closeProjectCommand = value; }
        }

        static private RoutedUICommand m_newProjectFilterCommand = new RoutedUICommand("New Project", "New Project", typeof(MenuManager));

        public static RoutedUICommand NewProjectFilterCommand
        {
            get { return m_newProjectFilterCommand; }
            set { m_newProjectFilterCommand = value; }
        }

        static private RoutedUICommand m_deleteProjectFilterCommand = new RoutedUICommand("Delete Filter", "DeleteProjectFilter", typeof(MenuManager));

        public static RoutedUICommand DeleteProjectFilterCommand
        {
            get { return m_deleteProjectFilterCommand; }
            set { m_deleteProjectFilterCommand = value; }
        }
        #endregion

        #region templates
        static private RoutedUICommand m_runTemplate = new RoutedUICommand("Run Template", "RunTemplate", typeof(MenuManager));

        public static RoutedUICommand RunTemplate
        {
            get { return m_runTemplate; }
            set { m_runTemplate = value; }
        }

        static private RoutedUICommand m_renameTemplate = new RoutedUICommand("Rename Template", "RenameTemplate", typeof(MenuManager));

        public static RoutedUICommand RenameTemplate
        {
            get { return m_renameTemplate; }
            set { m_renameTemplate = value; }
        }

        static private RoutedUICommand m_saveTemplate = new RoutedUICommand("Save Template", "SaveTemplate", typeof(MenuManager));

        public static RoutedUICommand SaveTemplate
        {
            get { return m_saveTemplate; }
            set { m_saveTemplate = value; }
        }

        static private RoutedUICommand m_loadTemplate = new RoutedUICommand("Load Template", "LoadTemplate", typeof(MenuManager));

        public static RoutedUICommand LoadTemplate
        {
            get { return m_loadTemplate; }
            set { m_loadTemplate = value; }
        }
        #endregion

        #region parameter
        static private RoutedUICommand m_addParameterCommand = new RoutedUICommand("Add Parameter", "AddParameter", typeof(MenuManager));

        public static RoutedUICommand AddParameterCommand
        {
            get { return m_addParameterCommand; }
            set { m_addParameterCommand = value; }
        }

        static private RoutedUICommand m_cloneParameterCommand = new RoutedUICommand("Clone Parameter", "CloneParameter", typeof(MenuManager));

        public static RoutedUICommand CloneParameterCommand
        {
            get { return m_cloneParameterCommand; }
            set { m_cloneParameterCommand = value; }
        }

        static private RoutedUICommand m_removeParameterCommand = new RoutedUICommand("Remove Parameter", "RemoveParameter", typeof(MenuManager));

        public static RoutedUICommand RemoveParameterCommand
        {
            get { return m_removeParameterCommand; }
            set { m_removeParameterCommand = value; }
        }
        #endregion

        #region ObjectBuilder
        static private RoutedUICommand m_removeElement = new RoutedUICommand("Remove Element", "RemoveElement", typeof(MenuManager));

        public static RoutedUICommand RemoveElementCommand
        {
            get { return m_removeElement; }
            set { m_removeElement = value; }
        }

        static private RoutedUICommand m_renameGameObject = new RoutedUICommand("Rename GameObject", "RenameGameObject", typeof(MenuManager));

        public static RoutedUICommand RenameGameObject
        {
            get { return m_renameGameObject; }
            set { m_renameGameObject = value; }
        }

        static private RoutedUICommand m_deleteGameObject = new RoutedUICommand("Delete GameObject", "DeleteGameObject", typeof(MenuManager));

        public static RoutedUICommand DeleteGameObject
        {
            get { return m_deleteGameObject; }
            set { m_deleteGameObject = value; }
        }
        #endregion
        #endregion

        public static MenuManager getDefaultMenu()
        {
            MenuManager menu = new MenuManager();

            CommandGroup homeGroup = menu.addCommandGroup("Home");
            CommandGroup templateGroup = menu.addCommandGroup("GameObject Templates");
            CommandGroup editorGroup = menu.addCommandGroup("3D Editor");
            // homeGroup
            // connect
            CommandList connectCommands = homeGroup.addCommandList("Connection");
            connectCommands.addCommand("Connect", Connect, "/CogaenEditor2;component/icons/Connect.png", null);
            connectCommands.addCommand("Disconnect", Disconnect, "/CogaenEditor2;component/icons/Disconnect.png", null);

            //// project
            //CommandList projectCommands = homeGroup.addCommandList("Project");
            //projectCommands.addCommand(m_newProjectCommand);
            //projectCommands.addCommand(m_saveProjectCommand);
            //projectCommands.addCommand(m_loadProjectCommand);
            //projectCommands.addCommand(m_closeProjectCommand);

            // data
            CommandList dataCommands = homeGroup.addCommandList("Data");
            //dataCommands.addCommand(m_listComponentsCommand);
            dataCommands.addCommand("Import Components", ImportComponents, "", "/CogaenEditor2;component/icons/ImportIcon.png");
            //dataCommands.addCommand(m_listGameObjectsCommand);
            
            // Templates
            CommandList templateCommands = homeGroup.addCommandList("Templates");
            ////templateCommands.addCommand(m_newGameObjectCommand);
            //templateCommands.addCommand(m_newTemplateCommand);
            //templateCommands.addCommand(m_runTemplateCommand);
            templateCommands.addCommand("Run Template", RunTemplate, "/CogaenEditor2;component/icons/new.png", "");
            templateCommands.addCommand("New Template", ApplicationCommands.New, "", "/CogaenEditor2;component/icons/new.png");

            //// templategroup
            ////
            //CommandList templateCommands2 = templateGroup.addCommandList("Templates");
            //templateCommands2.addCommand(m_newTemplateCommand);
            //templateCommands2.addCommand(m_saveTemplateCommand);
            //templateCommands2.addCommand(m_loadTemplateCommand);
            //templateCommands2.addCommand(m_renameTemplateCommand);
            //templateCommands2.addCommand(m_runTemplateCommand);
            //templateCommands2.addCommand(m_deleteTemplateCommand);
            //templateCommands2.addCommand(m_exportTemplateCommand);
            //templateCommands2.addCommand(m_convertTemplateToMacroCommand);
            //templateCommands2.addCommand(m_convertMacroToTemplateCommand);

            //CommandList gameObjects = templateGroup.addCommandList("GameObjects");
            //gameObjects.addCommand(m_newGameObjectCommand);
            //gameObjects.addCommand(m_deleteGameObjectCommand);

            // 3D Editor View
            //
            CommandList editor3d = editorGroup.addCommandList("Basic");
            //editor3d.addCommand(m_showEditorCommand);

            return menu;
        }

        private ObservableCollection<CommandGroup> m_commandGroups = new ObservableCollection<CommandGroup>();

        public ObservableCollection<CommandGroup> CommandGroups
        {
            get { return m_commandGroups; }
            set { m_commandGroups = value; }
        }

        public MenuManager()
        {

        }

        CommandGroup addCommandGroup(String name)
        {
            CommandGroup newGroup = new CommandGroup(name);
            m_commandGroups.Add(newGroup);
            return newGroup;
        }
    }
}
