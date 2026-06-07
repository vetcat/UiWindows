namespace UiWindowsMvp.Runtime.Composition
{
    public interface IServiceRegistry : IServiceResolver
    {
        void Register<TService>(TService service) where TService : class;
    }
}
