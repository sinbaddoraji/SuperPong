using Microsoft.Xna.Framework;

namespace PingPong.Implementation.GameMenuScreen;

public class MenuItem : GameEntity.GameEntity
{
    public string Text { get; set; }

    public Color SelectColor { get; set; }

    public Color DefaultColor { get; set; }
}