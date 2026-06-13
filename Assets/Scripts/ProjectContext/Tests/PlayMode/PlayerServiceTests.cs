using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace ProjectContext.Player.Tests.PlayMode
{
    public sealed class PlayerServiceTests
    {
        [Test]
        public void Constructor_UsesOpenUiPlayerDefaults()
        {
            using var service = new PlayerService(CreateSettings());

            Assert.That(GetCurrentValue<int>(service, "Health"), Is.EqualTo(100));
            Assert.That(GetCurrentValue<int>(service, "Xp"), Is.EqualTo(0));
            Assert.That(GetCurrentValue<int>(service, "Coins"), Is.EqualTo(0));
            Assert.That(GetCurrentValue<int>(service, "Level"), Is.EqualTo(1));
            Assert.That(GetCurrentValue<string>(service, "Name"), Is.EqualTo(PlayerService.DefaultPlayerName));
            Assert.That(GetCurrentValue<PlayerXpProgress>(service, "XpProgress"),
                Is.EqualTo(new PlayerXpProgress(1, 0, 0, 100, false)));
        }

        [Test]
        public void HealthCommands_ClampToSettingsBounds()
        {
            using var service = new PlayerService(CreateSettings());

            service.SetHealth(150);
            Assert.That(GetCurrentValue<int>(service, "Health"), Is.EqualTo(100));

            service.SetHealth(-10);
            Assert.That(GetCurrentValue<int>(service, "Health"), Is.EqualTo(0));

            service.AddHealth(25);
            Assert.That(GetCurrentValue<int>(service, "Health"), Is.EqualTo(25));
        }

        [Test]
        public void Commands_UpdateObservableReadModelState()
        {
            using var service = new PlayerService(CreateSettings());
            IPlayerCommands commands = service;
            var healthValues = new List<int>();
            var coinValues = new List<int>();
            var names = new List<string>();

            using var healthSubscription = Subscribe<int>(service, "Health", healthValues.Add);
            using var coinSubscription = Subscribe<int>(service, "Coins", coinValues.Add);
            using var nameSubscription = Subscribe<string>(service, "Name", names.Add);

            commands.SetHealth(40);
            commands.AddCoins(25);
            commands.RemoveCoins(10);
            commands.SetName("Vitaly");

            Assert.That(healthValues, Is.EqualTo(new[] { 100, 40 }));
            Assert.That(coinValues, Is.EqualTo(new[] { 0, 25, 15 }));
            Assert.That(names, Is.EqualTo(new[] { PlayerService.DefaultPlayerName, "Vitaly" }));
        }

        [Test]
        public void SetXp_PublishesProgressAndLevelUps()
        {
            using var service = new PlayerService(CreateSettings());
            var levelUps = new List<int>();
            var xpUpdates = new List<PlayerXpProgress>();

            using var levelUpSubscription = Subscribe<int>(service, "LevelUps", levelUps.Add);
            using var xpUpdateSubscription = Subscribe<PlayerXpProgress>(service, "XpUpdates", xpUpdates.Add);

            service.SetXp(50);
            Assert.That(GetCurrentValue<int>(service, "Level"), Is.EqualTo(1));
            Assert.That(GetCurrentValue<PlayerXpProgress>(service, "XpProgress"),
                Is.EqualTo(new PlayerXpProgress(1, 50, 50, 100, false)));
            Assert.That(levelUps, Is.Empty);

            service.SetXp(100);
            Assert.That(GetCurrentValue<int>(service, "Level"), Is.EqualTo(2));
            Assert.That(GetCurrentValue<PlayerXpProgress>(service, "XpProgress"),
                Is.EqualTo(new PlayerXpProgress(2, 100, 0, 200, false)));

            service.SetXp(300);
            Assert.That(GetCurrentValue<int>(service, "Level"), Is.EqualTo(3));
            Assert.That(GetCurrentValue<PlayerXpProgress>(service, "XpProgress"),
                Is.EqualTo(new PlayerXpProgress(3, 300, 0, 300, false)));

            Assert.That(levelUps, Is.EqualTo(new[] { 2, 3 }));
            Assert.That(xpUpdates, Is.EqualTo(new[]
            {
                new PlayerXpProgress(1, 50, 50, 100, false),
                new PlayerXpProgress(2, 100, 0, 200, false),
                new PlayerXpProgress(3, 300, 0, 300, false)
            }));
        }

        [Test]
        public void GetLevelByXp_ComputesCurrentProgressFromConfiguredLevelBounds()
        {
            using var service = new PlayerService(CreateSettings());

            service.SetXp(250);

            var level = service.GetLevelByXp(out var nextLevelXpBound);

            Assert.That(level, Is.EqualTo(2));
            Assert.That(nextLevelXpBound, Is.EqualTo(200));
            Assert.That(service.GetCurrentLevelXp(), Is.EqualTo(150));
            Assert.That(GetCurrentValue<PlayerXpProgress>(service, "XpProgress").NormalizedProgress,
                Is.EqualTo(0.75f).Within(0.0001f));
        }

        [Test]
        public void SetLevel_ClampsToConfiguredLevelRangeAndRefreshesProgress()
        {
            using var service = new PlayerService(CreateSettings());
            var xpUpdates = new List<PlayerXpProgress>();

            using var xpUpdateSubscription = Subscribe<PlayerXpProgress>(service, "XpUpdates", xpUpdates.Add);

            service.SetLevel(99);
            Assert.That(GetCurrentValue<int>(service, "Level"), Is.EqualTo(5));
            Assert.That(GetCurrentValue<PlayerXpProgress>(service, "XpProgress"),
                Is.EqualTo(new PlayerXpProgress(5, 0, 400, 400, true)));

            service.SetLevel(-10);
            Assert.That(GetCurrentValue<int>(service, "Level"), Is.EqualTo(1));
            Assert.That(GetCurrentValue<PlayerXpProgress>(service, "XpProgress"),
                Is.EqualTo(new PlayerXpProgress(1, 0, 0, 100, false)));

            Assert.That(xpUpdates, Is.EqualTo(new[]
            {
                new PlayerXpProgress(5, 0, 400, 400, true),
                new PlayerXpProgress(1, 0, 0, 100, false)
            }));
        }

        [Test]
        public void DisposingSubscription_StopsReadModelUpdatesForThatSubscriber()
        {
            using var service = new PlayerService(CreateSettings());
            var observedHealth = new List<int>();

            var subscription = Subscribe<int>(service, "Health", observedHealth.Add);
            subscription.Dispose();

            service.SetHealth(50);

            Assert.That(observedHealth, Is.EqualTo(new[] { 100 }));
        }

        [Test]
        public void Dispose_IsIdempotentAndRejectsFurtherCommands()
        {
            var service = new PlayerService(CreateSettings());

            service.Dispose();
            service.Dispose();

            Assert.Throws<ObjectDisposedException>(() => service.SetHealth(50));
        }

        private static PlayerSettings CreateSettings()
        {
            return new PlayerSettings(100, new[] { 0, 100, 200, 300, 400 });
        }

        private static T GetCurrentValue<T>(PlayerService service, string propertyName)
        {
            var property = GetReactiveSurface(service, propertyName);
            var currentValueProperty =
                property.GetType().GetProperty("CurrentValue", BindingFlags.Public | BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null, $"{propertyName} must expose CurrentValue.");
            return (T)currentValueProperty.GetValue(property);
        }

        private static IDisposable Subscribe<T>(PlayerService service, string propertyName, Action<T> onNext)
        {
            var observable = GetReactiveSurface(service, propertyName);
            var subscribeExtensions = observable.GetType().Assembly.GetType("R3.ObservableSubscribeExtensions");
            Assert.That(subscribeExtensions, Is.Not.Null, "R3 ObservableSubscribeExtensions must be available.");

            foreach (var method in subscribeExtensions.GetMethods(BindingFlags.Public | BindingFlags.Static))
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

        private static object GetReactiveSurface(PlayerService service, string propertyName)
        {
            var property = typeof(PlayerService).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null, $"{propertyName} must be a public player read-model property.");
            return property.GetValue(service);
        }
    }
}