using System.Threading;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

public class CountdownSet : ICommandSet, ICommandSetChild
{
    public CountdownSet(ICommandSet belongTo)
    {
        CommandSetHelper.InitCommands(this);
        this.BelongTo = belongTo;
    }

    public string SetName => "countdown";

    public ICommandSet BelongTo { get; private set; }

    public bool IsSet { get => true; }

    public List<ICommandSet> Children => throw new NotImplementedException();

    [Command("new", 2)]
    public async void NewCountdown(string[] args, SocketMessage msg)
    {
        double time;
        if (Double.TryParse(args[0], out time))
        {
            Countdown c = new Countdown(Convert.ToInt64(time * 1000));
            c.CountdownFinished += async c =>
            {
                await msg.Channel.SendMessageAsync($"Countdown -{args[1]}- ended.");
                c = null;
            };
            await msg.Channel.SendMessageAsync($"Countdown -{args[1]}- started.");
        }
        else
        {
            await msg.Channel.SendMessageAsync("Invalid time!");
        }
    }

    public class Countdown
    {
        public Stopwatch watch = new Stopwatch();
        public delegate void CountdownFinishedHandler(Countdown countdown);
        public event CountdownFinishedHandler CountdownFinished;
        public long milliseconds = 1000;

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