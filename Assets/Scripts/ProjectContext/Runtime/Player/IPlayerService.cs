namespace ProjectContext.Player
{
    public interface IPlayerService : IPlayerReadModel, IPlayerCommands
    {
        IPlayerSettings Settings { get; }
        int GetCurrentLevelXp();
        int GetLevelByXp(out int nextLevelXpBound);
    }
}