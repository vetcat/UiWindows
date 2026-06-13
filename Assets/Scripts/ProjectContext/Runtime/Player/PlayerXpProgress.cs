using System;

namespace ProjectContext.Player
{
    public readonly struct PlayerXpProgress : IEquatable<PlayerXpProgress>
    {
        public PlayerXpProgress(int level, int totalXp, int currentLevelXp, int nextLevelXpBound, bool isMaxLevel)
        {
            Level = level;
            TotalXp = totalXp;
            CurrentLevelXp = currentLevelXp;
            NextLevelXpBound = nextLevelXpBound;
            IsMaxLevel = isMaxLevel;
        }

        public int Level { get; }
        public int TotalXp { get; }
        public int CurrentLevelXp { get; }
        public int DifferenceXp => CurrentLevelXp;
        public int NextLevelXpBound { get; }
        public bool IsMaxLevel { get; }

        public float NormalizedProgress
        {
            get
            {
                if (NextLevelXpBound <= 0)
                {
                    return IsMaxLevel ? 1f : 0f;
                }

                var progress = (float)CurrentLevelXp / NextLevelXpBound;
                if (progress < 0f)
                {
                    return 0f;
                }

                return progress > 1f ? 1f : progress;
            }
        }

        public bool Equals(PlayerXpProgress other)
        {
            return Level == other.Level &&
                   TotalXp == other.TotalXp &&
                   CurrentLevelXp == other.CurrentLevelXp &&
                   NextLevelXpBound == other.NextLevelXpBound &&
                   IsMaxLevel == other.IsMaxLevel;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerXpProgress other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Level;
                hashCode = (hashCode * 397) ^ TotalXp;
                hashCode = (hashCode * 397) ^ CurrentLevelXp;
                hashCode = (hashCode * 397) ^ NextLevelXpBound;
                hashCode = (hashCode * 397) ^ IsMaxLevel.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(PlayerXpProgress left, PlayerXpProgress right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerXpProgress left, PlayerXpProgress right)
        {
            return !left.Equals(right);
        }
    }
}