using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;
using Colorful;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Logging {
    public sealed class LoggerProvider : ILoggerProvider {
        private const string MESSAGE_FORMAT = "[{0}] [{1}] [{2}]\n{3}";
        private readonly ConcurrentQueue<Formatter[]> _queue;
        private readonly ConcurrentDictionary<string, ILogger> _loggers;
        private bool _isDisposed;

        public LoggerProvider() {
            _loggers = new ConcurrentDictionary<string, ILogger>();
            _queue = new ConcurrentQueue<Formatter[]>();
            _isDisposed = false;

            _ = RunAsync();
        }

        public ILogger CreateLogger(string categoryName) {
            if (_loggers.TryGetValue(categoryName, out var logger)) {
                return logger;
            }

            logger = new ColorfulLogger(categoryName, LogLevel.Trace, this);
            _loggers.TryAdd(categoryName, logger);
            return logger;
        }

        public void Dispose() {
            _isDisposed = true;
            _loggers.Clear();
        }

        public void Enqueue(Formatter[] formatters) {
            _queue.Enqueue(formatters);
        }

        private async Task RunAsync() {
            while (!_isDisposed) {
                if (!_queue.TryDequeue(out var formatters))
                {
                    await Task.Delay(5);
                    continue;
                }

                Console.WriteLineFormatted(MESSAGE_FORMAT, Color.White, formatters);
                await Task.Delay(5);
            }
        }
    }
}