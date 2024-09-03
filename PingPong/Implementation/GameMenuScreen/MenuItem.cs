using Microsoft.Xna.Framework;
using PingPong.Implementation.GameEntitiy;

namespace PingPong.Implementation.GameMenuScreen;

public class MenuItem : GameEntity
{
    public string Text { get; set; }

    public Color SelectColor { get; set; }

    public Color DefaultColor { get; set; }
}