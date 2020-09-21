using Newtonsoft.Json.Converters;

namespace GameSessionFeedback.Utils
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}