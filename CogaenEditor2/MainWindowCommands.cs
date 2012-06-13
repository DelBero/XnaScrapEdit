using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls.Ribbon;
using CogaenEditor2.GUI.Windows;
using CogaenDataItems.DataItems;
using CogaenEditor2.Manager;
using CogaenEditor2.Helper;
using CogaenDataItems.Manager;

namespace CogaenEditor2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        #region default command binding
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (((RoutedCommand)e.Command).Name == "Copy")
            {
                //app.copy(sender, e);
            }
            else if (((RoutedCommand)e.Command).Name == "Paste")
            {
                //app.paste(sender, e);
            }
            else if (((RoutedCommand)e.Command).Name == "Delete")
            {
                //app.delete(sender, e);
            }
            else if (((RoutedCommand)e.Command).Name == "Properties")
            {
                //app.props(sender, e);
            }
            else if (((RoutedCommand)e.Command).Name == "New")
            {
                //app.newCommand(sender, e);
                app.newTemplate();
            }
            else if (((RoutedCommand)e.Command).Name == "Open")
            {
                //app.open(sender, e);
            }
            else if (((RoutedCommand)e.Command).Name == "Save")
            {
                app.saveTemplate(true);
            }
            else if (((RoutedCommand)e.Command).Name == "SaveAll")
            {
                app.saveCurrentProject();
            }
            else if (((RoutedCommand)e.Command).Name == "Refresh")
            {

            }
            else
            {
                e.Command.Execute(e.Parameter);
            }
        }
        #endregion

        private void CommandBinding_CanExecuteNew(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_ExecutedNew(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)Application.Current;
            app.newTemplate();
        }

        private void CommandBinding_CanExecuteConnect(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = (App)Application.Current;
            e.CanExecute = app.MessageHandler.Connection.CanConnect;
        }

        private void CommandBinding_ExecutedConnect(object sender, ExecutedRoutedEventArgs e)
        {
            ConnectionWindow conWnd = new ConnectionWindow();
            conWnd.Show();
        }

        private void CommandBinding_CanExecuteDisconnect(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = (App)Application.Current;
            e.CanExecute = !app.MessageHandler.Connection.CanConnect;
        }

        private void CommandBinding_ExecutedDisconnect(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        private void Rename_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Rename_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        #region objectbuilder
        private void Props_CanExecute_ObjectBuilder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Props_Executed_ObjectBuilder(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter is Element)
            {
                Element stateMachine = e.Parameter as Element;
                if (stateMachine.Semantic == Element.ElementSemantic.STATEMACHINE)
                {
                    Parameter transitions = null;
                    foreach (Parameter param in stateMachine.Parameters)
                    {
                        if (param.Semantic == ParameterSemantic.TRANSITIONS)
                        {
                            transitions = param;
                            break;
                        }
                    }
                    if (transitions != null)
                    {
                        StateMachine sm = new StateMachine(stateMachine.Name, transitions, stateMachine.ParentGameObject.Elements);
                        app.StateMachineWin.DataContext = sm;
                        app.StateMachineWin.Show();
                    }
                }
            }
        }

        private void RemoveParameter_CanExecute_ObjectBuilder(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is Parameter)
            {
                e.CanExecute = true;
            }
        }

        private void RemoveParameter_Executed_ObjectBuilder(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter is Parameter)
            {
                Parameter param = e.Parameter as Parameter;
                param.Remove();
            }
        }

        private void Rename_CanExecute_GameObject(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is Parameter)
            {
                e.CanExecute = true;
            }
        }

        private void Rename_Executed_GameObject(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter is Parameter)
            {
                Parameter param = e.Parameter as Parameter;
                param.Remove();
            }
        }

        #endregion

        #region templates
        private void Close_CanExecute_Template(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Close_Executed_Template(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            app.closeTemplate();
        }

        private void OpenTemplate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "Template|*.ctl";
            System.Windows.Forms.DialogResult result = openFile.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                app.openTemplate(openFile.FileName);
            }
        }

        private void OpenRecent_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenRecent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter is App.RecentFile)
            {
                App.RecentFile file = e.Parameter as App.RecentFile;
                string[] extension = file.Filename.Split('.');
                if (extension.Length == 2)
                {
                    if (extension[1] == "ctl")
                    {
                        app.openTemplate(file.FullFilename);
                    }
                    else if (extension[1]== "cep")
                    {
                        app.loadProject(file.FullFilename);
                    }
                }
            }
        }

        private void RunTemplate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = App.Current as App;
            e.CanExecute = app.CurrentProject != null;
        }

        private void RunTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App app = App.Current as App;
            app.runTemplate();
        }

        private void ConvertLive2Script_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ConvertLive2Script_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
        }
        #endregion

        #region peramter
        private void SendMessage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SendMessage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)Application.Current;
            SendMessageWindow msgWnd = new SendMessageWindow();
            msgWnd.Show(e.Parameter);
        }
        #endregion

        #region project
        private void New_CanExecute_Project(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void New_Executed_Project(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (app.CurrentProject != null)
            {
                MessageBoxResult res = MessageBox.Show("Save Project?", "Close Project", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Cancel)
                {
                    return;
                }
                app.closeProject(res == MessageBoxResult.OK);
            }

            NewProjectWindow ProjectWindow = NewProjectWindow.GetInstance();
            NewProjectData data = new NewProjectData(app);
            data.Name = "Project1";
            data.Directory = ".";
            ProjectWindow.DataContext = data;
            bool? result = ProjectWindow.ShowDialog();
            if (result.HasValue)
            {
                if (result.Value)
                {
                    app.newProject(data);
                }
            }
        }

        private bool saveCurrentProjectIfDirty()
        {
            App app = (App)App.Current;
            if (app.CurrentProject != null && app.CurrentProject.Dirty)
            {
                MessageBoxResult result = MessageBox.Show("The current project has unsaved changes. Do you want to save them?", "Close Project", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
                else
                {
                    app.closeProject(result == MessageBoxResult.Yes);
                }
            }
            return true;
        }

        private void Load_CanExecute_Project(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Load_Executed_Project(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (!saveCurrentProjectIfDirty())
                return;
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "Project|*.cep";
            System.Windows.Forms.DialogResult result = openFile.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                app.loadProject(openFile.FileName);
            }
        }

        private void Save_CanExecute_Project(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = (App)App.Current;
            e.CanExecute = app.CurrentProject != null;
        }

        private void Save_Executed_Project(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            app.saveCurrentProject();
        }

        private void Close_CanExecute_Project(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = (App)App.Current;
            e.CanExecute = app.CurrentProject != null;
        }

        private void Close_Executed_Project(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            saveCurrentProjectIfDirty();
        }


        private void Props_CanExecute_Project(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Props_Executed_Project(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter == null)
            {
                app.projectProperties();
            }
            else if (e.Parameter is ProjectFilter)
            {
                ProjectFilter maybeRoot = e.Parameter as ProjectFilter;
                if (maybeRoot == app.CurrentProject.Root)
                {
                    app.projectProperties();
                }
            }
        }
        #region project files and filters
        private void Open_CanExecute_ProjectFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed_ProjectFile(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter is ProjectTemplateFile)
            {
                ProjectTemplateFile template = e.Parameter as ProjectTemplateFile;
                app.loadTemplate(template);
            }
        }

        private void New_CanExecute_ProjectFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void New_Executed_ProjectFile(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter is ProjectFilter)
            {
                ProjectFilter filter = e.Parameter as ProjectFilter;
                app.newTemplate(filter);
            }
            else
            {
                app.newTemplate();
            }
        }

        private void Delete_CanExecute_ProjectFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Delete_Executed_ProjectFile(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter is IProjectElement)
            {
                IProjectElement element = e.Parameter as IProjectElement;
                app.deleteProjectItem(element);
            }
            else if (sender is TreeView)
            {
                TreeView tv = sender as TreeView;
                IProjectElement element = tv.SelectedItem as IProjectElement;
                app.deleteProjectItem(element);
            }
        }

        private void Rename_CanExecute_ProjectFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Rename_Executed_ProjectFile(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            if (e.Parameter is IProjectElement)
            {
                IProjectElement element = e.Parameter as IProjectElement;
                element.Editable = true;
                StringQuery querry = new StringQuery();
                StringQueryItem qry = new StringQueryItem("Enter the new name", "Rename", element.Name);
                querry.DataContext = qry;
                bool? result = querry.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    element.Name = qry.Text;

                    // TODO check if Template already open
                    ObjectBuilder template = app.openTemplate(element.Name);
                    int index = element.Name.LastIndexOf(".");
                    String templateName = element.Name.Substring(0, index);
                    index = templateName.LastIndexOfAny(new char[] { '\\', '/' });
                    templateName = templateName.Substring(index);
                    template.Name = template.Name;
                    app.saveTemplate(template, true);
                }
                //app.renameProjectItem(element);
            }
            else if (sender is TreeView)
            {
                TreeView tv = sender as TreeView;
                IProjectElement projectElement = tv.SelectedItem as IProjectElement;
                projectElement.Editable = true;
                //app.renameProjectItem(tv.SelectedItem as IProjectElement);
            }
        }

        private void Props_CanExecute_ProjectFile(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter == null || e.Parameter is ProjectFilter)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
        #endregion



        private void Export_CanExecute_ProjectFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Export_Executed_ProjectFile(object sender, ExecutedRoutedEventArgs e)
        {
            App app = (App)App.Current;
            app.StatusText = "Exporting...";
            if (e.Parameter is ProjectFilter)
            {
                ProjectFilter filter = e.Parameter as ProjectFilter;
                if (app.CurrentProject != null)
                {
                    app.CurrentProject.export(filter);
                }
            }
            else if (e.Parameter is ProjectTemplateFile)
            {
                ProjectTemplateFile file = e.Parameter as ProjectTemplateFile;
                if (app.CurrentProject != null)
                {
                    app.CurrentProject.export(file);
                }
            }
            else
            {
                if (app.CurrentProject != null)
                {
                    app.CurrentProject.export();
                }
            }
            app.StatusText = "Finished Exporting";
        }
        #endregion

        #region resources
        private void Open_CanExecute_CogaenResource(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed_CogaenResource(object sender, ExecutedRoutedEventArgs e)
        {
            App app = App.Current as App;
            // get selected resource
            MessageBoxResult result = MessageBox.Show(app.MainWindow, "Really load this resource?\n This can take a while.", "Load Resource", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
                app.MessageHandler.getResource(e.Parameter as AbstractResource);
        }

        private void Open_CanExecute_LocalResource(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed_LocalResource(object sender, ExecutedRoutedEventArgs e)
        {
            ResourceManager resMan = e.Parameter as ResourceManager;
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = ofd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                resMan.loadFile(ofd.FileName);
            }
        }
        #endregion

        #region sorting
        private void Sort_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            App app = Application.Current as App;
            if (app.ObjectBuilder != null && app.ObjectBuilder.ScriptObjects != null)
                e.CanExecute = app.ObjectBuilder.ScriptObjects.Count > 1;
            else
                e.CanExecute = false;
        }

        private void Sort_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App app = Application.Current as App;
            if (app.ObjectBuilder != null)
                app.ObjectBuilder.sort();
        }
        #endregion
    }
}
