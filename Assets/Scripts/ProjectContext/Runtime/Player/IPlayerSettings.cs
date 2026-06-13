using System.Collections.Generic;

namespace ProjectContext.Player
{
    public interface IPlayerSettings
    {
        int MaxHealth { get; }
        IReadOnlyList<int> XpLevels { get; }
    }
}