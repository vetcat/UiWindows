using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProjectContext.Localization;
using ProjectContext.Settings;
using UnityEngine;

namespace ProjectContext.Player.Tests.PlayMode
{
    public sealed class SettingsLocalizationServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey(GameSettingsService.MusicVolumeKey);
            PlayerPrefs.DeleteKey(GameSettingsService.SoundVolumeKey);
            PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey(GameSettingsService.MusicVolumeKey);
            PlayerPrefs.DeleteKey(GameSettingsService.SoundVolumeKey);
            PlayerPrefs.DeleteKey(LocalizationService.LanguageKey);
        }

        [Test]
        public void SettingsCommands_ClampPersistAndPublishReadOnlyState()
        {
            using var service = new GameSettingsService(0.25f, 0.75f);
            var musicValues = new List<float>();
            var soundValues = new List<float>();

            using var musicSubscription = Subscribe<float>(service, "MusicVolume", musicValues.Add);
            using var soundSubscription = Subscribe<float>(service, "SoundVolume", soundValues.Add);

            service.SetMusicVolume(1.5f);
            service.SetSoundVolume(float.NaN);

            Assert.That(GetCurrentValue<float>(service, "MusicVolume"), Is.EqualTo(1f));
            Assert.That(GetCurrentValue<float>(service, "SoundVolume"), Is.EqualTo(0f));
            Assert.That(PlayerPrefs.GetFloat(GameSettingsService.MusicVolumeKey), Is.EqualTo(1f));
            Assert.That(PlayerPrefs.GetFloat(GameSettingsService.SoundVolumeKey), Is.EqualTo(0f));
            Assert.That(musicValues, Is.EqualTo(new[] { 0.25f, 1f }));
            Assert.That(soundValues, Is.EqualTo(new[] { 0.75f, 0f }));
        }

        [Test]
        public void LocalizationCommands_PublishCurrentLanguageAndTranslateVisibleKeys()
        {
            using var service = new LocalizationService(SystemLanguage.English);
            var changedLanguages = new List<SystemLanguage>();

            using var languageSubscription =
                Subscribe<SystemLanguage>(service, "CurrentLanguage", changedLanguages.Add);
            using var changeEventSubscription =
                Subscribe<SystemLanguage>(service, "LanguageChanged", changedLanguages.Add);

            service.ChangeLanguage(SystemLanguage.French);

            Assert.That(GetCurrentValue<SystemLanguage>(service, "CurrentLanguage"), Is.EqualTo(SystemLanguage.French));
            Assert.That(service.Translate("Settings"), Is.EqualTo("Paramètres"));
            Assert.That(service.Translate("TextMusic"), Is.EqualTo("MUSIQUE"));
            Assert.That(PlayerPrefs.GetInt(LocalizationService.LanguageKey), Is.EqualTo((int)SystemLanguage.French));
            Assert.That(changedLanguages, Is.EqualTo(new[]
            {
                SystemLanguage.English,
                SystemLanguage.French,
                SystemLanguage.French
            }));
        }

        [Test]
        public void Dispose_IsIdempotentAndRejectsFurtherCommands()
        {
            var settings = new GameSettingsService();
            var localization = new LocalizationService(SystemLanguage.English);

            settings.Dispose();
            settings.Dispose();
            localization.Dispose();
            localization.Dispose();

            Assert.Throws<ObjectDisposedException>(() => settings.SetMusicVolume(0.5f));
            Assert.Throws<ObjectDisposedException>(() => localization.ChangeLanguage(SystemLanguage.German));
        }

        private static T GetCurrentValue<T>(object service, string propertyName)
        {
            var property = GetReactiveSurface(service, propertyName);
            var currentValueProperty =
                property.GetType().GetProperty("CurrentValue",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null, $"{propertyName} must expose CurrentValue.");
            return (T)currentValueProperty.GetValue(property);
        }

        private static IDisposable Subscribe<T>(object service, string propertyName, Action<T> onNext)
        {
            var observable = GetReactiveSurface(service, propertyName);
            var subscribeExtensions = observable.GetType().Assembly.GetType("R3.ObservableSubscribeExtensions");
            Assert.That(subscribeExtensions, Is.Not.Null, "R3 ObservableSubscribeExtensions must be available.");

            foreach (var method in subscribeExtensions.GetMethods(System.Reflection.BindingFlags.Public |
                                                                  System.Reflection.BindingFlags.Static))
            {
                if (!method.IsGenericMethodDefinition || method.Name != "Subscribe")
                {
                    continue;
                }

                var parameters = method.GetParameters();
                if (method.GetGenericArguments().Length == 1 &&
                    parameters.Length == 2 &&
                    parameters[1].ParameterType.IsGenericType &&
                    parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>))
                {
                    return (IDisposable)method.MakeGenericMethod(typeof(T))
                        .Invoke(null, new object[] { observable, onNext });
                }
            }

            Assert.Fail("R3 Subscribe<T>(Observable<T>, Action<T>) overload was not found.");
            return null;
        }

        private static object GetReactiveSurface(object service, string propertyName)
        {
            var property = service.GetType().GetProperty(propertyName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.That(property, Is.Not.Null, $"{propertyName} must be a public read-model property.");
            return property.GetValue(service);
        }
    }
}