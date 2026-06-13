using System;
using System.Collections.Generic;

namespace ProjectContext.Player
{
    public sealed class PlayerSettings : IPlayerSettings
    {
        public static readonly PlayerSettings Default = new(100, new[] { 0, 100, 200, 300, 400 });

        private readonly int[] _xpLevels;

        public int MaxHealth { get; }
        public IReadOnlyList<int> XpLevels => _xpLevels;

        public PlayerSettings(int maxHealth, IReadOnlyList<int> xpLevels)
        {
            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth), maxHealth, "Max health must be positive.");
            }

            if (xpLevels == null)
            {
                throw new ArgumentNullException(nameof(xpLevels));
            }

            if (xpLevels.Count == 0)
            {
                throw new ArgumentException("At least one XP level must be configured.", nameof(xpLevels));
            }

            _xpLevels = new int[xpLevels.Count];
            for (var i = 0; i < xpLevels.Count; i++)
            {
                var xpLevel = xpLevels[i];
                if (xpLevel < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(xpLevels), xpLevel, "XP levels must not be negative.");
                }

                _xpLevels[i] = xpLevel;
            }

            if (_xpLevels[0] != 0)
            {
                throw new ArgumentException("XP level index 0 must be 0 for the starting level baseline.",
                    nameof(xpLevels));
            }

            MaxHealth = maxHealth;
        }
    }
}