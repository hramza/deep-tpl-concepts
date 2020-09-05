namespace Hra.Framework.Web.Domain.Http
{
    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }
}
