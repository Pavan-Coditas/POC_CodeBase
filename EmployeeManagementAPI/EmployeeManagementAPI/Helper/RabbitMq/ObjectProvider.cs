namespace EmployeeManagement.Api.Helper.RabbitMq
{
    public class ObjectProvider : IObjectProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ObjectProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetInstance<T>(string key)
        {
            var namedInstance = _serviceProvider.GetServices<T>()
                .FirstOrDefault(instance => instance!.GetType().Name.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (namedInstance == null)
            {
                throw new KeyNotFoundException($"No named instance found for key '{key}'.");
            }

            return namedInstance;
        }

    }
}
