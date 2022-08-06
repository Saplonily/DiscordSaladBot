using System.Threading;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace SaladBot;

public class CountdownSet : ICommandSet
{
    public string SetName => "countdown";

    public ICommandSet BelongTo { get; private set; }

    public bool IsSet { get => true; }

    public List<ICommandSet> ChildCommandSets { get; private set; }

    public List<Command> ChildCommands { get; private set; }

    public CountdownSet(ICommandSet belongTo)
    {
        ChildCommands = CommandSetHelper.GetCommands(this);
        this.BelongTo = belongTo;
    }

    [Command("new", 2)]
    public void NewCountdown(string[] args, SocketMessage msg)
    {
        double time;
        if (Double.TryParse(args[0], out time))
        {
            Countdown c = new Countdown(Convert.ToInt64(time * 1000));
            c.CountdownFinished += c =>
            {
                msg.Channel.SendMessageAsync($"Countdown -{args[1]}- ended.");
                c = null;
            };
            msg.Channel.SendMessageAsync($"Countdown -{args[1]}- started.");
        }
        else
        {
            msg.Channel.SendMessageAsync("Invalid time!");
        }
    }

    public class Countdown
    {
        public Stopwatch watch = new Stopwatch();
        public long milliseconds = 1000;
        public delegate void CountdownFinishedHandler(Countdown countdown);
        public event CountdownFinishedHandler CountdownFinished;

        public Countdown(long time)
        {
            milliseconds = time;
            watch.Start();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (watch.ElapsedMilliseconds > milliseconds)
                    {
                        CountdownFinished.Invoke(this);
                        break;
                    }
                    Thread.Sleep(30);
                }
                return;
            });
        }
    }
}