using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using CogaenDataItems.Manager;

namespace CogaenEditorControls.Resources.TemplateSelectors
{
    public class MacroTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is ObjectBuilder)
            {
                ObjectBuilder obj = item as ObjectBuilder;

                if (obj.IsMacro && obj.IsRegistered)
                {
                    return
                        element.FindResource("RegisteredMacro") as DataTemplate;
                }
                else if (obj.IsMacro && obj.IsLive)
                {
                    return
                        element.FindResource("LiveMacro") as DataTemplate;
                }
                else
                {
                    return
                        element.FindResource("UnregisteredMacro") as DataTemplate;
                }
            }
            return null;
        }

    }
}
