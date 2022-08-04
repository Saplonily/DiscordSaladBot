using System;
using System.Collections.Generic;
using System.Linq;
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
            //depth是
            int depth = -1;
            Command cmdFound = null;
            depth = FindCommandInSet(set, ref cmdParts, ref message, out cmdFound);
            //合并多余参数,+2因为depth指向集合参数位置,要右移指令位后再移到参数0位
            CombineArgsParts(ref cmdParts, depth + 2);
            cmdFound.Action.Invoke(cmdParts[(depth + 1)..], message);
        }

    }

    /// <summary>
    /// 在指定集合中递归地寻找指令
    /// </summary>
    /// <param name="set">集合</param>
    /// <param name="commandFound">out参数,返回找到的指令,否则为null</param>
    /// <returns></returns>
    public static int FindCommandInSet(ICommandSet set, ref string[] cmdParts, ref SocketMessage message, out Command commandFound)
    {
        //如果指令集昵称与指令第一项相同,则调用指令集内的东西
        if (set.SetName == cmdParts[0])
        {
            //看看有没有更多指令集
            if (set.ChildCommandSets.Count != 0)
            {

            }
            else //否则就查找里面的指令
            {
                foreach (var cmd in set.ChildCommands)
                {
                    if (cmd.CommandName == cmdParts[1])
                    {
                        //指令昵称相同,执行指令并传参
                        cmd.Action.Invoke(cmdParts[1..], message);
                    }
                }
            }
        }
        //异常返回
        commandFound = null;
        return -1;
    }

    /// <summary>
    /// 合并多余的参数并追加到最后一个参数后
    /// </summary>
    public static string[] CombineArgsParts(ref string[] strs, int index)
    {
        string combinedArgs = "";
        foreach (var item in strs[index..])
        {
            combinedArgs += item + " ";
        }
        strs[index] = combinedArgs;
        return strs;
    }

}
