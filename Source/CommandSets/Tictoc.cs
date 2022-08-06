using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using System.Linq;
using System.Numerics;

namespace SaladBot.Sets;

public partial class TictocGameSet
{
    public class Tictoc
    {
        public Tictoc() { }

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
                    2 => ":white_check_mark:",
                    _ => "-1"
                };
            }
        }

        public ISocketMessageChannel playingChannel;

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
                    playingChannel.SendMessageAsync($"{user.Mention} joined the game!");
                }
                else
                {
                    playingChannel.SendMessageAsync($"{user.Mention}, you have joined the game.");
                }
            }
            else
            {
                playingChannel.SendMessageAsync($"{user.Mention}, no more seats for playing!");
            }
        }

        public void Leave(SocketUser user)
        {
            if (players.Contains(user))
            {
                playingChannel.SendMessageAsync($"{user.Mention}, you have left the game!");
                players.Remove(user);
            }
            else
            {
                playingChannel.SendMessageAsync($"{user.Mention}, you haven't joined the game!");
            }
        }

        public Tictoc(ISocketMessageChannel channel)
        {
            playingChannel = channel;
        }

        public SocketUser Place(int width, int height, SocketUser user)
        {
            if (currentPlayer == user)
            {
                if (grids[height, width].status == -1)
                {
                    grids[height, width].status = players.FindIndex((u) => u.Username == user.Username);
                    ShowGamePad();
                    var winner = CheckWinner();
                    if (CheckWinner() is not null)
                    {
                        return winner;
                    }
                    if (GameIsRunning)
                    {
                        var ind = players.FindIndex((i) => i == currentPlayer);

                        if (players.Count - 1 == ind)
                            ind = 0;
                        else
                            ind++;
                        currentPlayer = players[ind];
                        playingChannel.SendMessageAsync($"{currentPlayer.Mention}, it's not your turn!");
                    }
                    return null;
                }
                else
                {
                    playingChannel.SendMessageAsync("You can't place there!");
                }
            }
            else
            {
                playingChannel.SendMessageAsync($"{user.Mention}, it's not your turn!");
            }
            return null;
        }

        public void SetSize(int width, int height)
        {
            size = (width, height);
            playingChannel.SendMessageAsync("Size has been set.");
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
            playingChannel.SendMessageAsync($"Here is the game status:\n{this.GetGameString()}");
        }

        public void End()
        {
            this.GameIsRunning = false;
        }

        public void Start()
        {
            if(players.Count != 0)
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
            else
            {
                playingChannel.SendMessageAsync("No players in the game!");
            }
        }

        public SocketUser CheckWinner()
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
                    var win4 = true;
                    for (int i = 0; i < counts; i++)
                    {
                        if (w + counts > size.width || grids[w + i, h].status != aim)
                            win1 = false;
                        if (h + counts > size.height || grids[w, h + i].status != aim)
                            win2 = false;
                        if (h + counts > size.height || w + counts > size.width || grids[w + i, h + i].status != aim)
                            win3 = false;
                        if (h - counts < 0 || w - counts < 0 || grids[w - i, h - i].status != aim)
                            win4 = false;
                    }
                    if (win1 || win2 || win3 || win4)
                        goto wined;

                }
            }
            return null;
        wined:
            return this.players[aim];
        }
    }
}