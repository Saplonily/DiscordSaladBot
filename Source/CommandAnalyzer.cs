using System;
using System.Collections.Generic;
using Discord.WebSocket;

public static class CommandAnalyzer
{
    public static List<ICommandSet> RootCommandSet { get; } = new List<ICommandSet>();

    public static void AddToRoot(ICommandSet set)
    {
        RootCommandSet.Add(set);
    }

    public static void AnalyzeMessage(SocketMessage message)
    {
        var text = message.Content;
        text = text.Trim();
        var parts = text.Split(" ");


        foreach (var cmd in RootCommandSet)
        {
            cmd.BelongTo
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