using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Discord;
using Discord.WebSocket;

namespace SaladBot.Sets;

public partial class TictocGameSet : ICommandSet
{
    public string SetName => "tictoc";
    public List<ICommandSet> ChildCommandSets { get => null; }
    public List<Command> ChildCommands { get; private set; }
    public bool IsSet { get => true; }
    public ICommandSet BelongTo { get; private set; }

    protected Dictionary<SocketGuild, Tictoc> Data { get; private set; } = new Dictionary<SocketGuild, Tictoc>();

    public TictocGameSet(ICommandSet belongTo)
    {
        BelongTo = belongTo;
        ChildCommands = CommandSetHelper.GetCommands(this);
    }

    [Command("create")]
    public void Create(string[] args, SocketMessage msg)
    {
        msg.Channel.SendMessageAsync("A tictoc game created!");
        var tictoc = msg.GetDataIns(Data);
        tictoc.playingChannel = msg.Channel;
    }

    [Command("join")]
    public void Join(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).Join(msg.Author);
    }

    [Command("leave")]
    public void Leave(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).Leave(msg.Author);
    }

    [Command("set_counts", 1)]
    public void SetCounts(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).counts = int.Parse(args[0]);
        msg.Channel.SendMessageAsync($"*Counts to win* has been set to {msg.GetDataIns(Data).counts}");
    }

    [Command("check")]
    public void Check(string[] args, SocketMessage msg)
    {
        if (msg.GetDataIns(Data).GameIsRunning)
        {
            msg.GetDataIns(Data).ShowGamePad();
        }
        else
        {
            msg.Channel.SendMessageAsync("Game haven't started!");
        }
    }

    [Command("start")]
    public void Start(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).Start();
    }

    [Command("place")]
    public void Place(string[] args, SocketMessage msg)
    {
        var game = msg.GetDataIns(Data);
        var winner = game.Place(int.Parse(args[0]) - 1, int.Parse(args[1]) - 1, msg.Author);
        if (winner is not null)
        {
            msg.Channel.SendMessageAsync($"{winner.Mention} won the game!");
            this.End(args, msg);
        }
    }

    [Command("set_size",2)]
    public void StaSetSize(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).SetSize(int.Parse(args[0]), int.Parse(args[1]));
    }

    [Command("end")]
    public void End(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).End();
        msg.Channel.SendMessageAsync("Game ended!");
        Data.Remove(msg.GetGuildIn());
    }

    [Command("list")]
    public void ShowPlayerList(string[] args, SocketMessage msg)
    {
        var str = "Players playing: ";
        foreach (var item in msg.GetDataIns(Data).players)
        {
            str += item.Username + " ";
        }
        msg.Channel.SendMessageAsync(str);
    }

}