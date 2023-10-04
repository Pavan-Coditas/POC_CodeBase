using EmployeeManagment.Services.CorrelationId;

namespace EmployeeManagement.Api.Helper.Handlers
{

    public class RequestHandler : DelegatingHandler
    {
        private readonly ICorrelationIdGenerator _correlationIdAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestHandler(ICorrelationIdGenerator correlationIdAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _correlationIdAccessor = correlationIdAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("CorrelationId", _correlationIdAccessor.Get()); // Getting correlationid from request context. 
            return base.SendAsync(request, cancellationToken);
        }
    }
}
