using System.Xml.Linq;

namespace BuzzerWolf.BBAPI
{
    public class BBAPIResponse
    {
        public bool IsSuccess { get; init; }
        public string? Error { get; init; }
        public XElement Response { get; init; }
    }
}
