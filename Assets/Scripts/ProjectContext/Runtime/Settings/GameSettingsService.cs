using System;
using R3;
using UnityEngine;

namespace ProjectContext.Settings
{
    public sealed class GameSettingsService : IGameSettingsService, IDisposable
    {
        public const string MusicVolumeKey = "MusicVolume";
        public const string SoundVolumeKey = "SoundVolume";
        public const float DefaultVolume = 1f;

        private readonly ReactiveProperty<float> musicVolume;
        private readonly ReactiveProperty<float> soundVolume;
        private bool disposed;

        public GameSettingsService()
            : this(
                ReadVolume(MusicVolumeKey, DefaultVolume),
                ReadVolume(SoundVolumeKey, DefaultVolume))
        {
        }

        public GameSettingsService(float initialMusicVolume, float initialSoundVolume)
        {
            musicVolume = new ReactiveProperty<float>(Clamp01(initialMusicVolume));
            soundVolume = new ReactiveProperty<float>(Clamp01(initialSoundVolume));
        }

        public ReadOnlyReactiveProperty<float> MusicVolume => musicVolume;
        public ReadOnlyReactiveProperty<float> SoundVolume => soundVolume;

        public void SetMusicVolume(float value)
        {
            ThrowIfDisposed();
            SetVolume(musicVolume, MusicVolumeKey, value);
        }

        public void SetSoundVolume(float value)
        {
            ThrowIfDisposed();
            SetVolume(soundVolume, SoundVolumeKey, value);
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            soundVolume.Dispose();
            musicVolume.Dispose();
        }

        private static void SetVolume(ReactiveProperty<float> target, string key, float value)
        {
            var nextValue = Clamp01(value);
            target.Value = nextValue;
            PlayerPrefs.SetFloat(key, nextValue);
            PlayerPrefs.Save();
        }

        private static float ReadVolume(string key, float fallback)
        {
            return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : fallback;
        }

        private static float Clamp01(float value)
        {
            if (float.IsNaN(value))
            {
                return 0f;
            }

            if (value < 0f)
            {
                return 0f;
            }

            return value > 1f ? 1f : value;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(GameSettingsService));
            }
        }
    }
}