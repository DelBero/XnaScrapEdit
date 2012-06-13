using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogaenDataItems.DataItems;
using System.Windows.Controls;
using System.Windows;
using CogaenEditor2.Manager;
using CogaenDataItems.Manager;

namespace CogaenEditor2.GUI.DragDrop
{
    public partial class DragDropHandler
    {
        #region member
        App m_app;

        public App App
        {
            get { return m_app; }
            set { m_app = value; }
        }
        #endregion
        #region GameObject
        public void GameObject_PreviewDragOver(GameObject pGo, DragEventArgs e)
        {
            if (pGo != null)
            {
                Element element = e.Data.GetData(typeof(Element)) as Element;
                if (element != null)
                {
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        public void GameObject_Drop(GameObject pGo, DragEventArgs e)
        {
            if (pGo != null)
            {
                Element elements = e.Data.GetData(typeof(Element)) as Element;
                if (elements != null)
                {
                    Element newElement = new Element(elements);
                    if (pGo != null)
                    {
                        pGo.Add(newElement);
                        e.Handled = true;
                    }
                }
            }
        }

        public void GameObjectsCanvas_DragEnter(object sender, DragEventArgs e)
        {
            DataObject data = e.Data as DataObject;
            Element element = data.GetData(typeof(Element)) as Element;
            if (element != null)
            {
                //System.Console.WriteLine(comp);
            }
            else
            {
                e.Effects = DragDropEffects.None;
                //e.Handled = true;
            }
        }

        public void GameObjectsCanvas_Drop(object sender, DragEventArgs e)
        {
            Element element = e.Data.GetData(typeof(Element)) as Element;
            if (element != null)
            {
                System.Console.WriteLine("dropped " + element);
                GameObject pGo = App.ObjectBuilder.click(e.GetPosition(sender as Canvas)) as GameObject;
                Element newElement = new Element(element);
                if (pGo != null)
                {
                    pGo.Add(newElement);
                }
                else
                {
                    pGo = App.newGameObject();
                    if (pGo != null)
                    {
                        pGo.Position = e.GetPosition(sender as Canvas);
                        pGo.Add(newElement);
                    }
                }
            }
            ObjectBuilder ob = e.Data.GetData(typeof(ObjectBuilder)) as ObjectBuilder;
            if (ob != null && ob.IsMacro)
            {
                if (ob.IsRegistered || ob.IsLive)
                {
                    // TODO: Show Macro window
                    MacroCall macroCall = new MacroCall();
                    macroCall.Position = e.GetPosition(sender as Canvas);
                    macroCall.Macro = ob;
                    if (ob != App.ObjectBuilder)
                    {
                        App.ObjectBuilder.ScriptObjects.Add(macroCall);
                    }
                    else
                    {
                        MessageBox.Show("Cannot call Macro from within itself!", "Error");
                    }
                }
                else
                {
                    App.registerMacroInScript(ob);

                    (App.MainWindow as MainWindow).sortMacros();
                }
            }
        }
        #endregion

        #region parameter
        public void Parameter_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (sender is TextBox)
            {
                //Parameter.PreviewDragOver((sender as TextBox).DataContext as Parameter, e);
            }
        }

        public void Parameter_Drop(object sender, DragEventArgs e)
        {
            if (sender is TextBox)
            {
                //Parameter.Drop((sender as TextBox).DataContext as Parameter, e);
            }
        }
        #endregion

        #region Gameobject grid
        public void GameObjectsGrid_DragEnter(object sender, DragEventArgs e)
        {
            DataObject data = e.Data as DataObject;
            Element element = data.GetData(typeof(Element)) as Element;
            if (element != null)
            {
                //System.Console.WriteLine(comp);
            }
            else
            {
                e.Effects = DragDropEffects.None;
                //e.Handled = true;
            }
        }

        public void GameObjectsGrid_Drop(object sender, DragEventArgs e)
        {
            App app = App.Current as App;
            if (app == null)
                return;
            Element element = e.Data.GetData(typeof(Element)) as Element;
            if (element != null)
            {
                System.Console.WriteLine("dropped " + element);
                GameObject pGo = app.ObjectBuilder.click(e.GetPosition(sender as Canvas)) as GameObject;
                Element newElement = new Element(element);
                if (pGo != null)
                {
                    pGo.Add(newElement);
                }
                else
                {
                    pGo = app.newGameObject();
                    if (pGo != null)
                    {
                        pGo.Position = e.GetPosition(sender as Canvas);
                        pGo.Add(newElement);
                    }
                }
            }
            ObjectBuilder ob = e.Data.GetData(typeof(ObjectBuilder)) as ObjectBuilder;
            if (ob != null && ob.IsMacro)
            {
                if (ob.IsRegistered || ob.IsLive)
                {
                    // TODO: Show Macro window
                    MacroCall macroCall = new MacroCall();
                    macroCall.Position = e.GetPosition(sender as Canvas);
                    macroCall.Macro = ob;
                    if (ob != app.ObjectBuilder)
                    {
                        app.ObjectBuilder.ScriptObjects.Add(macroCall);
                    }
                    else
                    {
                        MessageBox.Show("Cannot call Macro from within itself!", "Error");
                    }
                }
                else
                {
                    app.registerMacroInScript(ob);
                    (app.MainWindow as MainWindow).sortMacros();
                }
            }
        }
        #endregion

        #region project
        public void Project_PreviewDragOver(object sender, DragEventArgs e)
        {
            App app = App.Current as App;
            if (app.CurrentProject == null)
            {
                return;
            }
            if (sender is TreeView)
            {
                IProjectElement projectElement = e.Data.GetData(typeof(ProjectTemplateFile)) as IProjectElement;
                if (projectElement == null)
                    projectElement = e.Data.GetData(typeof(ProjectFilter)) as IProjectElement;
                app.CurrentProject.PreviewDragOver(projectElement, e);
            }
        }

        public void Project_Drop(object sender, DragEventArgs e)
        {
            App app = App.Current as App;
            if (app.CurrentProject == null)
            {
                return;
            }
            if (sender is TreeView)
            {
                //IInputElement input = (sender as TreeView).InputHitTest(e.GetPosition(sender as IInputElement));
                IProjectElement projectElement = e.Data.GetData(typeof(ProjectTemplateFile)) as IProjectElement;
                if (projectElement == null)
                    projectElement = e.Data.GetData(typeof(ProjectFilter)) as IProjectElement;
                app.CurrentProject.Drop(projectElement, e);
            }
        }
        #endregion
    }
}
