using R3;

namespace ProjectContext.Settings
{
    public interface IGameSettingsReadModel
    {
        ReadOnlyReactiveProperty<float> MusicVolume { get; }
        ReadOnlyReactiveProperty<float> SoundVolume { get; }
    }
}