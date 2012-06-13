using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using CogaenDataItems.Manager;

namespace CogaenEditor2.Resources.TemplateSelectors
{
    public class ObjectBuilderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is ObjectBuilder)
            {
                ObjectBuilder objectBuilder = item as ObjectBuilder;

                if (objectBuilder.IsMacro)
                    return
                        element.FindResource("MacroTemplate") as DataTemplate;
                else
                    return
                        element.FindResource("ScriptTemplate") as DataTemplate;
            }
            return null;
        }

    }
}
