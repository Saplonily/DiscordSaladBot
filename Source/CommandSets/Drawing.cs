using System.Collections.Generic;
using Discord;
using System.Drawing;
using Discord.WebSocket;

namespace SaladBot.Sets;

public partial class DrawingSet : ICommandSet
{
    public class Drawing
    {
        public List<SocketUser> AccessibleUser { get; set; } = new List<SocketUser>();
        public Bitmap Bitmap { get; init; }
        public Graphics Graphics { get; init; }

        public Drawing()
        {
            Bitmap = new Bitmap(100,100);
            Graphics = Graphics.FromImage(Bitmap);
        }

        public void GenerateImageFile(string fileName)
        {
            Bitmap.Save(fileName);
        }
    }
}