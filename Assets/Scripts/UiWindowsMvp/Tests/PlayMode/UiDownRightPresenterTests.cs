using NUnit.Framework;
using ProjectContext.Localization;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiDownRightPresenterTests
    {
        [Test]
        public void Presenter_BindsSettingsButtonAndRefreshesLocalizedLabelOnlyWhileShown()
        {
            var viewObject = new GameObject("UiDownRight Test View", typeof(RectTransform), typeof(UiDownRightView));
            var windowObject =
                new GameObject("UiDownRight Test Window", typeof(RectTransform), typeof(UiDownRightWindow));
            using var localization = new LocalizationService(SystemLanguage.English);
            var firstScope = new WindowPresenterShowScope();
            var secondScope = new WindowPresenterShowScope();
            var openCount = 0;

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiDownRightWindow>();
                var presenter = new UiDownRightPresenter(
                    () => openCount++,
                    localization,
                    _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstScope);

                Assert.That(view.TextSettings.text, Is.EqualTo("Settings"));

                view.ButtonSettings.onClick.Invoke();
                Assert.That(openCount, Is.EqualTo(1));

                localization.ChangeLanguage(SystemLanguage.German);
                Assert.That(view.TextSettings.text, Is.EqualTo("die Einstellungen"));

                firstScope.Dispose();
                view.ButtonSettings.onClick.Invoke();
                localization.ChangeLanguage(SystemLanguage.French);

                Assert.That(openCount, Is.EqualTo(1));
                Assert.That(view.TextSettings.text, Is.EqualTo("die Einstellungen"));

                presenter.OnShowBegin(secondScope);

                Assert.That(view.TextSettings.text, Is.EqualTo("Paramètres"));

                view.ButtonSettings.onClick.Invoke();
                Assert.That(openCount, Is.EqualTo(2));
            }
            finally
            {
                firstScope.Dispose();
                secondScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        internal static UiDownRightView BuildView(GameObject viewObject)
        {
            var view = viewObject.GetComponent<UiDownRightView>();
            view.Body = CreateComponent<Image>("Body", viewObject.transform).rectTransform;
            view.ButtonSettings = view.Body.gameObject.AddComponent<Button>();
            view.ButtonSettings.targetGraphic = view.Body.GetComponent<Image>();
            view.TextSettings = CreateComponent<Text>("TextSettings", view.Body);
            view.ImageSettings = CreateComponent<Image>("ImageSettings", view.Body);
            return view;
        }

        private static T CreateComponent<T>(string name, Transform parent)
            where T : Component
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(T));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<T>();
        }
    }
}
