using System;
using System.Drawing;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace SaladBot.Sets;

using SysColor = System.Drawing.Color;

public partial class SaladSet
{
    [Command("random_color")]
    public async Task RandomColor(string[] args, SocketMessage msg)
    {
        Random r = new Random();
        var R = r.Next(0,255);
        var G = r.Next(0,255);
        var B = r.Next(0,255);
        
        await SendColor(Color.FromArgb(R,G,B),msg);

        await msg.Channel.SendMessageAsync(
            $"The color is: rgb({R},{G},{B}) hex:#" + $"{R.ToString("x")}{G.ToString("x")}{B.ToString("x")}".ToUpper()
            );
    }

    [Command("get_color",3)]
    public async void GetColor(string[] args,SocketMessage msg)
    {
        int R = int.Parse(args[0]);
        int G = int.Parse(args[1]);
        int B = int.Parse(args[2]);
        
        await SendColor(Color.FromArgb(R,G,B),msg);

        await msg.Channel.SendMessageAsync(
            $"The color is: rgb({R},{G},{B}) hex:#"+
            $"{R.ToString("x")}{G.ToString("x")}{B.ToString("x")}".ToUpper()
            );
    }

    public Task SendColor(SysColor color,SocketMessage msg)
    {
        return Task.Factory.StartNew(()=>
        {
            Bitmap bitmap = new Bitmap(100,100);
            Graphics g = Graphics.FromImage(bitmap);
            g.DrawRectangle(new Pen(color,100),new Rectangle(0,0,100,100));
            var fileName = $"tempImages\\{msg.Id}.png";
            bitmap.Save(fileName);
            msg.Channel.SendFileAsync(fileName);
        });
    }
}