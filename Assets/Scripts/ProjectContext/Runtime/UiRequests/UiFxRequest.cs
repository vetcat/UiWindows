namespace ProjectContext.UiRequests
{
    public readonly struct UiFxRequest
    {
        public UiFxRequest(UiFxKind kind, int amount, UiFxTarget target)
        {
            Kind = kind;
            Amount = amount < 0 ? 0 : amount;
            Target = target;
        }

        public UiFxKind Kind { get; }
        public int Amount { get; }
        public UiFxTarget Target { get; }
    }
}
