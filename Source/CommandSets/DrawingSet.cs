using System.Drawing;
using System.Diagnostics.Tracing;
using System;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

using SysColor = System.Drawing.Color;

namespace SaladBot.Sets;

public partial class DrawingSet : ICommandSet
{
    public string SetName { get => "drawing"; }
    public List<ICommandSet> ChildCommandSets { get => null; }
    public List<Command> ChildCommands { get; private set; }
    public bool IsSet { get => true; }
    public ICommandSet BelongTo { get; init; }
    public Dictionary<string, Drawing> Drawings { get; } = new Dictionary<string, Drawing>();

    public DrawingSet(ICommandSet belongTo)
    {
        BelongTo = belongTo;
        ChildCommands = CommandSetHelper.GetCommands(this);
    }

    [Command("show", 1)]
    public void Show(string[] args, SocketMessage msg)
    {
        var fileName = @$"tempImages\{msg.Id}_temp.png";
        GetDrawingIfExists(args[0]).GenerateImageFile(fileName);
        msg.Channel.SendFileAsync(fileName);
    }

    [Command("draw_line", 4)]
    public void DrawLine(string[] args, SocketMessage msg)
    {
        var point1str = args[1].Split(',');
        var point2str = args[2].Split(',');
        var point1 = new Point(int.Parse(point1str[0]),int.Parse(point1str[1]));
        var point2 = new Point(int.Parse(point2str[0]),int.Parse(point2str[1]));
        var color = SysColor.FromName(args[0]);
        GetDrawingIfExists(args[3]).Graphics.DrawLine(new Pen(color,5),point1,point2);
    }

    /// <summary>
    /// 获取一个Draw,没有的话就创建
    /// </summary>
    /// <param name="drawingName"></param>
    /// <returns></returns>
    public Drawing GetDrawingIfExists(string drawingName)
    {
        if (Drawings.ContainsKey(drawingName))
        {
            return Drawings[drawingName];
        }
        else
        {
            var draw = new Drawing();
            Drawings.Add(drawingName, draw);
            return draw;
        }
    }
}