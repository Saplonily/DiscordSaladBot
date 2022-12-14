using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace SaladBot;

public static class CommandAnalyzer
{
    public static List<ICommandSet> RootCommandSet { get; private set; }
        = new List<ICommandSet>();

    public static void AddRoot(ICommandSet set) => RootCommandSet.Add(set);

    public static void AnalyzeMessage(SocketMessage message)
    {
        var parts = message.Content.Split(' ');
        parts[0] = parts[0].Remove(0, 1);
        Command cmd = null;
        int depth = -1;
        foreach (var set in RootCommandSet)
        {
            depth = FindCommandInSet(set, in parts, out cmd);
            if (cmd is not null)
                break;
        }
        if (cmd is null)
        {
            message.Channel.SendMessageAsync("Command not found.");
        }
        else
        {
            //parts:
            //salad countdown new 100 yourFuckingName
            //parts.Length = 5
            //depth = 1
            //parts.Length - depth - 2 = 2
            string[] gotArgs = parts[(depth + 2)..];
            //gotArgs : 100 yourFuckingName bulabula
            //cmdArgs : count = 2
            if (gotArgs.Length > cmd.ArgsCounts)
            {
                CombineArgsParts(ref gotArgs, cmd.ArgsCounts);
            }
            if (gotArgs.Length < cmd.ArgsCounts)
            {
                message.Channel.SendMessageAsync($"{message.Author.Mention} Less argument got.");
                return;
            }
            cmd.Action.Invoke(gotArgs, message);
        }
    }

    /// <summary>
    /// 在指定集合中递归地寻找指令
    /// </summary>
    /// <param name="set">集合</param>
    /// <param name="cmdParts">指令参数</param>
    /// <param name="message">消息本体</param>
    /// <param name="commandFound">out参数,返回找到的指令,否则为null</param>
    /// <param name="depthSearched">搜索深度,递归内用,默认置0</param>
    /// <returns>搜索到的深度</returns>
    public static int FindCommandInSet(
            ICommandSet set,
            in string[] cmdParts,
            out Command commandFound,
            int depthSearched = 0
        )
    {
        commandFound = null;
        int depth = depthSearched;
        //如果当前深度下指令集昵称与指令的深度位置的昵称相同,则查找指令集内的东西
        if (set.SetName == cmdParts[depth])
        {
            //看看有没有更多指令集
            //salad where_are_you
            if (set.ChildCommandSets is not null && cmdParts.Length - 2 > depth)
            {
                depth++;
                Command deeperCommandFound = null;
                foreach (var deeperSet in set.ChildCommandSets)
                {
                    FindCommandInSet(deeperSet, cmdParts, out deeperCommandFound, depth);
                    if (deeperCommandFound is not null) break;
                }
                if (deeperCommandFound is null)
                {
                    //有更多指令集,但是没有符合的,退出来,接下来会前往下面解析指令
                    depth--;
                }
                else
                {
                    //找到了指令,直接返回
                    commandFound = deeperCommandFound;
                    return depth;
                }
            }
            //遍历指令集里的指令
            foreach (var cmd in set.ChildCommands)
            {
                //TODO here
                if (cmd.CommandName == cmdParts[depth + 1])
                {
                    //指令昵称相同,返回函数
                    commandFound = cmd;
                    return depth;
                }
            }

        }
        //异常返回
        commandFound = null;
        return -1;
    }

    /// <summary>
    /// <para>合并多余的参数并追加到最后一个参数后</para>
    /// <para>位于argsRange之外的参数会被合并到最后一个参数处</para>
    /// </summary>
    /// <param name="strs">参数</param>
    /// <param name="cmdArgsCount">cmd本身参数数量</param>
    public static void CombineArgsParts(ref string[] strs, int cmdArgsCount)
    {
        //salad repeat I'm stupid
        //-> I'm stupid
        //-> cmdArgsCount = 1
        var lst = strs[cmdArgsCount - 1];
        for (int i = cmdArgsCount; i < strs.Length; i++)
        {
            lst += " " + strs[i];
        }
        strs[cmdArgsCount - 1] = lst;
    }

}
