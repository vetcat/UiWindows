using NUnit.Framework;
using ProjectContext.Player;
using System.Reflection;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiTopLeftPresenterTests
    {
        [Test]
        public void Presenter_BindsReadModelAndRoutesHealthAndXpCommandsThroughPorts()
        {
            var viewObject = new GameObject("UiTopLeft Test View", typeof(RectTransform), typeof(UiTopLeftView));
            var windowObject = new GameObject("UiTopLeft Test Window", typeof(RectTransform), typeof(UiTopLeftWindow));
            using var service = new PlayerService(PlayerSettings.Default);
            using var showScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiTopLeftWindow>();
                var presenter = new UiTopLeftPresenter(
                    service,
                    service,
                    PlayerSettings.Default,
                    UiTopLeftPresenter.DefaultHealthCommandStep,
                    UiTopLeftPresenter.DefaultXpCommandStep,
                    _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);

                Assert.That(view.TextPlayerName.text, Is.EqualTo(PlayerService.DefaultPlayerName));
                Assert.That(view.TextLevelValue.text, Is.EqualTo(PlayerService.InitialLevel.ToString()));
                Assert.That(view.HealthData.TextValue.text, Is.EqualTo("100 / 100"));
                Assert.That(view.HealthData.Slider.value, Is.EqualTo(100f));
                Assert.That(view.XpData.TextValue.text, Is.EqualTo("0 XP (0 / 100)"));

                view.HealthData.ButtonReduce.onClick.Invoke();
                view.XpData.ButtonAdd.onClick.Invoke();

                Assert.That(GetCurrentValue<int>(service, "Health"), Is.EqualTo(90));
                Assert.That(GetCurrentValue<int>(service, "Xp"), Is.EqualTo(25));
                Assert.That(view.HealthData.TextValue.text, Is.EqualTo("90 / 100"));
                Assert.That(view.XpData.TextValue.text, Is.EqualTo("25 XP (25 / 100)"));
                Assert.That(view.XpData.Slider.value, Is.EqualTo(25f));
            }
            finally
            {
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void Presenter_DisposesShowScopeButtonHandlersOnHideScopeCleanup()
        {
            var viewObject = new GameObject("UiTopLeft Test View", typeof(RectTransform), typeof(UiTopLeftView));
            var windowObject = new GameObject("UiTopLeft Test Window", typeof(RectTransform), typeof(UiTopLeftWindow));
            using var service = new PlayerService(PlayerSettings.Default);
            var showScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiTopLeftWindow>();
                var presenter = new UiTopLeftPresenter(
                    service,
                    service,
                    PlayerSettings.Default,
                    UiTopLeftPresenter.DefaultHealthCommandStep,
                    UiTopLeftPresenter.DefaultXpCommandStep,
                    _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);
                showScope.Dispose();

                view.HealthData.ButtonReduce.onClick.Invoke();
                service.SetHealth(50);

                Assert.That(GetCurrentValue<int>(service, "Health"), Is.EqualTo(50));
                Assert.That(view.HealthData.TextValue.text, Is.EqualTo("100 / 100"));
            }
            finally
            {
                showScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        private static UiTopLeftView BuildView(GameObject viewObject)
        {
            var view = viewObject.GetComponent<UiTopLeftView>();
            view.TextPlayerName = CreateText("Player Name", viewObject.transform);
            view.TextLevel = CreateText("Level Label", viewObject.transform);
            view.TextLevelValue = CreateText("Level Value", viewObject.transform);
            view.HealthData = CreatePlayerDataLayout("Health", viewObject.transform);
            view.XpData = CreatePlayerDataLayout("XP", viewObject.transform);
            return view;
        }

        private static PlayerDataLayout CreatePlayerDataLayout(string name, Transform parent)
        {
            return new PlayerDataLayout
            {
                TextValue = CreateText(name + " Text", parent),
                Slider = CreateComponent<Slider>(name + " Slider", parent),
                ButtonAdd = CreateComponent<Button>(name + " Add", parent),
                ButtonReduce = CreateComponent<Button>(name + " Reduce", parent)
            };
        }

        private static Text CreateText(string name, Transform parent)
        {
            return CreateComponent<Text>(name, parent);
        }

        private static T CreateComponent<T>(string name, Transform parent)
            where T : Component
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(T));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<T>();
        }

        private static T GetCurrentValue<T>(PlayerService service, string propertyName)
        {
            var property = typeof(PlayerService).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null);
            var reactiveSurface = property.GetValue(service);
            var currentValueProperty =
                reactiveSurface.GetType().GetProperty("CurrentValue", BindingFlags.Public | BindingFlags.Instance);
            Assert.That(currentValueProperty, Is.Not.Null);
            return (T)currentValueProperty.GetValue(reactiveSurface);
        }
    }
}