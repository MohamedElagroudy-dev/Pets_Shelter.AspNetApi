namespace API.Helper
{
    public class ResponseAPI
    {
        public ResponseAPI(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetMessageFromStatusCode(statusCode);
        }

        private static string GetMessageFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Done",
                400 => "Bad Request",
                401 => "Un Authorized",
                404 => "Resource Not Found",
                500 => "Server Error",
                _ => "Unknown"
            };
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class ResponseAPI<T> : ResponseAPI
    {
        public ResponseAPI(int statusCode, string? message = null, T? data = default)
            : base(statusCode, message)
        {
            Data = data;
        }

        public T? Data { get; set; }
    }
}
