using System.Linq;
using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

class Program
{
    public static void Main(string[] args)
    {
        var p = new Program();
        singleton = p;
        p.MainAsync().GetAwaiter().GetResult();
    }

    private DiscordSocketClient _client;
    public static Program singleton;
    public static DiscordSocketClient Client { get => Program.singleton._client; }

    public async Task MainAsync()
    {
        DiscordSocketConfig dsc = new DiscordSocketConfig();
        dsc.MessageCacheSize = 5;

        _client = new DiscordSocketClient(dsc);
        _client.MessageReceived += CommandHandler;
        _client.Log += Log;
        var token = "";

        using (JsonDocument jd = JsonDocument.Parse(File.ReadAllText("config.json")))
        {
            token = jd.RootElement.GetProperty("config").GetProperty("token").GetString();
        }

        CommandAnalyzer.AddPublicRoot(new SaladPublicSet());

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();


        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private Task CommandHandler(SocketMessage message)
    {
        if (message.Author.IsBot) //忽略其他bot的
            return Task.CompletedTask;

        string command = message.Content.Trim();
        int lengthOfCommand = command.Length;

        Console.WriteLine($"Message arrived:\n{message.Author}:{command}");
        bool isCommand = (command.StartsWith('!')) || (command.StartsWith('/'));
        if (isCommand && lengthOfCommand > 1)
        {
            CommandAnalyzer.AnalyzeMessage(message);
        }

        return Task.CompletedTask;
    }
}