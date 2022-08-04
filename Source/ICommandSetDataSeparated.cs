using System;
using Discord;

public interface ICommandSetGuildSeparated : ICommandSet
{
    IUserGuild GuildIn { get; }
}