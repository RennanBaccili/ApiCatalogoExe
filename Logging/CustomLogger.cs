
namespace ApiCatalogo.Logging
{
    public class CustomLogger : ILogger
    {
        readonly string loggerName;

        readonly CustomLoggerProviderConfiguration Configuration;

        public CustomLogger(string name, CustomLoggerProviderConfiguration config)
        {
            loggerName = name;
            Configuration = config;
        }

        
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == Configuration.LogLevel;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = $"{logLevel.ToString()}:{eventId.Id} - {formatter(state, exception)}";
            
            EscreverTextoNoArquivo(message);
        }

        private void EscreverTextoNoArquivo(string message)
        {
            string caminhoArquivoLog = @"C:\Log\log.txt";

            using (StreamWriter streamWriter = new StreamWriter(caminhoArquivoLog, true))
                try
                {
                    {
                        streamWriter.WriteLine(message);
                        streamWriter.Close();
                    }
                } catch (Exception)
                {
                    throw;
                }
            

        }
    }
}
