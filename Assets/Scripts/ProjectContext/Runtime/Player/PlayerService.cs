using System;
using ProjectContext.UiRequests;
using R3;

namespace ProjectContext.Player
{
    public sealed class PlayerService : IPlayerService, IDisposable
    {
        public const string DefaultPlayerName = "Default name";
        public const int InitialLevel = 1;

        private readonly ReactiveProperty<int> _health;
        private readonly ReactiveProperty<int> _xp;
        private readonly ReactiveProperty<int> _coins;
        private readonly ReactiveProperty<int> _level;
        private readonly ReactiveProperty<string> _name;
        private readonly ReactiveProperty<PlayerXpProgress> _xpProgress;
        private readonly Subject<PlayerXpProgress> _xpUpdates = new();
        private readonly Subject<int> _levelUps = new();
        private readonly IUiFeedbackCommands feedbackCommands;

        private bool _disposed;

        public PlayerService(IPlayerSettings settings)
            : this(settings, null)
        {
        }

        public PlayerService(IPlayerSettings settings, IUiFeedbackCommands feedbackCommands)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.feedbackCommands = feedbackCommands;

            _health = new ReactiveProperty<int>(Settings.MaxHealth);
            _xp = new ReactiveProperty<int>(0);
            _coins = new ReactiveProperty<int>(0);
            _level = new ReactiveProperty<int>(InitialLevel);
            _name = new ReactiveProperty<string>(DefaultPlayerName);
            _xpProgress =
                new ReactiveProperty<PlayerXpProgress>(CalculateProgress(_xp.CurrentValue, _level.CurrentValue));
        }

        public IPlayerSettings Settings { get; }
        public ReadOnlyReactiveProperty<int> Health => _health;
        public ReadOnlyReactiveProperty<int> Xp => _xp;
        public ReadOnlyReactiveProperty<int> Coins => _coins;
        public ReadOnlyReactiveProperty<int> Level => _level;
        public ReadOnlyReactiveProperty<string> Name => _name;
        public ReadOnlyReactiveProperty<PlayerXpProgress> XpProgress => _xpProgress;
        public Observable<PlayerXpProgress> XpUpdates => _xpUpdates;
        public Observable<int> LevelUps => _levelUps;

        public void SetHealth(int value)
        {
            ThrowIfDisposed();
            _health.Value = Clamp(value, 0, Settings.MaxHealth);
        }

        public void AddHealth(int amount)
        {
            SetHealth(AddClamped(_health.CurrentValue, amount));
        }

        public void SetXp(int value)
        {
            ThrowIfDisposed();

            var nextXp = Math.Max(0, value);
            if (_xp.CurrentValue == nextXp)
            {
                return;
            }

            _xp.Value = nextXp;
            ApplyLevelFromXp(nextXp);
            PublishXpProgress();
        }

        public void AddXp(int amount)
        {
            SetXp(AddClamped(_xp.CurrentValue, amount));
        }

        public void SetCoins(int value)
        {
            ThrowIfDisposed();
            _coins.Value = value;
        }

        public void AddCoins(int amount)
        {
            SetCoins(AddClamped(_coins.CurrentValue, amount));
        }

        public void RemoveCoins(int amount)
        {
            SetCoins(AddClamped(_coins.CurrentValue, -amount));
        }

        public void AddCoinsWithFx(int amount)
        {
            ThrowIfDisposed();
            var normalizedAmount = Math.Max(0, amount);
            feedbackCommands?.RequestCollectFx(normalizedAmount, UiFxTarget.Coins);
            AddCoins(normalizedAmount);
        }

        public void RemoveCoinsWithFx(int amount)
        {
            ThrowIfDisposed();
            var normalizedAmount = Math.Max(0, amount);
            feedbackCommands?.RequestSpendFx(normalizedAmount, UiFxTarget.Coins);
            RemoveCoins(normalizedAmount);
        }

        public void SetLevel(int value)
        {
            ThrowIfDisposed();

            var nextLevel = Clamp(value, InitialLevel, Settings.XpLevels.Count);
            if (_level.CurrentValue == nextLevel)
            {
                return;
            }

            _level.Value = nextLevel;
            PublishXpProgress();
        }

        public void SetName(string value)
        {
            ThrowIfDisposed();
            _name.Value = value ?? string.Empty;
        }

        public int GetCurrentLevelXp()
        {
            ThrowIfDisposed();
            return XpProgress.CurrentValue.CurrentLevelXp;
        }

        public int GetLevelByXp(out int nextLevelXpBound)
        {
            ThrowIfDisposed();
            var progress = CalculateProgress(_xp.CurrentValue, CalculateLevel(_xp.CurrentValue));
            nextLevelXpBound = progress.NextLevelXpBound;
            return progress.Level;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _levelUps.Dispose();
            _xpUpdates.Dispose();
            _xpProgress.Dispose();
            _name.Dispose();
            _level.Dispose();
            _coins.Dispose();
            _xp.Dispose();
            _health.Dispose();
        }

        private void ApplyLevelFromXp(int totalXp)
        {
            var oldLevel = _level.CurrentValue;
            var levelByXp = CalculateLevel(totalXp);
            if (oldLevel > 0 && oldLevel < levelByXp)
            {
                _level.Value = levelByXp;
                _levelUps.OnNext(levelByXp);
            }
        }

        private void PublishXpProgress()
        {
            var progress = CalculateProgress(_xp.CurrentValue, _level.CurrentValue);
            _xpProgress.Value = progress;
            _xpUpdates.OnNext(progress);
        }

        private PlayerXpProgress CalculateProgress(int totalXp, int level)
        {
            var clampedLevel = Clamp(level, InitialLevel, Settings.XpLevels.Count);
            var levelStartXp = GetLevelStartXp(clampedLevel);
            var isMaxLevel = clampedLevel >= Settings.XpLevels.Count;
            var nextLevelXpBound =
                isMaxLevel ? Settings.XpLevels[Settings.XpLevels.Count - 1] : Settings.XpLevels[clampedLevel];
            var currentLevelXp = isMaxLevel ? nextLevelXpBound : Math.Max(0, totalXp - levelStartXp);

            return new PlayerXpProgress(clampedLevel, totalXp, currentLevelXp, nextLevelXpBound, isMaxLevel);
        }

        private int CalculateLevel(int totalXp)
        {
            var accumulatedXp = 0;
            for (var level = InitialLevel; level < Settings.XpLevels.Count; level++)
            {
                var nextLevelXp = Settings.XpLevels[level];
                if (totalXp < AddClamped(accumulatedXp, nextLevelXp))
                {
                    return level;
                }

                accumulatedXp = AddClamped(accumulatedXp, nextLevelXp);
            }

            return Settings.XpLevels.Count;
        }

        private int GetLevelStartXp(int level)
        {
            var accumulatedXp = 0;
            var exclusiveUpperBound = Clamp(level, InitialLevel, Settings.XpLevels.Count);
            for (var i = 0; i < exclusiveUpperBound; i++)
            {
                accumulatedXp = AddClamped(accumulatedXp, Settings.XpLevels[i]);
            }

            return accumulatedXp;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PlayerService));
            }
        }

        private static int AddClamped(int value, int amount)
        {
            var result = (long)value + amount;
            if (result > int.MaxValue)
            {
                return int.MaxValue;
            }

            return result < int.MinValue ? int.MinValue : (int)result;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }
    }
}