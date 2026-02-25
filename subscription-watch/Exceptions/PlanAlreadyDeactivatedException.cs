namespace subscription_watch.Exceptions
{
    public class PlanAlreadyDeactivatedException : Exception
    {
        public PlanAlreadyDeactivatedException(string title)
        : base($"Plan with title '{title}' already deactivated") { }
    }
}
