using System;
using System.Collections.Generic;
using Discord.WebSocket;

public static class CommandAnalyzer
{
    public static List<ICommandSet> RootPublicCommandSet { get; private set; }
        = new List<ICommandSet>();
    public static List<ICommandSetGuildSeparated> RootSeparatedCommandSet { get; private set; }
        = new List<ICommandSetGuildSeparated>();

    public static void AddPublicRoot(ICommandSet set) => RootPublicCommandSet.Add(set);
    public static void AddSeparatedCommandSet(ICommandSetGuildSeparated set) => RootSeparatedCommandSet.Add(set);

    public static void AnalyzeMessage(SocketMessage message)
    {
        var text = message.Content;
        text = text.Trim();
        var cmdParts = text.Split(" ");
        cmdParts[0].Remove(0, 1);
        /*
            此时指令被解析为
            salad tictoc place 1 1
        */
        //遍历公共指令集
        foreach (var set in RootPublicCommandSet)
        {
            
        }
    }
        //var args = new string[cmd.ArgsCounts];
        //Array.Copy(parts, 2, args, 0, args.Length);
        //cmd.Action.Invoke(args, message);

}
// if (cmdParts.Length - 2 > cmd.ArgsCounts)
// {
//     for (int i = 1; i <= cmdParts.Length - 2 - cmd.argsCounts; i++)
//     {
//         cmdParts[1 + cmd.ArgsCounts] += " " + cmdParts[1 + cmd.argsCounts + i];
//     }
// }