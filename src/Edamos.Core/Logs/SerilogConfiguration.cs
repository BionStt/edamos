using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Edamos.Core.Logs
{
    public static class SerilogConfiguration
    {
        public static LoggerConfiguration WriteToRollingFile(this LoggerConfiguration lc, string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));

            return lc.WriteTo.RollingFile(
                Path.Combine("Logs", fileName),
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {EventId} {Message:lj} {Properties}{NewLine}{Exception}");
        }

        public static LoggerConfiguration FilterIncludeOnlyCategory(this LoggerConfiguration lc, string category,
            params LogEventLevel[] excludeLevels)
        {
            return lc.Filter.ByExcluding(e => excludeLevels.Contains(e.Level))
                .Filter.ByIncludingOnly(Matching.FromSource(category));
        }

        public static LoggerConfiguration FilterExcludeLevelsFromCategory(this LoggerConfiguration lc, 
            string category,
            Func<LogEvent,bool> include,
                params LogEventLevel[] excludeLevels)
        {
            return lc.Filter.ByExcluding(
                e => excludeLevels.Contains(e.Level) && Matching.FromSource(category).Invoke(e));
        }

        public static FilterBuilder NewFilterBuilder(this LoggerConfiguration lc)
        {
            return new FilterBuilder(lc);
        }

        public static FilterBuilder AddSource(this FilterBuilder builder, string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            builder.AddCollection.Add(Matching.FromSource(source));

            return builder;
        }

        public static FilterBuilder RemoveLevelsFromSource(this FilterBuilder builder, string source, params LogEventLevel[] levels)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            builder.RemoveCollection.Add(e => Matching.FromSource(source).Invoke(e) && levels.Contains(e.Level));

            return builder;
        }

        public class FilterBuilder
        {
            private readonly LoggerConfiguration _lc;

            public FilterBuilder(LoggerConfiguration lc)
            {
                _lc = lc ?? throw new ArgumentNullException(nameof(lc));
            }

            public readonly List<Func<LogEvent,bool>> RemoveCollection = new List<Func<LogEvent, bool>>();

            public readonly List<Func<LogEvent,bool>> AddCollection = new List<Func<LogEvent, bool>>();

            public LoggerConfiguration BuildExclude()
            {
                _lc.Filter.ByExcluding(e =>
                {
                    bool result = this.RemoveCollection.Any(excf => excf(e));

                    result = result & !this.AddCollection.Any(incf => incf(e));

                    return result;
                });

                return _lc;
            }

            public LoggerConfiguration BuildIncludeOnly()
            {
                _lc.Filter.ByIncludingOnly(e =>
                {
                    bool result = this.AddCollection.Any(incf => incf(e));

                    result = result & !this.RemoveCollection.Any(excf => excf(e));

                    return result;
                });

                return _lc;
            }
        }
    }
}