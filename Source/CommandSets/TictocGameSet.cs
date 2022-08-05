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

    public TictocGameSet(ICommandSet belongTo)
    {
        BelongTo = belongTo;
        ChildCommands = CommandSetHelper.GetCommands(this);
    }

    [Command("join")]
    public void Join(string[] args, SocketMessage msg)
    {
        tictoc.Join(msg.Author);
    }

    [Command("leave")]
    public void Leave(string[] args, SocketMessage msg)
    {
        tictoc.Leave(msg.Author);
    }

    [Command("set_counts", 1)]
    public void SetCounts(string[] args, SocketMessage msg)
    {
        tictoc.counts = int.Parse(args[0]);
        tictoc.Msg($"*Counts to win* has been set to {tictoc.counts}");
    }

    [Command("check")]
    public void Check(string[] args, SocketMessage msg)
    {
        if (tictoc.GameIsRunning)
        {
            tictoc.ShowGamePad();
        }
        else
        {
            tictoc.Msg("Game have not been started!");
        }
    }

    [Command("start")]
    public void Start(string[] args, SocketMessage msg)
    {
        tictoc.Start();
    }

    [Command("place")]
    public void Place(string[] args, SocketMessage msg)
    {
        tictoc.Place(int.Parse(args[0]) - 1, int.Parse(args[1]) - 1, msg.Author);
    }

    [Command("set_size")]
    public void StaSetSize(string[] args, SocketMessage msg)
    {
        tictoc.SetSize(int.Parse(args[0]), int.Parse(args[1]));
    }

    [Command("end")]
    public void End(string[] args, SocketMessage msg)
    {
        tictoc.End();
        tictoc = null;
    }

    [Command("list")]
    public void ShowPlayerList(string[] args, SocketMessage msg)
    {
        var str = "Players playing: ";
        foreach (var item in tictoc.players)
        {
            str += item.Username + " ";
        }
        tictoc.Msg(str);
    }

}