using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BlossomPrepTool
{
    /// <summary>
    /// Handles application localization and culture switching
    /// </summary>
    public static class Localizer
    {
        private static CultureInfo _currentCulture;
        private static readonly Dictionary<string, IReadOnlyDictionary<string, string>> FallbackLocales =
            new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["en-US"] = Locale_en_US.Strings,
                ["en"] = Locale_en_US.Strings,
                ["de-DE"] = Locale_de_DE.Strings,
                ["de"] = Locale_de_DE.Strings
            };

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
            var fallback = GetFallbackString(key);
            if (!string.IsNullOrEmpty(fallback))
            {
                return fallback;
            }

            return $"[{key}]";
        }

        /// <summary>
        /// Gets a localized string with format arguments
        /// </summary>
        public static string GetString(string key, params object[] args)
        {
            string template = null;

            if (string.IsNullOrEmpty(template))
            {
                template = GetFallbackString(key);
            }

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

        private static string GetFallbackString(string key)
        {
            if (_currentCulture != null)
            {
                if (FallbackLocales.TryGetValue(_currentCulture.Name, out var exact) &&
                    exact.TryGetValue(key, out var exactValue))
                {
                    return exactValue;
                }

                var neutral = _currentCulture.TwoLetterISOLanguageName;
                if (FallbackLocales.TryGetValue(neutral, out var neutralMap) &&
                    neutralMap.TryGetValue(key, out var neutralValue))
                {
                    return neutralValue;
                }
            }

            if (FallbackLocales.TryGetValue(EnglishUS.Name, out var fallback) &&
                fallback.TryGetValue(key, out var fallbackValue))
            {
                return fallbackValue;
            }

            return null;
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
