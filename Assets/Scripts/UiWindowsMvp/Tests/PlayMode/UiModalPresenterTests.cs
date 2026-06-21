using NUnit.Framework;
using ProjectContext.UiRequests;
using UiWindowsMvp.SampleSceneWindows;
using UiWindowsMvp.UIAdapter;
using UnityEngine;

namespace UiWindowsMvp.Tests.PlayMode
{
    public sealed class UiModalPresenterTests
    {
        [Test]
        public void Presenter_BindsModalRequestAndCompletesThroughCommands()
        {
            var viewObject = new GameObject("UiModal Test View", typeof(RectTransform), typeof(UiModalView));
            var windowObject = new GameObject("UiModal Test Window", typeof(RectTransform), typeof(UiModalWindow));
            using var modal = new UiModalService();
            using var showScope = new WindowPresenterShowScope();
            var okCalls = 0;
            var cancelCalls = 0;

            try
            {
                var view = viewObject.GetComponent<UiModalView>();
                var window = windowObject.GetComponent<UiModalWindow>();
                var presenter = new UiModalPresenter(modal, modal, _ => view);

                modal.ShowInfoOkCancel("Delete item?", "This cannot be undone.", () => okCalls++, () => cancelCalls++);
                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);

                Assert.That(view.TextCaption.text, Is.EqualTo("Delete item?"));
                Assert.That(view.TextDescription.text, Is.EqualTo("This cannot be undone."));

                view.ButtonOk.onClick.Invoke();

                Assert.That(okCalls, Is.EqualTo(1));
                Assert.That(cancelCalls, Is.Zero);
                Assert.That(modal.CurrentRequest, Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }

        [Test]
        public void Presenter_DisposesShowScopeHandlersAndReadModelSubscription()
        {
            var viewObject = new GameObject("UiModal Test View", typeof(RectTransform), typeof(UiModalView));
            var windowObject = new GameObject("UiModal Test Window", typeof(RectTransform), typeof(UiModalWindow));
            using var modal = new UiModalService();
            var showScope = new WindowPresenterShowScope();
            var closeCalls = 0;

            try
            {
                var view = viewObject.GetComponent<UiModalView>();
                var window = windowObject.GetComponent<UiModalWindow>();
                var presenter = new UiModalPresenter(modal, modal, _ => view);

                modal.ShowInfoOk("First", "One", () => closeCalls++);
                presenter.Bind(window);
                presenter.Initialize();
                presenter.OnShowBegin(showScope);
                showScope.Dispose();

                modal.ShowInfoOk("Second", "Two", () => closeCalls++);
                view.ButtonOk.onClick.Invoke();

                Assert.That(view.TextCaption.text, Is.EqualTo("First"));
                Assert.That(closeCalls, Is.Zero);
                Assert.That(modal.CurrentRequest.Caption, Is.EqualTo("Second"));
            }
            finally
            {
                showScope.Dispose();
                Object.DestroyImmediate(windowObject);
                Object.DestroyImmediate(viewObject);
            }
        }
    }
}