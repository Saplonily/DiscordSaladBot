using System;
using System.Collections.Generic;
using Discord.WebSocket;

public class Command
{
    public string CommandName { get; }
    public int ArgsCounts { get; } = 0;
    public ActionHandler Action { get; private set; }
    public delegate void ActionHandler(string[] args, SocketMessage msg);

    public Command(string commandName, int argsCounts, ActionHandler action)
    {
        CommandName = commandName;
        ArgsCounts = argsCounts;
        Action = action;
    }

    public static Command Create(string commandName, int argsCounts, ActionHandler action) =>
        new Command(commandName, argsCounts, action);

}