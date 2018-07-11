using App.Metrics;
using App.Metrics.Timer;

namespace Edamos.Core.Cache
{
    public static class CacheMetrics
    {
        public static TimerOptions ReadTimer { get; } =
            new TimerOptions
            {
                Name = "r_time",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Milliseconds,
                Context = "Cache"               
            };

        public static TimerOptions WriteTimer { get; } =
            new TimerOptions
            {
                Name = "w_time",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Milliseconds,
                Context = "Cache"
            };

        public static TimerOptions FactoryTimer { get; } =
            new TimerOptions
            {
                Name = "f_time",
                MeasurementUnit = Unit.Requests,
                RateUnit = TimeUnit.Milliseconds,
                Context = "Cache"
            };
    }
}