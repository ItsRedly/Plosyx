using Console = Colorful.Console;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Drawing;

[DllImport("libc")]
static extern uint getuid();

Console.Title = "Plosyx";
Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;
Console.ForegroundColor = Color.Magenta;
Console.WriteAscii("Plosyx");
Console.SetCursorPosition(Console.WindowWidth - 10, 2);
Console.WriteLine("Discords:");
Console.CursorLeft = Console.WindowWidth - 10;
Console.WriteLine($"Kiwi#2789");
Console.CursorLeft = Console.WindowWidth - 14;
Console.WriteLine($"ItsRedly#1174");
Console.CursorLeft = 14;
Console.WriteLine("𝓜𝓪𝓭𝓮 𝓫𝔂 𝓘𝓽𝓼𝓡𝓮𝓭𝓵𝔂 𝓪𝓷𝓭 𝓚𝓲𝔀𝓲\n");
try
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                Console.CursorLeft = (Console.WindowWidth - "Plosyx must be run as administrator. Please restart Plosyx as administrator to proceed".Length) / 2;
                Console.Write("Plosyx must be run as administrator. Please restart Plosyx as administrator to proceed");
                Console.ReadKey(true);
                return;
            }
        }
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && getuid() != 0)
    {
        Console.CursorLeft = (Console.WindowWidth - "Plosyx must be run as root/sudo. Please restart Plosyx as root/sudo to proceed".Length) / 2;
        Console.Write("Plosyx must be run as root/sudo. Please restart Plosyx as root/sudo to proceed");
        Console.ReadKey(true);
        return;
    }
}
catch
{
    Console.CursorLeft = (Console.WindowWidth - "Unable to determine administrator or root/sudo status. Plosyx will attempt to run but errors might occour".Length) / 2;
    Console.WriteLine("Unable to determine administrator or root/sudo status. Plosyx will attempt to run but errors might occour");
}
Console.Write("What is the thread limit for handling connections? (default is 50000000 and a high value might make it impossible to exit): ");
string threadLimit;
if (!int.TryParse(threadLimit = Console.ReadLine()!, out _)) { threadLimit = "50000000"; }
Console.CursorTop -= 2;
Console.Write(new string(' ', Console.WindowWidth));
Console.CursorLeft = (Console.WindowWidth - "Control panel will be opened in a browser window (it might take a while thought)".Length) / 2;
Console.WriteLine("Control panel will be opened in a browser window (it might take a while thought)");
Console.Write(new string(' ', Console.WindowWidth));
Console.CursorLeft = (Console.WindowWidth - "Press any key to stop the webserver...".Length) / 2;
Console.Write("Press any key to stop the webserver...");
HttpListener website = new HttpListener() { AuthenticationSchemes = AuthenticationSchemes.Negotiate };
website.Prefixes.Add("http://*:8080/");
website.Start();
CancellationTokenSource source = new CancellationTokenSource();
Enumerable.Repeat(Task.Run(() =>
{
    while (true)
    {
        HttpListenerContext context = website.GetContext();
        context.Response.ContentEncoding = Encoding.Default;
        context.Response.ContentType = "text/html";
        context.Response.StatusCode = 404;
        byte[] content = Encoding.Default.GetBytes($"<html><head><title>Plosyx</title></head><body><center><h1 style=\"color: red\">Error</h1><h3>Endpoint not found</h3></center></body></html>");
        switch (context.Request.Url!.AbsolutePath)
        {
            case "/":
                content = Encoding.Default.GetBytes($"<html><head><title>Plosyx</title></head><body><center><h1>Plosyx Control Panel</h1><h3>Its time to cause some caos, isn't it Mr(s). {context.User!.Identity!.Name!.Substring(Environment.MachineName.Length + 1)}???</h3></center></body></html>");
                context.Response.StatusCode = 200;
                break;

            case "/api/user":
                switch (context.Request.HttpMethod)
                {
                    case "GET":
                        context.Response.StatusCode = 200;
                        break;

                    case "POST":
                        context.Response.StatusCode = 200;
                        break;

                    case "RENAME":
                        context.Response.StatusCode = 200;
                        break;

                    case "DELETE":
                        context.Response.StatusCode = 200;
                        break;

                    default:
                        content = Encoding.Default.GetBytes($"<html><head><title>Plosyx</title></head><body><center><h1 style=\"color: red\">Error</h1><h3>Invalid http method. Suported types: GET, RENAME, DELETE</h3></center></body></html>");
                        context.Response.StatusCode = 405;
                        break;
                }
                break;
        }
        context.Response.ContentLength64 = content.LongLength;
        context.Response.OutputStream.Write(content);
    }
}), int.Parse(threadLimit)).ToList();
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { Process.Start("explorer", "http://127.0.0.1:8080"); }
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { Process.Start("xdg-open", "http://127.0.0.1:8080"); }
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { Process.Start("open", "http://127.0.0.1:8080"); }
Console.ReadKey(true);
source.Cancel();
website.Close();