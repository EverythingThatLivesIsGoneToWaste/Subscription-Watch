namespace subscription_watch.Models
{
    public class PostgresOptions
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 5432;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string SslMode { get; set; } = "Prefer";
        public bool Pooling { get; set; } = true;
        public bool IncludeErrorDetail { get; set; } = false;
    }
}
