using System;

namespace ProjectContext.UiRequests
{
    public interface IUiModalCommands
    {
        void ShowInfoOk(string caption, string description, Action handlerClose = null);
        void ShowInfoOkCancel(string caption, string description, Action handlerOk = null, Action handlerCancel = null);
        void ShowWait(string caption = "");
        void HideWait();
        void CompleteCurrent(UiModalResult result);
        void Clear();
    }
}
