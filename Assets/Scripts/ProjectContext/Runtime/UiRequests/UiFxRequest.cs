namespace ProjectContext.UiRequests
{
    public readonly struct UiFxRequest
    {
        public UiFxRequest(UiFxKind kind, int amount, UiFxTarget target)
            : this(kind, amount, null, target)
        {
        }

        public UiFxRequest(UiFxKind kind, int amount, UiFxTarget source, UiFxTarget target)
            : this(kind, amount, (UiFxTarget?)source, target)
        {
        }

        private UiFxRequest(UiFxKind kind, int amount, UiFxTarget? source, UiFxTarget target)
        {
            Kind = kind;
            Amount = amount < 0 ? 0 : amount;
            Source = source;
            Target = target;
        }

        public UiFxKind Kind { get; }
        public int Amount { get; }
        public UiFxTarget? Source { get; }
        public UiFxTarget Target { get; }
    }
}
