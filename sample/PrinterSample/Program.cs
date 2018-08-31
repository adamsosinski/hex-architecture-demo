using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using static System.Console;
using static System.Environment;
using static System.Text.RegularExpressions.Regex;

namespace PrinterSample
{
    using Indicators;
    using Notifiers;
    using Notifiers.Exceptions;
    using Printer.Commands;
    using Printer.Indicators;
    using Printer.Notifiers;

    class Program
    {
        private static bool _processing = false;
        private static string _printerType;
        private static string _printType;
        private static bool? _notifie = null;

        private static IConfigurationRoot _config;
        private static ManualResetEventSlim _waitHandle = new ManualResetEventSlim(initialState: false);

        static async Task Main(string[] args)
        {
            var env = GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _config = builder.Build();

            await Task.Run(async () =>
            {
                Write("***********************************" + NewLine +
                       "*                                 *" + NewLine +
                       "*       Super Fancy Printer       *" + NewLine +
                       "*                                 *" + NewLine +
                       "***********************************" + NewLine);
                WriteLine("Welcome to Super Fancy Printer. Enter help to see available options.");

                CancelKeyPress += async (e, a) => await ProcessInput("exit");

                await ProcessInput(ReadLine());
            });

            _waitHandle.Wait();
        }

        private static async Task ProcessInput(string input)
        {
            var i = input?.Trim().ToLowerInvariant() ?? String.Empty;
            GroupCollection match;
            switch (i)
            {
                case "help":
                    Write("command [options]" + NewLine + NewLine +
                        "Available commands:" + NewLine +
                        "   print       - print requested content. For more information use print help" + NewLine +
                        "   exit        - exit from aplication" + NewLine +
                        "   help        - show help for application" + NewLine + NewLine);
                    await ProcessInput(ReadLine());
                    break;
                case "print help":
                    Write("print [options] <content>" + NewLine + NewLine +
                        "Available options:" + NewLine +
                        "   printerType       - select printer type. [s] for standard, [p] for premium. Default is standard." + NewLine +
                        "   printType         - select print type. [b] for black, [c] for color. Default is black." + NewLine +
                        "   n                 - select if want to receive notification on finsh." + NewLine +
                        "   d                 - select for default options" + NewLine + NewLine);
                    await ProcessInput(ReadLine());
                    break;
                case var o when IsMatch(o, "print(?: printertype)? (s|p)(?: printtype)? (b|c) n (.*)"):
                    match = Match(o, "print(?: printertype)? (s|p)(?: printtype)? (b|c) n (.*)").Groups;
                    _printerType = match[1].Value;
                    _printType = match[2].Value;
                    _notifie = true;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print(?: printertype)? (s|p) n(?: printtype)? (b|c) (.*)"):
                    match = Match(o, "print(?: printertype)? (s|p) n(?: printtype)? (b|c) (.*)").Groups;
                    _printerType = match[1].Value;
                    _notifie = true;
                    _printType = match[2].Value;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print(?: printtype)? (b|c)(?: printertype)? (s|p) n (.*)"):
                    match = Match(o, "print(?: printtype)? (b|c)(?: printertype)? (s|p) n (.*)").Groups;
                    _printType = match[1].Value;
                    _printerType = match[2].Value;
                    _notifie = true;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print(?: printtype)? (b|c) n(?: printertype)? (s|p) (.*)"):
                    match = Match(o, "print(?: printtype)? (b|c) n(?: printertype)? (s|p) (.*)").Groups;
                    _printType = match[1].Value;
                    _notifie = true;
                    _printerType = match[2].Value;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print n(?: printertype)? (s|p)(?: printtype)? (b|c) (.*)"):
                    match = Match(o, "print n(?: printertype)? (s|p)(?: printtype)? (b|c) (.*)").Groups;
                    _notifie = true;
                    _printerType = match[1].Value;
                    _printType = match[2].Value;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print n(?: printtype)? (b|c)(?: printertype)? (s|p) (.*)"):
                    match = Match(o, "print n(?: printtype)? (b|c)(?: printertype)? (s|p) (.*)").Groups;
                    _notifie = true;
                    _printType = match[1].Value;
                    _printerType = match[2].Value;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print(?: printertype)? (s|p)(?: printtype)? (b|c) (.*)"):
                    match = Match(o, "print(?: printertype)? (s|p)(?: printtype)? (b|c) (.*)").Groups;
                    _printerType = match[1].Value;
                    _printType = match[2].Value;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print(?: printertype)? (s|p) n (.*)"):
                    match = Match(o, "print(?: printertype)? (s|p) n (.*)").Groups;
                    _printerType = match[1].Value;
                    _notifie = true;
                    await Print(match[2].Value);
                    break;
                case var o when IsMatch(o, "print(?: printtype)? (b|c)(?: printertype)? (s|p) (.*)"):
                    match = Match(o, "print(?: printtype)? (b|c)(?: printertype)? (s|p) (.*)").Groups;
                    _printType = match[1].Value;
                    _printerType = match[2].Value;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print(?: printtype)? (b|c) n (.*)"):
                    match = Match(o, "print(?: printtype)? (b|c) n (.*)").Groups;
                    _printType = match[1].Value;
                    _notifie = true;
                    await Print(match[2].Value);
                    break;
                case var o when IsMatch(o, "print notifie(?: true|false)?(?: printertype)? (s|p) (.*)"):
                    match = Match(o, "print notifie(?: true|false)?(?: printertype)? (s|p) (.*)").Groups;
                    _notifie = Boolean.Parse(match[1].Value);
                    _printerType = match[2].Value;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print notifie(?: true|false)?(?: printtype)? (b|c) (.*)"):
                    match = Match(o, "print notifie(?: true|false)?(?: printtype)? (b|c) (.*)").Groups;
                    _notifie = Boolean.Parse(match[1].Value);
                    _printType = match[2].Value;
                    await Print(match[3].Value);
                    break;
                case var o when IsMatch(o, "print(?: printertype)? (s|p) (.*)"):
                    match = Match(o, "print(?: printertype)? (s|p) (.*)").Groups;
                    _printerType = match[1].Value;
                    await Print(match[2].Value);
                    break;
                case var o when IsMatch(o, "print(?: printtype)? (b|c) (.*)"):
                    match = Match(o, "print(?: printtype)? (b|c) (.*)").Groups;
                    _printType = match[1].Value;
                    await Print(match[2].Value);
                    break;
                case var o when IsMatch(o, "print n (.*)"):
                    match = Match(o, "print n (.*)").Groups;
                    _notifie = true;
                    await Print(match[1].Value);
                    break;
                case var o when IsMatch(o, "print d (.*)"):
                    await Print(Match(o, "print d (.*)").Groups[1].Value);
                    break;
                case "print":
                    _processing = true;
                    Write("Please chose printer type:" + NewLine +
                        "s for standard" + NewLine +
                        "p for premium" + NewLine);
                    await ProcessInput(ReadLine());
                    break;
                case "s"
                when _processing:
                case "p"
                when _processing:
                    _printerType = i;
                    Write("Please chose print type:" + NewLine +
                        "b for black" + NewLine +
                        "c for color" + NewLine);
                    await ProcessInput(ReadLine());
                    break;
                case "b"
                when !String.IsNullOrEmpty(_printerType):
                case "c"
                when !String.IsNullOrEmpty(_printerType):
                    _printType = i;
                    Write("Do you want to be notify on finish [y/n]" + NewLine);
                    await ProcessInput(ReadLine());
                    break;
                case "y"
                when !String.IsNullOrEmpty(_printType):
                case "n"
                when !String.IsNullOrEmpty(_printType):
                    _notifie = i == "y" ? true : false;
                    Write("Please provide content to print" + NewLine);
                    var content = ReadLine();
                    if (content.Trim().ToLowerInvariant() == "back")
                    {
                        await ProcessInput("back");
                        break;
                    }
                    if (String.IsNullOrWhiteSpace(content))
                    {
                        WriteLine("Content can't be empty");
                        await ProcessInput((bool)_notifie ? "y" : "n");
                    }
                    await Print(content);
                    break;
                case "back"
                when _processing:
                    if (_notifie != null)
                    {
                        _notifie = null;
                        await ProcessInput(_printType);
                        break;
                    }
                    if (!String.IsNullOrEmpty(_printType))
                    {
                        _printType = null;
                        await ProcessInput(_printerType);
                        break;
                    }
                    if (!String.IsNullOrEmpty(_printerType))
                    {
                        _printerType = null;
                        await ProcessInput("print");
                        break;
                    }
                    break;
                case "exit"
                when _processing:
                    _processing = false;
                    _printerType = null;
                    _printType = null;
                    _notifie = null;
                    await ProcessInput(ReadLine());
                    break;
                case "exit"
                when !_processing:
                    WriteLine("Thank you for using Super Fancy Printer");
                    _waitHandle.Set();
                    break;
                case "":
                    await ProcessInput(ReadLine());
                    break;
                default:
                    WriteLine("Unrecognized command, please try again or input help for available options");
                    await ProcessInput(ReadLine());
                    break;
            }
        }

        private static async Task Print(string content)
        {
            byte[] c = System.Text.Encoding.UTF8.GetBytes(content);

            IPaperLevelIndicator pli = _printerType == "p" ?
                new ComplexPaperLevelIndicator((Int32)Math.Ceiling((double)(c.Length / 100))) as IPaperLevelIndicator :
                new SimplePaperLevelIndicator() as IPaperLevelIndicator;
            IBlackLevelIndicator bli = new SimpleBlackLevelIndicator();
            IPrinterNotifier n = null;
            IPrintCommand cmd;

            if (_notifie == true)
                n = new RabbitMq(_config);

            try
            {
                if (_printType == "c")
                {
                    IColorLevelIndicator cli = _printerType == "p" ?
                        new ComplexColorLevelIndicator() as IColorLevelIndicator :
                        new SimpleColorLevelIndicator() as IColorLevelIndicator;
                    cmd = new PrintColorCommand(content: c,
                        paperLevelIndicator: pli,
                        blackLevelIndicator: bli,
                        colorLevelIndicator: cli,
                        notifier: n);
                }
                else
                {
                    cmd = new PrintBlackCommand(content: c,
                        paperLevelIndicator: pli,
                        blackLevelIndicator: bli,
                        notifier: n);

                }
                if (!(n is null))
                    await n.Subscribe((msg) =>
                    {
                        if (_printType == "c")
                            ForegroundColor = ConsoleColor.Cyan;

                        WriteLine(msg);
                        ResetColor();
                    }, PrinterNotificationTypes.Printed, cmd.Id);

                await cmd.Execute();
            }
            catch (CannotConnectToNotificationServiceException)
            {
                Write("Can not connect to notification service. Do you want to proceed? [y/n]: ");
                var key = ReadKey();
                Write(NewLine);
                if (key.Key == ConsoleKey.Y)
                {
                    _notifie = false;
                    await Print(content);
                }
            }
            catch (ApplicationException ex)
            {
                WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Magenta;
                WriteLine(ex.Message);
                ResetColor();

                ConsoleKeyInfo key;
                do key = ReadKey(intercept: false);
                while (key.Key == ConsoleKey.Enter);

                await ProcessInput("exit");
            }
            finally
            {
                _processing = false;
                _printerType = null;
                _printType = null;
                _notifie = null;
                n?.Dispose();
                await ProcessInput(ReadLine());
            }
        }
    }
}