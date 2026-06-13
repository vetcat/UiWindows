using R3;

namespace ProjectContext.Player
{
    public interface IPlayerReadModel
    {
        ReadOnlyReactiveProperty<int> Health { get; }
        ReadOnlyReactiveProperty<int> Xp { get; }
        ReadOnlyReactiveProperty<int> Coins { get; }
        ReadOnlyReactiveProperty<int> Level { get; }
        ReadOnlyReactiveProperty<string> Name { get; }
        ReadOnlyReactiveProperty<PlayerXpProgress> XpProgress { get; }
        Observable<PlayerXpProgress> XpUpdates { get; }
        Observable<int> LevelUps { get; }
    }
}