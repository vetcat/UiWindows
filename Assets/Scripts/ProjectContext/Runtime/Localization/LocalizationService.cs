using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace ProjectContext.Localization
{
    public sealed class LocalizationService : ILocalizationService, IDisposable
    {
        public const string LanguageKey = "Language";
        public const SystemLanguage DefaultLanguage = SystemLanguage.English;

        private static readonly IReadOnlyList<SystemLanguage> Languages = new[]
        {
            SystemLanguage.Russian,
            SystemLanguage.English,
            SystemLanguage.German,
            SystemLanguage.French
        };

        private static readonly IReadOnlyDictionary<SystemLanguage, IReadOnlyDictionary<string, string>> Catalog =
            new Dictionary<SystemLanguage, IReadOnlyDictionary<string, string>>
            {
                [SystemLanguage.English] = new Dictionary<string, string>
                {
                    ["Settings"] = "Settings",
                    ["TextMusic"] = "MUSUC",
                    ["TextSound"] = "SOUND",
                    ["Language"] = "Language"
                },
                [SystemLanguage.French] = new Dictionary<string, string>
                {
                    ["Settings"] = "Paramètres",
                    ["TextMusic"] = "MUSIQUE",
                    ["TextSound"] = "SONNER",
                    ["Language"] = "Langue"
                },
                [SystemLanguage.German] = new Dictionary<string, string>
                {
                    ["Settings"] = "die Einstellungen",
                    ["TextMusic"] = "MUSIK",
                    ["TextSound"] = "KLANG",
                    ["Language"] = "Sprache"
                },
                [SystemLanguage.Russian] = new Dictionary<string, string>
                {
                    ["Settings"] = "Настройки",
                    ["TextMusic"] = "МУЗЫКА",
                    ["TextSound"] = "ЗВУК",
                    ["Language"] = "Язык"
                }
            };

        private readonly ReactiveProperty<SystemLanguage> currentLanguage;
        private readonly Subject<SystemLanguage> languageChanged = new();
        private bool disposed;

        public LocalizationService()
            : this(ResolveInitialLanguage())
        {
        }

        public LocalizationService(SystemLanguage initialLanguage)
        {
            currentLanguage = new ReactiveProperty<SystemLanguage>(NormalizeLanguage(initialLanguage));
        }

        public IReadOnlyList<SystemLanguage> AvailableLanguages => Languages;
        public ReadOnlyReactiveProperty<SystemLanguage> CurrentLanguage => currentLanguage;
        public Observable<SystemLanguage> LanguageChanged => languageChanged;

        public bool HasKey(string key)
        {
            ThrowIfDisposed();
            return Catalog[currentLanguage.CurrentValue].ContainsKey(key);
        }

        public string Translate(string key, params object[] args)
        {
            ThrowIfDisposed();

            if (key == null || !Catalog[currentLanguage.CurrentValue].TryGetValue(key, out var value))
            {
                Debug.LogError("Localization key not found " + key);
                return key ?? string.Empty;
            }

            if (args == null || args.Length == 0)
            {
                return value.Replace(@"\n", "\n");
            }

            try
            {
                return string.Format(value, args).Replace(@"\n", "\n");
            }
            catch (Exception exception)
            {
                return "ERROR: " + exception.Message;
            }
        }

        public void ChangeLanguage(SystemLanguage language)
        {
            ThrowIfDisposed();

            if (Catalog.ContainsKey(language) == false)
            {
                Debug.LogError("Localization not found for language = " + language);
                return;
            }

            if (currentLanguage.CurrentValue == language)
            {
                return;
            }

            currentLanguage.Value = language;
            PlayerPrefs.SetInt(LanguageKey, (int)language);
            PlayerPrefs.Save();
            languageChanged.OnNext(language);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            languageChanged.Dispose();
            currentLanguage.Dispose();
        }

        private static SystemLanguage ResolveInitialLanguage()
        {
            if (PlayerPrefs.HasKey(LanguageKey))
            {
                return NormalizeLanguage((SystemLanguage)PlayerPrefs.GetInt(LanguageKey));
            }

            return NormalizeLanguage(Application.systemLanguage);
        }

        private static SystemLanguage NormalizeLanguage(SystemLanguage language)
        {
            return Catalog.ContainsKey(language) ? language : DefaultLanguage;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(LocalizationService));
            }
        }
    }
}