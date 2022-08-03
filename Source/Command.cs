using System;
using System.Collections.Generic;
using Discord.WebSocket;

using ActionCommand = System.Action<string[], Discord.WebSocket.SocketMessage>;

public class Command
{
    
    public string CommandName { get; }
    public int ArgsCounts { get; } = 0;
    public ActionCommand Action { get; }
    public delegate void ActionHandler(string[] args,SocketMessage msg);

    public Command(string commandName, int argsCounts, ActionCommand action)
    {
        CommandName = commandName;
        ArgsCounts = argsCounts;
        Action = action;
    }

    public static Command Create(string commandName, int argsCounts, ActionCommand action) =>
        new Command(commandName, argsCounts, action);

    public static void AnalyzeMessage(SocketMessage message)
    {
        var text = message.Content;
        text = text.Trim();
        var parts = text.Split(" ");

        
        foreach (var cmd in RootCommandSet)
        {
            cmd.BelongTo.BelongTo.BelongTo.BelongTo.BelongTo.BelongTo
            if (cmd.CommandName == parts[1])
            {
                if (parts.Length - 2 >= cmd.ArgsCounts)
                {
                    //parts: salad repeat I'm sb
                    //cmd.argsCounts: 1
                    //parts.Length - 2: 2


                    if (parts.Length - 2 > cmd.ArgsCounts)
                    {
                        for (int i = 1; i <= parts.Length - 2 - cmd.argsCounts; i++)
                        {
                            parts[1 + cmd.ArgsCounts] += " " + parts[1 + cmd.argsCounts + i];
                        }
                    }

                    var args = new string[cmd.ArgsCounts];
                    Array.Copy(parts, 2, args, 0, args.Length);
                    cmd.Action.Invoke(args, message);
                }
                else
                {
                    message.Channel.SendMessageAsync(
                        $"{message.Author.Mention}, less arguments got!"
                    );
                }
            }
        }
    }


}