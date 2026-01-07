namespace AetherBags.Extensions;

public static class LoggerExtensions
{
    extension(object logger)
    {
        public void DebugOnly(string message)
        {
            if (System.Config?.General?.DebugEnabled == true)
            {
                Services.Logger.DebugOnly(message);
            }
        }

        public void DebugOnly(string message, params object[] args) => DebugOnly(logger, string.Format(message, args));
    }
}