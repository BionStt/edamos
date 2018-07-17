using Microsoft.Extensions.Logging;

namespace Edamos.AspNetCore
{
    public static class LogEvents
    {
        public static EventId UnhandledException = new EventId(123, nameof(UnhandledException));
    }
}