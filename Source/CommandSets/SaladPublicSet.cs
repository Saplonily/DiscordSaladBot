using System.Windows.Input;
using System.Text;
using System.Xml;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using System.Text.Json;

using StringDictionary = System.Collections.Generic.Dictionary<string, string>;
using System.Reflection;

public class SaladPublicSet : ICommandSet
{
    public SocketGuild guildIn;
    public TictocGameSet tictoc;
    public ISocketMessageChannel dataChannel;

    private string resourceHeader;
    private int resourceCheckingNumber = 5;
    private Stopwatch watch = new Stopwatch();

    public ICommandSet BelongTo => null;
    public string SetName { get; private set; } = "salad";
    public List<ICommandSetChild> Children { get; private set; }
    public bool IsSet { get => true; }

    public SaladPublicSet()
    {
        using (JsonDocument jd = JsonDocument.Parse(File.ReadAllText("config.json")))
        {
            resourceHeader = jd.RootElement.GetProperty("config").GetProperty("resource_header").GetString();
        }
        //init commands
        var cmds = CommandSetHelper.GetCommands(this);

        Command.CheckingCommands.AddRange(new List<Command>()
        {
            Command.Create("help",1,async(args,msg)=>
            {
                switch(args[0])
                {
                    case "tictoc":
                        await msg.Channel.SendMessageAsync(await FindResource("tictocHelpString"));
                        break;
                }
            }),
            Command.Create("set_data_channel",1,async(args,msg)=>
            {
                //dataChannel = 
                //await msg.Channel.SendMessageAsync($"data channel has been set to #{dataChannel}");
            }),
            Command.Create("watch",1,async(args,msg)=>
            {
                switch(args[0])
                {
                    case "start":
                        watch.Start();
                        await msg.Channel.SendMessageAsync("The stopwatch is running!");
                        break;
                    case "stop":
                        watch.Stop();
                        await msg.Channel.SendMessageAsync(
                            $"The stopwatch stopped! Time = {watch.ElapsedMilliseconds / 1000.0f}s"
                        );
                        watch.Reset();
                        break;
                    case "pause":
                        watch.Stop();
                        await msg.Channel.SendMessageAsync(
                            $"The stopwatch paused! Time = {watch.ElapsedMilliseconds / 1000.0f}s"
                        );
                        break;
                    case "resume":
                        watch.Start();
                        await msg.Channel.SendMessageAsync(
                            $"The stopwatch resumed. from time = {watch.ElapsedMilliseconds / 1000.0f}s"
                        );
                        break;
                }
            })
        });

        Command.DefaultPrefix = "tictoc";
        Command.CheckingCommands.AddRange(new List<Command>()
        {
            Command.Create("join",0,(args,msg) => {
                tictoc.Join(msg.Author);
            }),
            Command.Create("leave",0,(args,msg) => {
                tictoc.Leave(msg.Author);
            }),
            Command.Create("counts",1,(args,msg) => {
                tictoc.counts = int.Parse(args[0]);
                tictoc.Msg($"*Counts to win* has been set to {tictoc.counts}");
            }),
            Command.Create("check",0,(args,msg) => {
                if (tictoc.GameIsRunning)
                {
                    tictoc.ShowGamePad();
                }
                else
                {
                    tictoc.Msg("Game have not been started!");
                }
            }),
            Command.Create("start",0,(args,msg) => {
                tictoc.Start();
            }),
            Command.Create("place",2,(args,msg)=>{
                tictoc.Place(int.Parse(args[0]) - 1, int.Parse(args[1]) - 1, msg.Author);
            }),
            Command.Create("set_size",2,(args,msg)=>{
                tictoc.SetSize(int.Parse(args[0]),int.Parse(args[1]));
            }),
            Command.Create("end",0,(args,msg)=>{
                tictoc.End();
                tictoc = null;
            }),
            Command.Create("list",0,(args,msg)=>
            {
                var str = "Players playing: ";
                foreach (var item in tictoc.players)
                {
                    str += item.Username + " ";
                }
                tictoc.Msg(str);
            })
        });
    }

    [Command("hello", 0)]
    public async Task Hello(string[] args, SocketMessage msg)
    {
        await msg.Channel.SendMessageAsync($"{msg.Author.Mention} Hello!");
    }

    [Command("age", 0)]
    public async Task Age(string[] args, SocketMessage msg)
    {
        await msg.Channel.SendMessageAsync($"Your account was created at {msg.Author.CreatedAt.DateTime}");
    }

    [Command("tictoc", 0)]
    public async Task Tictoc(string[] args, SocketMessage msg)
    {
        tictoc = new TictocGameSet(msg.Channel);
        await msg.Channel.SendMessageAsync($"tictoc game started! send \"/tictoc join\" to join the game!");
    }

    [Command("repeat", 1)]
    public async Task Repeat(string[] args, SocketMessage msg)
    {
        await msg.Channel.SendMessageAsync(args[0]);
    }

    [Command("repeat_to_channel", 2)]
    public async Task RepeatToChannel(string[] args, SocketMessage msg)
    {
        var channelEnum = msg.MentionedChannels.GetEnumerator();
        channelEnum.MoveNext();
        var channel = channelEnum.Current as IMessageChannel;
        await channel.SendMessageAsync(args[1]);
    }

    [Command("read_resource", 1)]
    public async Task ReadResource(string[] args, SocketMessage msg)
    {
        var s = FindResource(args[0]);
        await msg.Channel.SendMessageAsync($"resource found:\n{await s}");
    }

    [Command("check_weather", 2)]
    public async Task CheckWeather(string[] args, SocketMessage msg)
    {
        var xmlStr = HTTPRequester.SendPost(
            "http://www.webxml.com.cn/WebServices/WeatherWebService.asmx/getWeatherbyCityName",
            new StringDictionary()
            {
                {"theCityName", args[0]}
            },
            "GET"
            );
        XmlDocument xd = new XmlDocument();
        xd.LoadXml(xmlStr);
        XmlNode root = xd.ChildNodes[1];
        if (!root.FirstChild.FirstChild.Value.Contains("结果为空"))
        {
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < root.ChildNodes.Count; index++)
            {
                XmlNode item = root.ChildNodes[index];
                string value = item.FirstChild.Value;
                switch (index)
                {
                    case 0: sb.Append($"城市 : {value} "); break;
                    case 1: sb.Append($"- {value}\n"); break;
                    case 4: sb.Append($"数据最后更新时间: {value}\n"); break;
                }
                if (args[1].Contains("today"))
                {
                    switch (index)
                    {
                        case 5: sb.Append($"气温: {value}\n"); break;
                        case 6: sb.Append($"概况: {value} "); break;
                        case 7: sb.Append($"{value}\n"); break;
                        case 10: sb.Append($"{value}\n"); break;
                        case 11: if (args[1].Contains("showIndex")) sb.Append($"{value}"); break;
                    }
                }
                if (args[1].Contains("secondDay"))
                {
                    switch (index)
                    {
                        case 12: sb.Append($"第二天气温: {value}\n"); break;
                        case 13: sb.Append($"概况: {value} "); break;
                        case 14: sb.Append($"{value}\n"); break;
                    }
                }
                if (args[1].Contains("thirdDay"))
                {
                    switch (index)
                    {
                        case 17: sb.Append($"第三天气温: {value}\n"); break;
                        case 18: sb.Append($"概况: {value} "); break;
                        case 19: sb.Append($"{value}\n"); break;
                    }
                }
                if (args[1].Contains("introduceCity"))
                {
                    if (index == 22)
                        sb.Append(value);
                }

            }
            await msg.Channel.SendMessageAsync(sb.ToString());
        }
        else
        {
            await msg.Channel.SendMessageAsync($"not support city! city name : {args[0]}");
        }
    }

    [Command("send_file", 1)]
    public async Task SendFile(string[] args, SocketMessage msg)
    {
        if (args[0].Contains("..") || args[0].Contains("config.json"))
        {
            await msg.Channel.SendMessageAsync("nice try.");
            return;
        }
        if (File.Exists(args[0]))
        {
            await msg.Channel.SendFileAsync(args[0]);
        }
        else
        {
            await msg.Channel.SendMessageAsync($"file \"{args[0]}\" not exists!");
        }

    }

    [Command("check_ip", 1)]
    public async Task CheckIp(string[] args, SocketMessage msg)
    {
        var xmlStr = HTTPRequester.SendPost(
            "http://ws.webxml.com.cn/WebServices/IpAddressSearchWebService.asmx/getCountryCityByIp",
            new StringDictionary()
            {
                {"theIpAddress", args[0]}
            },
            "GET"
            );
        XmlDocument xd = new XmlDocument();
        xd.LoadXml(xmlStr);
        var root = xd.ChildNodes[1];
        var str = $"IP: {root.ChildNodes[0].FirstChild.Value}\n";
        str += $"Result: {root.ChildNodes[1].FirstChild.Value}";
        await msg.Channel.SendMessageAsync(str);
    }

    [Command("where_are_you", 0)]
    public async Task WhereAreYou(string[] args, SocketMessage msg)
    {
        await msg.Channel.SendMessageAsync("I'm in these servers:");
        foreach (var item in Program.Client.Guilds)
        {
            await msg.Channel.SendMessageAsync($"--> {item.Name}");
        }
    }

    [Command("send_http_request", 1)]
    public async Task SendHTTPRequest(string[] args, SocketMessage msg)
    {
        string result;
        result = HTTPRequester.SendPost(args[0], new Dictionary<string, string>(), "POST");
        await msg.Channel.SendMessageAsync(result);
    }

    private async Task<string> FindResource(string resourceName)
    {
        var msgs = dataChannel.GetMessagesAsync(resourceCheckingNumber);
        await foreach (var spans in msgs)
        {
            foreach (var item in spans)
            {
                var lines = item.Content.Split("\n");
                lines[0].Trim();
                if (lines[0].Contains(resourceHeader))
                {
                    var str = lines[0].Remove(0, resourceHeader.Length + 1);
                    if (str == resourceName)
                    {
                        //2是因为有空格和回车
                        return item.Content.Remove(0, resourceHeader.Length + resourceName.Length + 2);
                    }
                }
            }
        }
        return "Null";
    }
}