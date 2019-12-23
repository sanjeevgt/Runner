//using Microsoft.Extensions.Configuration;
//using Serilog;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Reflection;
//using System.Text;


//namespace LogLogging
//{
//    public class Imp : ILogger
//    {
//        IConfigurationSection connectionStringloggingDB;
//        IConfigurationSection customLoggingLevel;
//        IConfigurationSection logginTable;
//        IConfigurationSection loggingFilePath;

//        //public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
//        //.SetBasePath(Directory.GetCurrentDirectory())
//        // .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//        //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
//        //.Build();
//        public Imp()
//        {


//            string exelocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//            IConfiguration Configuration = new ConfigurationBuilder().SetBasePath(exelocation)
//                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                .Build().GetSection("CI.Logging");

//            connectionStringloggingDB = Configuration.GetSection("ConnectionStrings:LoggingDatabase");
//            customLoggingLevel = Configuration.GetSection("CustomLoggingLevel");
//            logginTable = Configuration.GetSection("LoggingTable");
//            loggingFilePath = Configuration.GetSection("LogFilePath");


//            // var logConfiguration = new LoggerConfiguration()
//            //.ReadFrom.Configuration(Configuration);

//            // Log.Logger = logConfiguration.CreateLogger();

//        }
//        public void LogInformation(LogModel logModel)
//        {


//            ValidateMandatoryEntries(connectionStringloggingDB, customLoggingLevel, logginTable, loggingFilePath);

//            //Log in console to trace any Serilog issues
//            Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Debug.WriteLine(msg));

//            var levelSwitch = new Serilog.Core.LoggingLevelSwitch();
//            levelSwitch.MinimumLevel = getLogEventLevel(customLoggingLevel.Value);

//            var logLevel = getLogEventLevel(LogLevels.Information.ToString());

//            using (var log = GetLog(connectionStringloggingDB.Value, logginTable.Value, loggingFilePath.Value, levelSwitch))
//            {
//                if (log.IsEnabled(logLevel))
//                {
//                    log.ForContext("Host", logModel.Host)
//                         .ForContext("Source", logModel.Source)
//                         .ForContext("User", logModel.User)
//                         .ForContext("CorellationId", logModel.CorellationId).Information(logModel.Message + $"{logModel.Host}:{logModel.User} : {logModel.CorellationId} : {logModel.Source}");
//                }
//                else
//                {
//                    var result = $"Logger: eventLevel:{nameof(logLevel)} is not enabled";
//                    System.Diagnostics.Debug.WriteLine(result);
//                }
//            }
//        }

//        public void LogError(ExceptionModel logModel)
//        {


//            ValidateMandatoryEntries(connectionStringloggingDB, customLoggingLevel, logginTable, loggingFilePath);

//            //Log in console to trace any Serilog issues
//            Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Debug.WriteLine(msg));

//            var levelSwitch = new Serilog.Core.LoggingLevelSwitch();
//            levelSwitch.MinimumLevel = getLogEventLevel(customLoggingLevel.Value);

//            var logLevel = getLogEventLevel(LogLevels.Error.ToString());

//            using (var log = GetLog(connectionStringloggingDB.Value, logginTable.Value, loggingFilePath.Value, levelSwitch))
//            {
//                if (log.IsEnabled(logLevel))
//                {
//                    log.ForContext("Host", logModel.Host)
//                         .ForContext("Source", logModel.Source)
//                         .ForContext("Type", logModel.ExceptionDetail.GetType())
//                         .ForContext("User", logModel.User)
//                         .ForContext("Statuscode", logModel.Statuscode)
//                         .ForContext("CorellationId", logModel.CorellationId).Error(logModel.ExceptionDetail, logModel.Message);
//                    // .Information(logModel.Message + $"{logModel.Host}:{logModel.User} : {logModel.CorellationId} : {logModel.Source}");
//                }
//                else
//                {
//                    var result = $"Logger: eventLevel:{nameof(logLevel)} is not enabled";
//                    System.Diagnostics.Debug.WriteLine(result);
//                }
//            }
//        }

//        /// <summary>
//        /// Gets a custom logger configuration
//        /// </summary>
//        /// <param name="connectionStringLoggingDb">connection string</param>
//        /// <param name="loggingTable">name of the table</param>
//        /// <param name="columnOptions">configuration of columns (excluded, additional, customized) in the table</param>
//        /// <param name="levelSwitch">minimun level to log</param>
//        /// <returns>custom logger configuration object</returns>
//        private static Serilog.Core.Logger GetLog(string connectionStringLoggingDb,
//            string loggingTable, string loggingFilePath, Serilog.Core.LoggingLevelSwitch levelSwitch)
//        {
//            return new LoggerConfiguration()
//                           //.MinimumLevel.Information()
//                           .MinimumLevel.ControlledBy(levelSwitch)
//                           .WriteTo.File(loggingFilePath,
//                           rollingInterval: RollingInterval.Minute,
//                           rollOnFileSizeLimit: true,
//                           fileSizeLimitBytes: 5242880,
//                           retainedFileCountLimit: 60)
//                            //.MinimumLevel.Override("SerilogDemo", Serilog.Events.LogEventLevel.Information)
//                            .WriteTo.MSSqlServer(connectionStringLoggingDb,
//                                           loggingTable,
//                                    //autoCreateSqlTable: false,
//                                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
//                                    )
//                           .CreateLogger();
//        }

//        /// <summary>
//        /// Gets correspondant Serilog Log Event Level
//        /// </summary>
//        /// <param name="logLevel">name of the log level</param>
//        /// <returns>LogEventLevel</returns>
//        private Serilog.Events.LogEventLevel getLogEventLevel(string logLevel)
//        {
//            var level = Serilog.Events.LogEventLevel.Debug;
//            try
//            {
//                level = (Serilog.Events.LogEventLevel)Enum.Parse(typeof(Serilog.Events.LogEventLevel), logLevel);
//            }
//            catch (Exception ex)
//            {
//                var result = $"CLogger.Logger.getLogEventLevel(): exception getting customlogging level, defaulted to Debug Level. logLevel:{logLevel}, exception:{ex}";
//                System.Diagnostics.Debug.WriteLine(result);
//            }
//            return level;
//        }

//        private void ValidateMandatoryEntries(IConfigurationSection connectionStringLoggingDb, IConfigurationSection customLoggingLevel,
//            IConfigurationSection loggingTable, IConfigurationSection loggingFilePath)
//        {
//            if (string.IsNullOrEmpty(connectionStringLoggingDb.Value))
//            {
//                throw new ArgumentNullException("ConnectionStrings:LoggingDatabase null or empty");
//            }

//            if (string.IsNullOrEmpty(customLoggingLevel.Value))
//            {
//                throw new ArgumentNullException("CustomLoggingLevel null or empty");
//            }

//            if (string.IsNullOrEmpty(loggingTable.Value))
//            {
//                throw new ArgumentNullException("LoggingTable null or empty");
//            }

//            if (string.IsNullOrEmpty(loggingFilePath.Value))
//            {
//                throw new ArgumentNullException("loggingFilePath null or empty");
//            }
//        }

//    }
//}
