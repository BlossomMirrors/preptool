using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace BlossomPrepTool
{
    /// <summary>
    /// Handles application localization and culture switching
    /// </summary>
    public static class Localizer
    {
        private static CultureInfo _currentCulture;
        private static ResourceManager _resourceManager;

        // Available cultures
        public static readonly CultureInfo EnglishUS = new CultureInfo("en-US");
        public static readonly CultureInfo GermanDE = new CultureInfo("de-DE");

        public static List<CultureInfo> AvailableCultures { get; } = new List<CultureInfo>
        {
            EnglishUS,
            GermanDE
        };

        static Localizer()
        {
            _resourceManager = new ResourceManager("BlossomPrepTool.Strings", typeof(Localizer).Assembly);
            _currentCulture = CultureInfo.CurrentUICulture;

            // Fall back to English if current culture is not supported
            if (!AvailableCultures.Any(c => c.Name == _currentCulture.Name))
            {
                if (_currentCulture.TwoLetterISOLanguageName == "de")
                {
                    _currentCulture = GermanDE;
                }
                else
                {
                    _currentCulture = EnglishUS;
                }
            }
        }

        private static void InitializeStringDictionaries()
        {
        }

        /// <summary>
        /// Gets the current UI culture
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (AvailableCultures.Any(c => c.Name == value.Name))
                {
                    _currentCulture = value;
                    CultureInfo.CurrentUICulture = value;
                }
            }
        }

        /// <summary>
        /// Gets a localized string resource
        /// </summary>
        public static string GetString(string key)
        {
            try
            {
                var value = _resourceManager?.GetString(key, _currentCulture);
                if (value != null)
                    return value;
            }
            catch { }

            return $"[{key}]";
        }

        /// <summary>
        /// Gets a localized string with format arguments
        /// </summary>
        public static string GetString(string key, params object[] args)
        {
            string template = null;

            try
            {
                template = _resourceManager?.GetString(key, _currentCulture);
            }
            catch { }

            if (!string.IsNullOrEmpty(template))
            {
                try
                {
                    return string.Format(template, args);
                }
                catch
                {
                    return template;
                }
            }

            return $"[{key}]";
        }

        /// <summary>
        /// Loads the preferred culture from OS settings
        /// </summary>
        public static void LoadPreferredCulture()
        {
            // Culture is already detected in the static constructor from OS settings
        }
    }
}
