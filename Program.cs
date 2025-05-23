namespace LabelViewUserSync
{
    internal static class Program
    {
        private static DateTime _lastActivity = DateTime.Now;
        private static readonly TimeSpan IdleTimeout = TimeSpan.FromHours(1);

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Add a global message filter to track user activity
            Application.AddMessageFilter(new ActivityMessageFilter(() => _lastActivity = DateTime.Now));

            // Start the idle monitor task
            StartIdleMonitor();

            Application.Run(new MainForm());
        }

        private static void StartIdleMonitor()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    if (DateTime.Now - _lastActivity > IdleTimeout)
                    {
                        Application.Exit();
                        break;
                    }
                }
            });
        }

        // Message filter to detect user activity
        private class ActivityMessageFilter : IMessageFilter
        {
            private readonly Action _onActivity;

            public ActivityMessageFilter(Action onActivity)
            {
                _onActivity = onActivity;
            }

            public bool PreFilterMessage(ref Message m)
            {
                // Mouse and keyboard messages
                if ((m.Msg >= 0x200 && m.Msg <= 0x20A) || // Mouse
                    (m.Msg >= 0x100 && m.Msg <= 0x109))   // Keyboard
                {
                    _onActivity();
                }
                return false;
            }
        }
    }
}