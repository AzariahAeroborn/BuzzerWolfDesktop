namespace BuzzerWolf.BBAPI.Exceptions
{
    public class UnexpectedResponseException : System.Exception
    {
        public UnexpectedResponseException() { }
        public UnexpectedResponseException(string message) : base(message) { }
    }
}
