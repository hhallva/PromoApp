namespace DataLayer.Models
{
    public class Response
    {
        public long Timestamp { get; }

        public string Message { get; }

        public int ErrorCode { get; }

        public Response(long timestamp, string message, int errorCode)
        {
            Timestamp = timestamp;
            Message = message;
            ErrorCode = errorCode;
        }

        public Response(string message, int errorCode) :
            this(DateTimeOffset.UtcNow.ToUnixTimeSeconds(), message, errorCode)
        {
        }
    }
}