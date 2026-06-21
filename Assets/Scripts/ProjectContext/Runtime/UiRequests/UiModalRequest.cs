using System;

namespace ProjectContext.UiRequests
{
    public sealed class UiModalRequest
    {
        internal UiModalRequest(
            int id,
            UiModalKind kind,
            string caption,
            string description,
            Action okHandler,
            Action cancelHandler,
            Action closeHandler)
        {
            Id = id;
            Kind = kind;
            Caption = caption ?? string.Empty;
            Description = description ?? string.Empty;
            OkHandler = okHandler;
            CancelHandler = cancelHandler;
            CloseHandler = closeHandler;
        }

        public int Id { get; }
        public UiModalKind Kind { get; }
        public string Caption { get; }
        public string Description { get; }

        internal Action OkHandler { get; }
        internal Action CancelHandler { get; }
        internal Action CloseHandler { get; }
    }
}
