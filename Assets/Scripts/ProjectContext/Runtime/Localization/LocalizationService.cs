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
                    ["Language"] = "Language",
                    ["ItemsShop"] = "Items Shop",
                    ["Group_1"] = "Group 1",
                    ["Group_2"] = "Group 2",
                    ["Item_1"] = "Item name 1",
                    ["Item_2"] = "Item name 2",
                    ["Item_3"] = "Item name 3",
                    ["Item_4"] = "Item name 4",
                    ["Item_5"] = "Item name 5",
                    ["Item_6"] = "Item name 6",
                    ["Item_7"] = "Item name 7",
                    ["Item_8"] = "Item name 8",
                    ["Item_9"] = "Item name 9",
                    ["Item_10"] = "Item name 10",
                    ["ChoiceItemCaption"] = "Choice of item",
                    ["ChoiceItemDescription"] = "The player selected item {0}",
                    ["ShopAmount"] = "Amount: {0}",
                    ["ShopGroup"] = "Group: {0}",
                    ["TimeTemplate"] = "Time : {0}",
                    ["HintDescription"] = "Test hint description, hint can be very large",
                    ["PressAndHold"] = "press and hold"
                },
                [SystemLanguage.French] = new Dictionary<string, string>
                {
                    ["Settings"] = "Paramètres",
                    ["TextMusic"] = "MUSIQUE",
                    ["TextSound"] = "SONNER",
                    ["Language"] = "Langue",
                    ["ItemsShop"] = "Boutique",
                    ["Group_1"] = "Groupe 1",
                    ["Group_2"] = "Groupe 2",
                    ["Item_1"] = "Nom de l'objet 1",
                    ["Item_2"] = "Nom de l'objet 2",
                    ["Item_3"] = "Nom de l'objet 3",
                    ["Item_4"] = "Nom de l'objet 4",
                    ["Item_5"] = "Nom de l'objet 5",
                    ["Item_6"] = "Nom de l'objet 6",
                    ["Item_7"] = "Nom de l'objet 7",
                    ["Item_8"] = "Nom de l'objet 8",
                    ["Item_9"] = "Nom de l'objet 9",
                    ["Item_10"] = "Nom de l'objet 10",
                    ["ChoiceItemCaption"] = "Choix d'objet",
                    ["ChoiceItemDescription"] = "Le joueur a sélectionné {0}",
                    ["ShopAmount"] = "Quantité : {0}",
                    ["ShopGroup"] = "Groupe : {0}",
                    ["TimeTemplate"] = "Temps : {0}",
                    ["HintDescription"] = "Description de l'indice de test, l'indice peut être très volumineux",
                    ["PressAndHold"] = "appuyez et maintenez"
                },
                [SystemLanguage.German] = new Dictionary<string, string>
                {
                    ["Settings"] = "die Einstellungen",
                    ["TextMusic"] = "MUSIK",
                    ["TextSound"] = "KLANG",
                    ["Language"] = "Sprache",
                    ["ItemsShop"] = "Gegenstandsladen",
                    ["Group_1"] = "Gruppe 1",
                    ["Group_2"] = "Gruppe 2",
                    ["Item_1"] = "Gegenstand 1",
                    ["Item_2"] = "Gegenstand 2",
                    ["Item_3"] = "Gegenstand 3",
                    ["Item_4"] = "Gegenstand 4",
                    ["Item_5"] = "Gegenstand 5",
                    ["Item_6"] = "Gegenstand 6",
                    ["Item_7"] = "Gegenstand 7",
                    ["Item_8"] = "Gegenstand 8",
                    ["Item_9"] = "Gegenstand 9",
                    ["Item_10"] = "Gegenstand 10",
                    ["ChoiceItemCaption"] = "Gegenstandsauswahl",
                    ["ChoiceItemDescription"] = "Der Spieler hat {0} ausgewählt",
                    ["ShopAmount"] = "Menge: {0}",
                    ["ShopGroup"] = "Gruppe: {0}",
                    ["TimeTemplate"] = "Zeit : {0}",
                    ["HintDescription"] = "Testhinweisbeschreibung, Hinweis kann sehr groß sein",
                    ["PressAndHold"] = "drücken und halten"
                },
                [SystemLanguage.Russian] = new Dictionary<string, string>
                {
                    ["Settings"] = "Настройки",
                    ["TextMusic"] = "МУЗЫКА",
                    ["TextSound"] = "ЗВУК",
                    ["Language"] = "Язык",
                    ["ItemsShop"] = "Магазин",
                    ["Group_1"] = "Группа 1",
                    ["Group_2"] = "Группа 2",
                    ["Item_1"] = "Предмет 1",
                    ["Item_2"] = "Предмет 2",
                    ["Item_3"] = "Предмет 3",
                    ["Item_4"] = "Предмет 4",
                    ["Item_5"] = "Предмет 5",
                    ["Item_6"] = "Предмет 6",
                    ["Item_7"] = "Предмет 7",
                    ["Item_8"] = "Предмет 8",
                    ["Item_9"] = "Предмет 9",
                    ["Item_10"] = "Предмет 10",
                    ["ChoiceItemCaption"] = "Выбор предмета",
                    ["ChoiceItemDescription"] = "Игрок выбрал {0}",
                    ["ShopAmount"] = "Количество: {0}",
                    ["ShopGroup"] = "Группа: {0}",
                    ["TimeTemplate"] = "Время : {0}",
                    ["HintDescription"] = "Тестовое описание хинта, хинт может быть очень большим",
                    ["PressAndHold"] = "нажать и удерживать"
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
