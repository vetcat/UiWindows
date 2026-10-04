namespace CompositionRoot.Runtime
{
    /// <summary>
    /// Starts a registered service after the scene's active objects have completed Awake.
    /// </summary>
    public interface IStartable
    {
        void Start();
    }
}
