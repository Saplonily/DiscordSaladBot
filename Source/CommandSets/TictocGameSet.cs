using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using System.Linq;
using System.Numerics;

public class TictocGameSet
{
    public class ChessGrid
    {
        //-1为空,其他为players列表里的序号
        public int status = -1;

        public string Sign
        {
            get => status switch
            {
                -1 => ":white_square_button:",
                0 => "❌",
                1 => "⭕",
                2 => "<:WTF_thinking:991165471825076254>",
                _ => "-1"
            };
        }
    }

    ISocketMessageChannel playingChannel;

    public List<SocketUser> players = new List<SocketUser>();

    SocketUser currentPlayer;

    public bool GameIsRunning = false;

    public ChessGrid[,] grids = { };
    
    //游戏规则,几个子
    public int counts = 3;
    //游戏棋盘大小
    public (int width, int height) size = (3, 3);

    //fucking commands:
    

    public void Join(SocketUser user)
    {
        if (players.Count < 3)
        {
            if (!players.Contains(user))
            {
                players.Add(user);
                Msg($"{user.Mention} joined the game!");
            }
            else
            {
                Msg($"{user.Mention}, you have joined the game.");
            }
        }
        else
        {
            Msg($"{user.Mention}, no more seats for playing!");
        }
    }

    public void Leave(SocketUser user)
    {
        if (players.Contains(user))
        {
            Msg($"{user.Mention}, you have left the game!");
            players.Remove(user);
        }
        else
        {
            Msg($"{user.Mention}, you haven't joined the game!");
        }
    }

    public TictocGameSet(ISocketMessageChannel channel)
    {
        playingChannel = channel;
    }

    public void Place(int width, int height, SocketUser user)
    {
        if (currentPlayer == user)
        {
            if (grids[height, width].status == -1)
            {
                grids[height, width].status = players.FindIndex((u) => u.Username == user.Username);
                ShowGamePad();
                CheckWinner();
                if (GameIsRunning)
                {
                    var ind = players.FindIndex((i) => i == currentPlayer);

                    if (players.Count - 1 == ind)
                        ind = 0;
                    else
                        ind++;
                    currentPlayer = players[ind];
                    Msg($"{currentPlayer.Mention}, it's your turn!");
                }
            }
            else
            {
                Msg("You can't place there!");
            }
        }
        else
        {
            Msg($"{user.Mention}, it's not your turn!");
        }
    }

    public void SetSize(int width, int height)
    {
        size = (width, height);
        Msg("Size has been set.");
    }

    public string GetGameString()
    {
        var str = "";
        for (int h = 0; h < size.height; h++)
        {
            str += $"---  ";
            for (int w = 0; w < size.width; w++)
            {
                str += grids[w, h].Sign + " ";
            }
            str += "\n";
        }
        return str;
    }

    public void ShowGamePad()
    {
        Msg($"Here is the game status:\n{this.GetGameString()}");
    }

    public void End()
    {
        this.GameIsRunning = false;
        Msg("Game ended!");
    }

    public void Msg(string msg)
    {
        playingChannel.SendMessageAsync(msg);
    }

    public void Start()
    {
        currentPlayer = players[0];
        grids = new ChessGrid[size.width, size.height];
        for (int h = 0; h < size.height; h++)
        {
            for (int w = 0; w < size.width; w++)
            {
                grids[w, h] = new ChessGrid();
            }
        }
        GameIsRunning = true;
        ShowGamePad();
    }

    public void CheckWinner()
    {
        //判断赢了没
        int aim;
        for (int h = 0; h < size.height; h++)
        {
            for (int w = 0; w < size.width; w++)
            {
                aim = grids[w, h].status;
                if (aim == -1) continue;
                var win1 = true;
                var win2 = true;
                var win3 = true;
                for (int i = 0; i < counts; i++)
                {
                    if (w + counts > size.width || grids[w + i, h].status != aim)
                        win1 = false;
                    if (h + counts > size.height || grids[w, h + i].status != aim)
                        win2 = false;
                    if (h + counts > size.height || w + counts > size.width || grids[w + i, h + i].status != aim)
                        win3 = false;
                }
                if (win1 || win2 || win3)
                    goto wined;

            }
        }
        return;
    wined:
        this.Msg($"{this.players[aim].Mention} win the game!");
        this.End();


    }
}