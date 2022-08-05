using System.Diagnostics;
using System;
using System.Collections.Generic;
using Discord.WebSocket;

namespace SaladBot;

[DebuggerDisplay("{CommandName}-{ArgsCounts}")]
public class Command : ICommandSetChild
{
    public string CommandName { get; }
    public int ArgsCounts { get; } = 0;
    public ActionHandler Action { get; private set; }

    public bool IsSet { get => false; }

    public ICommandSet BelongTo { get; private set; }

    public delegate void ActionHandler(string[] args, SocketMessage msg);

    public Command(string commandName, int argsCounts, ActionHandler action, ICommandSet belongTo)
    {
        CommandName = commandName;
        ArgsCounts = argsCounts;
        Action = action;
        BelongTo = belongTo;
    }

    public static Command Create(string commandName, int argsCounts, ActionHandler action, ICommandSet belongTo) =>
        new Command(commandName, argsCounts, action, belongTo);

}