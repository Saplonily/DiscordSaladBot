using System.Diagnostics;
using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using SaladBot;

namespace SaladBot.Sets;

public class WatchSet : ICommandSet
{
    public string SetName { get => "watch"; }
    public List<ICommandSet> ChildCommandSets { get => null; }
    public List<Command> ChildCommands { get; private set; }
    public bool IsSet { get => true; }
    public ICommandSet BelongTo { get; private set; }

    protected Dictionary<SocketGuild, Watch> Data { get; private set; } = new Dictionary<SocketGuild, Watch>();

    protected class Watch
    {
        public Stopwatch watch { get; private set; } = new Stopwatch();
    }

    public WatchSet(ICommandSet belongTo)
    {
        this.BelongTo = belongTo;
        ChildCommands = CommandSetHelper.GetCommands(this);
    }

    [Command("start")]
    public void Start(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).watch.Start();
        msg.Channel.SendMessageAsync("The stopwatch is running!");
    }

    [Command("stop")]
    public void Stop(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).watch.Stop();
        msg.Channel.SendMessageAsync(
            $"The stopwatch stopped! Time = {msg.GetDataIns(Data).watch.ElapsedMilliseconds / 1000.0f}s"
        );
        msg.GetDataIns(Data).watch.Reset();
    }
    [Command("pause")]
    public void Pause(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).watch.Stop();
        msg.Channel.SendMessageAsync(
            $"The stopwatch paused! Time = {msg.GetDataIns(Data).watch.ElapsedMilliseconds / 1000.0f}s"
        );
    }
    [Command("resume")]
    public void Resume(string[] args, SocketMessage msg)
    {
        msg.GetDataIns(Data).watch.Start();
        msg.Channel.SendMessageAsync(
            $"The stopwatch resumed. from time = {msg.GetDataIns(Data).watch.ElapsedMilliseconds / 1000.0f}s"
        );
    }
}