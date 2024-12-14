using System.Diagnostics;
using System.Security.Principal;

namespace RundllKiller
{
    class Program
    {
        static void Main(string[] args)
        {
            // Checking admin rights
            if (!IsAdministrator())
            {
                Console.WriteLine("program needs admin rights. Reloading...");
                RestartAsAdmin();
                return;
            }

            // Hiding Console window
            HideConsole();

            Console.WriteLine("Progrem opened with admin rights.");
            Console.WriteLine("Executing command: 'taskkill /F /IM rundll32.exe' every hour.");

            while (true)
            {
                try
                {
                    RunTaskKillCommand();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                // waiting for an hour
                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }

        static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        static void RestartAsAdmin()
        {
            // Reloading program with admin rights
            ProcessStartInfo proc = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                Verb = "runas"
            };

            try
            {
                Process.Start(proc);
            }
            catch
            {
                Console.WriteLine("Failed to get admin rights.");
            }
        }

        static void RunTaskKillCommand()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c taskkill /F /IM rundll32.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(output))
                    Console.WriteLine($"Вывод: {output}");

                if (!string.IsNullOrWhiteSpace(error))
                    Console.WriteLine($"Ошибка: {error}");
            }
        }
        
        static void HideConsole()
        {
            IntPtr hwnd = GetConsoleWindow();
            ShowWindow(hwnd, 0);
        }
        
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    }
}
