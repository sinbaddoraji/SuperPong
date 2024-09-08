using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PingPong.Enum;

namespace PingPong.Model.PongGame
{
    public class PongGameStartOptions
    {
        public Color Player1Color { get; set; }

        public Color Player2Color { get; set; }

        public GameMode GameMode { get; set; }
    }
}
