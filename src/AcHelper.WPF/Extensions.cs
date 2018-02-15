﻿using AcHelper.WPF.Themes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AcHelper.WPF
{
    /// <summary>
    /// Extensions for content controls to handle resources and themes.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Applies the given theme to the UserControl
        /// </summary>
        /// <param name="control"></param>
        /// <param name="oldTheme"></param>
        /// <param name="newTheme"></param>
        public static void ApplyTheme(this ContentControl control, string oldTheme, string newTheme)
        {
            if (string.IsNullOrEmpty(newTheme))
            {
                throw new ArgumentNullException("newTheme");
            }
            if (string.Equals(oldTheme, newTheme, StringComparison.CurrentCulture))
            {
                return;
            }

            // Get plugin name from Assembly to get the associated resourcesCollection
            string pluginName = GetAssemblyName(control);
            IPluginResourcesCollection collection = ThemeManager.GetResourceCollection(pluginName);
            ResourceDictionary newPluginTheme = ThemeManager.GetPluginTheme(pluginName, newTheme);

            // Check if new theme isn't accidentally active already
            if ((control.Resources.MergedDictionaries
                .FirstOrDefault(x => Equals(x.Source, newPluginTheme.Source)) != null))
            {
                return;
            }

            // Remove old theme
            if (!string.IsNullOrEmpty(oldTheme))
            {
                ResourceDictionary oldPluginTheme = ThemeManager.GetPluginTheme(pluginName, oldTheme);
                if (control.Resources.MergedDictionaries
                    .FirstOrDefault(x => Equals(x.Source, oldPluginTheme.Source)) is ResourceDictionary dict)
                {
                    control.Resources.MergedDictionaries.Remove(dict);
                }
            }

            // Set new theme
            control.Resources.MergedDictionaries.Add(newPluginTheme);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public static void ApplyResources(this ContentControl control)
        {
            // Get plugin name from assembly to get the associated resourcesCollection
            string pluginName = GetAssemblyName(control);
            IPluginResourcesCollection coll = ThemeManager.GetResourceCollection(pluginName);

            foreach (ResourceDictionary dict in coll.Resources)
            {
                control.Resources.MergedDictionaries.Add(dict);
            }
        }

        /// <summary>
        /// Returns the name of the Assembly where the ContentControl is created.
        /// </summary>
        /// <param name="control"></param>
        /// <returns>Name of Assembly.</returns>
        private static string GetAssemblyName(ContentControl control)
        {
            return (control.GetType()).Assembly.GetName().Name;
        }
    }
}
