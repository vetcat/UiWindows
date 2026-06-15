using UnityEngine;

namespace ProjectContext.Localization
{
    public interface ILocalizationCommands
    {
        void ChangeLanguage(SystemLanguage language);
    }
}