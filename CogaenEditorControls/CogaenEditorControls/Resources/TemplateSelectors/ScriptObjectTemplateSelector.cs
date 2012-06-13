using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using CogaenDataItems.DataItems;

namespace CogaenEditorControls.Resources.TemplateSelectors
{
    public class ScriptObjectTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is IScriptObject)
            {
                IScriptObject scriptObject = item as IScriptObject;

                if (scriptObject is GameObject)
                {
                    DataTemplate template = element.FindResource("GameObjectTemplate") as DataTemplate;
                    return template;
                }
                else if (scriptObject is LiveGameObject)
                {
                    DataTemplate template = element.FindResource("LiveGameObjectTemplate") as DataTemplate;
                    return template;
                }
                else if (scriptObject is MacroRegistration) 
                {
                    return
                        element.FindResource("MacroRegistrationTemplate") as DataTemplate;
                }
                else if (scriptObject is MacroCall)
                {
                    return
                        element.FindResource("MacroCallTemplate") as DataTemplate; 
                }
                else
                    return
                        element.FindResource("LiveGameObjectTemplate") as DataTemplate;
            }
            return null;
        }

    }
}
