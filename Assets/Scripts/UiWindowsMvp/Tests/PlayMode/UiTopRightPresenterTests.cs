using NUnit.Framework;
using ProjectContext.Player;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;
using UnityEngine.UI;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiTopRightPresenterTests
    {
        [Test]
        public void Presenter_BindsCoinsAndRegistersFxTargetOnlyWhileShown()
        {
            var viewObject = new GameObject("UiTopRight Test View", typeof(RectTransform), typeof(UiTopRightView));
            var windowObject = new GameObject("UiTopRight Test Window", typeof(RectTransform), typeof(UiTopRightWindow));
            using var service = new PlayerService(PlayerSettings.Default);
            var registry = new UiFxTargetRegistry();
            var firstScope = new WindowPresenterShowScope();
            var secondScope = new WindowPresenterShowScope();

            try
            {
                var view = BuildView(viewObject);
                var window = windowObject.GetComponent<UiTopRightWindow>();
                var presenter = new UiTopRightPresenter(service, registry, _ => view);

                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(firstScope);

                Assert.That(view.TextCoinsAmount.text, Is.EqualTo("0"));
                Assert.That(registry.TryGetTarget(UiFxTarget.Coins, out var target), Is.True);
                Assert.That(target, Is.SameAs(view.CoinIconRectTransform));

                service.SetCoins(125);
                Assert.That(view.TextCoinsAmount.text, Is.EqualTo("125"));

                firstScope.Dispose();
                Assert.That(registry.TryGetTarget(UiFxTarget.Coins, out _), Is.False);

                service.SetCoins(250);
                Assert.That(view.TextCoinsAmount.text, Is.EqualTo("125"));

                presenter.OnShowBegin(secondScope);
                Assert.That(view.TextCoinsAmount.text, Is.EqualTo("250"));
                Assert.That(registry.TryGetTarget(UiFxTarget.Coins, out target), Is.True);
                Assert.That(target, Is.SameAs(view.CoinIconRectTransform));
            }
            finally
            {
                firstScope.Dispose();
                secondScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        private static UiTopRightView BuildView(GameObject viewObject)
        {
            var view = viewObject.GetComponent<UiTopRightView>();
            view.Body = CreateComponent<Image>("Body", viewObject.transform).rectTransform;
            view.ImageIconCoins = CreateComponent<Image>("ImageIconCoins", view.Body);
            view.TextCoinsAmount = CreateComponent<Text>("TextCoinsAmount", view.Body);
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
