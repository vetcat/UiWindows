namespace UiWindowsMvp.Runtime.Composition
{
    public interface IServiceResolver
    {
        TService Resolve<TService>() where TService : class;

        bool TryResolve<TService>(out TService service) where TService : class;
    }
}
