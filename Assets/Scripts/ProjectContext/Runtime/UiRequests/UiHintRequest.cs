namespace ProjectContext.UiRequests
{
    public readonly struct UiHintRequest
    {
        public UiHintRequest(string description, UiHintAnchor anchor = UiHintAnchor.Center, float durationSeconds = 1.5f)
        {
            Description = description ?? string.Empty;
            Anchor = anchor;
            DurationSeconds = durationSeconds < 0f ? 0f : durationSeconds;
        }

        public string Description { get; }
        public UiHintAnchor Anchor { get; }
        public float DurationSeconds { get; }
    }
}
