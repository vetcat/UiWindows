using System;
using ProjectContext.Localization;
using ProjectContext.Settings;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Windows;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiSettingsPresenter : IWindowPresenter<UiSettingsWindow>
    {
        private readonly IGameSettingsReadModel settingsReadModel;
        private readonly IGameSettingsCommands settingsCommands;
        private readonly ILocalizationReadModel localizationReadModel;
        private readonly ILocalizationCommands localizationCommands;
        private readonly Func<UiSettingsWindow, UiSettingsView> viewResolver;

        private UiSettingsWindow window;
        private UiSettingsView view;
        private UiSettingsTab selectedTab = UiSettingsTab.Settings;
        private bool disposed;

        public UiSettingsPresenter(
            IGameSettingsReadModel settingsReadModel,
            IGameSettingsCommands settingsCommands,
            ILocalizationReadModel localizationReadModel,
            ILocalizationCommands localizationCommands)
            : this(settingsReadModel, settingsCommands, localizationReadModel, localizationCommands, ResolveView)
        {
        }

        internal UiSettingsPresenter(
            IGameSettingsReadModel settingsReadModel,
            IGameSettingsCommands settingsCommands,
            ILocalizationReadModel localizationReadModel,
            ILocalizationCommands localizationCommands,
            Func<UiSettingsWindow, UiSettingsView> viewResolver)
        {
            this.settingsReadModel =
                settingsReadModel ?? throw new ArgumentNullException(nameof(settingsReadModel));
            this.settingsCommands = settingsCommands ?? throw new ArgumentNullException(nameof(settingsCommands));
            this.localizationReadModel =
                localizationReadModel ?? throw new ArgumentNullException(nameof(localizationReadModel));
            this.localizationCommands =
                localizationCommands ?? throw new ArgumentNullException(nameof(localizationCommands));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
        }

        public void Bind(UiSettingsWindow window)
        {
            ThrowIfDisposed();
            this.window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void Initialize()
        {
            ThrowIfDisposed();
            view = ResolveRequiredView();
            ConfigureSliders();
            view.RebuildLanguageItems(localizationReadModel.AvailableLanguages);
            SelectTab(UiSettingsTab.Settings);
        }

        public void OnShowBegin(IUiShowScope showScope)
        {
            ThrowIfDisposed();
            if (showScope == null)
            {
                throw new ArgumentNullException(nameof(showScope));
            }

            view = ResolveRequiredView();
            RefreshLocalizedText();
            RefreshLanguageSelection(localizationReadModel.CurrentLanguage.CurrentValue);

            showScope.Add(settingsReadModel.MusicVolume.Subscribe(SetMusicVolume));
            showScope.Add(settingsReadModel.SoundVolume.Subscribe(SetSoundVolume));
            showScope.Add(localizationReadModel.CurrentLanguage.Subscribe(_ =>
            {
                RefreshLocalizedText();
                RefreshLanguageSelection(localizationReadModel.CurrentLanguage.CurrentValue);
            }));

            AddSliderListener(showScope, view.SettingsLayout?.SliderMusicVolume, settingsCommands.SetMusicVolume);
            AddSliderListener(showScope, view.SettingsLayout?.SliderSoundVolume, settingsCommands.SetSoundVolume);
            AddToggleListener(showScope, view.ToggleSettings, UiSettingsTab.Settings);
            AddToggleListener(showScope, view.ToggleLanguage, UiSettingsTab.Language);
            AddButtonListener(showScope, view.ButtonClose, HideWindow);
            AddLanguageListeners(showScope);
        }

        public void OnShowEnd()
        {
        }

        public void OnHideBegin()
        {
        }

        public void OnHideEnd()
        {
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            view = null;
            window = null;
        }

        private UiSettingsView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiSettingsView was not loaded by the UiSettingsWindow layout.");
            }

            return resolved;
        }

        private static UiSettingsView ResolveView(UiSettingsWindow window)
        {
            if (window == null)
            {
                return null;
            }

            return window.TryGetView(out var resolved) ? resolved : null;
        }

        private void ConfigureSliders()
        {
            ConfigureSlider(view.SettingsLayout?.SliderMusicVolume);
            ConfigureSlider(view.SettingsLayout?.SliderSoundVolume);
        }

        private static void ConfigureSlider(Slider slider)
        {
            if (slider == null)
            {
                return;
            }

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
        }

        private void SetMusicVolume(float value)
        {
            SetSlider(view.SettingsLayout?.SliderMusicVolume, value);
            SetText(view.SettingsLayout?.TextSliderMusicValue, FormatPercent(value));
        }

        private void SetSoundVolume(float value)
        {
            SetSlider(view.SettingsLayout?.SliderSoundVolume, value);
            SetText(view.SettingsLayout?.TextSliderSoundValue, FormatPercent(value));
        }

        private void SelectTab(UiSettingsTab tab)
        {
            selectedTab = tab;
            view.SelectTab(tab);
            RefreshLocalizedText();
        }

        private void RefreshLocalizedText()
        {
            if (view == null)
            {
                return;
            }

            SetText(view.TextToggleSettings, Translate("Settings"));
            SetText(view.TextToggleLanguage, Translate("Language"));
            SetText(view.SettingsLayout?.TextMusic, Translate("TextMusic"));
            SetText(view.SettingsLayout?.TextSound, Translate("TextSound"));
            SetText(
                view.TextHeader,
                Translate(selectedTab == UiSettingsTab.Settings ? "Settings" : "Language"));
        }

        private void RefreshLanguageSelection(SystemLanguage currentLanguage)
        {
            if (view?.LanguagesLayout?.Items == null)
            {
                return;
            }

            for (var i = 0; i < view.LanguagesLayout.Items.Count; i++)
            {
                var item = view.LanguagesLayout.Items[i];
                if (item?.Toggle == null)
                {
                    continue;
                }

                item.Toggle.SetIsOnWithoutNotify(item.Language == currentLanguage);
            }
        }

        private void AddLanguageListeners(IUiShowScope showScope)
        {
            if (view?.LanguagesLayout?.Items == null)
            {
                return;
            }

            for (var i = 0; i < view.LanguagesLayout.Items.Count; i++)
            {
                var item = view.LanguagesLayout.Items[i];
                if (item?.Toggle == null)
                {
                    continue;
                }

                UnityAction<bool> action = isOn =>
                {
                    if (isOn)
                    {
                        localizationCommands.ChangeLanguage(item.Language);
                        SelectTab(UiSettingsTab.Language);
                    }
                };

                item.Toggle.onValueChanged.AddListener(action);
                showScope.Add(new DisposableAction(() => item.Toggle.onValueChanged.RemoveListener(action)));
            }
        }

        private void HideWindow()
        {
            window?.Hide(TransitionParameters.Default.ReplaceImmediately(true));
        }

        private string Translate(string key)
        {
            return localizationReadModel.Translate(key);
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetSlider(Slider slider, float value)
        {
            if (slider != null)
            {
                slider.SetValueWithoutNotify(Mathf.Clamp01(value));
            }
        }

        private static string FormatPercent(float value)
        {
            return Mathf.RoundToInt(Mathf.Clamp01(value) * 100f).ToString();
        }

        private static void AddSliderListener(IUiShowScope showScope, Slider slider, UnityAction<float> action)
        {
            if (slider == null)
            {
                return;
            }

            slider.onValueChanged.AddListener(action);
            showScope.Add(new DisposableAction(() => slider.onValueChanged.RemoveListener(action)));
        }

        private void AddToggleListener(IUiShowScope showScope, Toggle toggle, UiSettingsTab tab)
        {
            if (toggle == null)
            {
                return;
            }

            UnityAction<bool> action = isOn =>
            {
                if (isOn)
                {
                    SelectTab(tab);
                }
            };

            toggle.onValueChanged.AddListener(action);
            showScope.Add(new DisposableAction(() => toggle.onValueChanged.RemoveListener(action)));
        }

        private static void AddButtonListener(IUiShowScope showScope, Button button, UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.AddListener(action);
            showScope.Add(new DisposableAction(() => button.onClick.RemoveListener(action)));
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(UiSettingsPresenter));
            }
        }

        private sealed class DisposableAction : IDisposable
        {
            private Action action;

            public DisposableAction(Action action)
            {
                this.action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public void Dispose()
            {
                var callback = action;
                action = null;
                callback?.Invoke();
            }
        }
    }
}