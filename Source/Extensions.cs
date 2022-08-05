
using System.Collections.Generic;
using Discord.WebSocket;

namespace SaladBot;

public static class Extensions
{
    public static SocketGuild GetGuildIn(this SocketMessage msg)
    {
        return (msg.Channel as SocketTextChannel)?.Guild;
    }

    public static T GetDataIns<T>(this SocketMessage msg,IDictionary<SocketGuild,T> dic) where T : new()
    {
        var guild = msg.GetGuildIn();
        if(dic.ContainsKey(guild))
        {
            return dic[guild];
        }
        else
        {
            var ins = new T();
            dic.Add(guild,ins);
            return ins;
        }
    }
}