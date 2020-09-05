using System.Collections.Generic;
using System.Linq;

namespace Hra.Framework.Web.Domain.Http
{
    public class ApiResponse
    {
        public ApiResponse() => Errors = new List<ApiError>();

        public bool Success => Errors?.Any() == false;

        public IEnumerable<ApiError> Errors { get; set; }
    }
}
