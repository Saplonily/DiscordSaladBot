using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Discord;

public class TictocGameSet : ICommandSetGuildSeparated
{
    public IUserGuild GuildIn { get; private set; }

    public string SetName { get => "tictoc"; }

    public List<ICommandSet> ChildCommandSets { get => null; }

    public List<Command> ChildCommands { get; private set; }

    public bool IsSet { get => true; }

    public ICommandSet BelongTo { get; private set; }

    public TictocGameSet(IUserGuild guild, ICommandSet belongTo)
    {
        GuildIn = guild;
        BelongTo = belongTo;
    }
    /*
    Command.CheckingCommands.AddRange(new List<Command>()
        {
            Command.Create("join",0,(args,msg) => {
                tictoc.Join(msg.Author);
            }),
            Command.Create("leave",0,(args,msg) => {
                tictoc.Leave(msg.Author);
            }),
            Command.Create("counts",1,(args,msg) => {
                tictoc.counts = int.Parse(args[0]);
                tictoc.Msg($"*Counts to win* has been set to {tictoc.counts}");
            }),
            Command.Create("check",0,(args,msg) => {
                if (tictoc.GameIsRunning)
                {
                    tictoc.ShowGamePad();
                }
                else
                {
                    tictoc.Msg("Game have not been started!");
                }
            }),
            Command.Create("start",0,(args,msg) => {
                tictoc.Start();
            }),
            Command.Create("place",2,(args,msg)=>{
                tictoc.Place(int.Parse(args[0]) - 1, int.Parse(args[1]) - 1, msg.Author);
            }),
            Command.Create("set_size",2,(args,msg)=>{
                tictoc.SetSize(int.Parse(args[0]),int.Parse(args[1]));
            }),
            Command.Create("end",0,(args,msg)=>{
                tictoc.End();
                tictoc = null;
            }),
            Command.Create("list",0,(args,msg)=>
            {
                var str = "Players playing: ";
                foreach (var item in tictoc.players)
                {
                    str += item.Username + " ";
                }
                tictoc.Msg(str);
            })
        });*/
    
}