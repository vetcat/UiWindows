using System.Collections.Generic;
using R3;
using UnityEngine;

namespace ProjectContext.Localization
{
    public interface ILocalizationReadModel
    {
        IReadOnlyList<SystemLanguage> AvailableLanguages { get; }
        ReadOnlyReactiveProperty<SystemLanguage> CurrentLanguage { get; }
        Observable<SystemLanguage> LanguageChanged { get; }
        bool HasKey(string key);
        string Translate(string key, params object[] args);
    }
}