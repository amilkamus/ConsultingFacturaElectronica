using log4net.Config;
using log4net.Repository;
using System;
using System.Reflection;

namespace FactElec.Log
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ConfiguracionAttribute : ConfiguratorAttribute
    {
        private const string conversionPattern = "%date{dd-MM-yyyy HH:mm:ss,fff} %-5level %thread %logger %method %message %exception %newline";
        private string NombreArchivo { get; set; }

        public ConfiguracionAttribute(string nombreArchivo)
            : base(0) => this.NombreArchivo = nombreArchivo;

        public override void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository)
        {
            var patternLayout = new log4net.Layout.PatternLayout
            {
                ConversionPattern = conversionPattern
            };
            patternLayout.ActivateOptions();

            var roller = new log4net.Appender.RollingFileAppender
            {
                AppendToFile = true,
                File = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logger"),
                Layout = patternLayout,
                RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date,
                DatePattern = $"'/'yy'/'MM'/'dd'/{NombreArchivo}_'yyyyMMdd'.log'",
                StaticLogFileName = false,
                LockingModel = new log4net.Appender.FileAppender.MinimalLock()
            };
            roller.ActivateOptions();

            var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)targetRepository;
            hierarchy.Root.AddAppender(roller);
            hierarchy.Root.Level = log4net.Core.Level.Info;
            hierarchy.Configured = true;
        }
    }
}
