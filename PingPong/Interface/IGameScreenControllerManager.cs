using Microsoft.Xna.Framework;

namespace PingPong.Interface;

public interface IGameScreenControllerManager
{
    public double CooldownPeriod { get; set; }
    bool PlayerOneKeyUp();

    bool PlayerOneKeyDown();

    bool PlayerOneKeyLeft();

    bool PlayerOneKeyRight();

    bool PlayerOneKeyAction();

    bool PlayerTwoKeyUp();

    bool PlayerTwoKeyDown();

    bool PlayerTwoKeyLeft();

    bool PlayerTwoKeyRight();

    bool PlayerTwoKeyAction();

    bool AnyPlayerKeyUp();

    bool AnyPlayerKeyDown();

    bool AnyPlayerKeyLeft();

    bool AnyPlayerKeyRight();

    bool AnyPlayerKeyAction();

    void Update(GameTime gameTime);
}