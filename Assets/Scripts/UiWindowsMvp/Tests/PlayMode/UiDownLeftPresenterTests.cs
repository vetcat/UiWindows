using NUnit.Framework;
using ProjectContext.Localization;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiDownLeftPresenterTests
    {
        [Test]
        public void Presenter_BindsShopButtonAndRefreshesLocalizedLabelOnlyWhileShown()
        {
            var viewObject = new GameObject("UiDownLeft Test View", typeof(RectTransform), typeof(UiDownLeftView));
            var windowObject =
                new GameObject("UiDownLeft Test Window", typeof(RectTransform), typeof(UiDownLeftWindow));
            using var localization = new LocalizationService(SystemLanguage.English);
            var firstScope = new WindowPresenterShowScope();
            var secondScope = new WindowPresenterShowScope();
            var openCount = 0;

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiDownLeftWindow>();
                var presenter = new UiDownLeftPresenter(
                    () => openCount++,
                    localization,
                    _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstScope);

                Assert.That(view.TextItemsShop.text, Is.EqualTo("Items Shop"));

                view.ButtonItemsShop.onClick.Invoke();
                Assert.That(openCount, Is.EqualTo(1));

                localization.ChangeLanguage(SystemLanguage.German);
                Assert.That(view.TextItemsShop.text, Is.EqualTo("Gegenstandsladen"));

                firstScope.Dispose();
                view.ButtonItemsShop.onClick.Invoke();
                localization.ChangeLanguage(SystemLanguage.French);

                Assert.That(openCount, Is.EqualTo(1));
                Assert.That(view.TextItemsShop.text, Is.EqualTo("Gegenstandsladen"));

                presenter.OnShowBegin(secondScope);

                Assert.That(view.TextItemsShop.text, Is.EqualTo("Boutique"));

                view.ButtonItemsShop.onClick.Invoke();
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

        internal static UiDownLeftView BuildView(GameObject viewObject)
        {
            var view = viewObject.GetComponent<UiDownLeftView>();
            return UiViewTestFixtures.Configure(view);
        }
    }
}
