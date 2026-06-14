using System;
using ProjectContext.Player;
using R3;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UiWindowsMvp.SampleSceneWindows
{
    public sealed class UiTopLeftPresenter : IWindowPresenter<UiTopLeftWindow>
    {
        public const int DefaultHealthCommandStep = 10;
        public const int DefaultXpCommandStep = 25;

        private readonly IPlayerReadModel readModel;
        private readonly IPlayerCommands commands;
        private readonly IPlayerSettings settings;
        private readonly int healthCommandStep;
        private readonly int xpCommandStep;
        private readonly Func<UiTopLeftWindow, UiTopLeftView> viewResolver;

        private UiTopLeftWindow window;
        private UiTopLeftView view;
        private bool disposed;

        public UiTopLeftPresenter(
            IPlayerReadModel readModel,
            IPlayerCommands commands,
            IPlayerSettings settings,
            int healthCommandStep = DefaultHealthCommandStep,
            int xpCommandStep = DefaultXpCommandStep)
            : this(readModel, commands, settings, healthCommandStep, xpCommandStep, ResolveView)
        {
        }

        internal UiTopLeftPresenter(
            IPlayerReadModel readModel,
            IPlayerCommands commands,
            IPlayerSettings settings,
            int healthCommandStep,
            int xpCommandStep,
            Func<UiTopLeftWindow, UiTopLeftView> viewResolver)
        {
            this.readModel = readModel ?? throw new ArgumentNullException(nameof(readModel));
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.viewResolver = viewResolver ?? throw new ArgumentNullException(nameof(viewResolver));
            this.healthCommandStep = Math.Max(1, healthCommandStep);
            this.xpCommandStep = Math.Max(1, xpCommandStep);
        }

        public void Bind(UiTopLeftWindow window)
        {
            ThrowIfDisposed();
            this.window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void Initialize()
        {
            ThrowIfDisposed();
            view = ResolveRequiredView();
            SetText(view.TextLevel, "Level");
        }

        public void OnShowBegin(IUiShowScope showScope)
        {
            ThrowIfDisposed();
            if (showScope == null)
            {
                throw new ArgumentNullException(nameof(showScope));
            }

            view = ResolveRequiredView();

            showScope.Add(readModel.Name.Subscribe(SetPlayerName));
            showScope.Add(readModel.Level.Subscribe(SetLevel));
            showScope.Add(readModel.Health.Subscribe(SetHealth));
            showScope.Add(readModel.XpProgress.Subscribe(SetXpProgress));

            AddButtonListener(showScope, view.HealthData.ButtonAdd, () => commands.AddHealth(healthCommandStep));
            AddButtonListener(showScope, view.HealthData.ButtonReduce, () => commands.AddHealth(-healthCommandStep));
            AddButtonListener(showScope, view.XpData.ButtonAdd, () => commands.AddXp(xpCommandStep));
            AddButtonListener(showScope, view.XpData.ButtonReduce, () => commands.AddXp(-xpCommandStep));
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

        private UiTopLeftView ResolveRequiredView()
        {
            var resolved = viewResolver(window);
            if (resolved == null)
            {
                throw new InvalidOperationException("UiTopLeftView was not loaded by the UiTopLeftWindow layout.");
            }

            return resolved;
        }

        private static UiTopLeftView ResolveView(UiTopLeftWindow window)
        {
            if (window == null)
            {
                return null;
            }

            return window.TryGetView(out var resolved) ? resolved : null;
        }

        private void SetPlayerName(string playerName)
        {
            SetText(view.TextPlayerName, playerName ?? string.Empty);
        }

        private void SetLevel(int level)
        {
            SetText(view.TextLevelValue, level.ToString());
        }

        private void SetHealth(int health)
        {
            SetText(view.HealthData.TextValue, $"{health} / {settings.MaxHealth}");
            SetSlider(view.HealthData.Slider, 0f, settings.MaxHealth, health);
        }

        private void SetXpProgress(PlayerXpProgress progress)
        {
            var max = Mathf.Max(1, progress.NextLevelXpBound);
            var value = progress.IsMaxLevel ? max : Mathf.Clamp(progress.CurrentLevelXp, 0, max);
            var text = progress.IsMaxLevel
                ? $"{progress.TotalXp} XP (max)"
                : $"{progress.TotalXp} XP ({progress.CurrentLevelXp} / {progress.NextLevelXpBound})";

            SetText(view.XpData.TextValue, text);
            SetSlider(view.XpData.Slider, 0f, max, value);
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetSlider(Slider slider, float min, float max, float value)
        {
            if (slider == null)
            {
                return;
            }

            slider.minValue = min;
            slider.maxValue = Mathf.Max(min + 0.0001f, max);
            slider.SetValueWithoutNotify(Mathf.Clamp(value, slider.minValue, slider.maxValue));
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
                throw new ObjectDisposedException(nameof(UiTopLeftPresenter));
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