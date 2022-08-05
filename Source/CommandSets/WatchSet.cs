using System.Diagnostics;
using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using SaladBot;

namespace Salad.Sets;

public class WatchSet : ICommandSet
{
    public IUserGuild GuildIn { get; private set; }
    public string SetName { get => "watch"; }
    public List<ICommandSet> ChildCommandSets { get => null; }
    public List<Command> ChildCommands { get; private set; }
    public bool IsSet { get => true; }
    public ICommandSet BelongTo { get; private set; }

    protected Dictionary<IUserGuild, Watch> Data { get; private set; } = new Dictionary<IUserGuild, Watch>();

    protected class Watch
    {
        private Stopwatch watch = new Stopwatch();
    }


    public WatchSet(ICommandSet belongTo)
    {
        this.BelongTo = belongTo;
        ChildCommands = CommandSetHelper.GetCommands(this);
    }

    [Command("start")]
    public void Start(string[] args, SocketMessage msg)
    {
        watch.Start();
        msg.Channel.SendMessageAsync("The stopwatch is running!");
    }

    [Command("stop")]
    public void Stop(string[] args, SocketMessage msg)
    {
        watch.Stop();
        msg.Channel.SendMessageAsync(
            $"The stopwatch stopped! Time = {watch.ElapsedMilliseconds / 1000.0f}s"
        );
        watch.Reset();
    }
    [Command("pause")]
    public void Pause(string[] args, SocketMessage msg)
    {
        watch.Stop();
        msg.Channel.SendMessageAsync(
            $"The stopwatch paused! Time = {watch.ElapsedMilliseconds / 1000.0f}s"
        );
    }
    [Command("resume")]
    public void Resume(string[] args, SocketMessage msg)
    {
        watch.Start();
        msg.Channel.SendMessageAsync(
            $"The stopwatch resumed. from time = {watch.ElapsedMilliseconds / 1000.0f}s"
        );
    }
}