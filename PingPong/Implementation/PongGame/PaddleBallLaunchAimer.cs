using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PingPong.Interface;
using PingPong.SimpleSprite;
using System;
using nkast.Aether.Physics2D.Dynamics;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;

namespace PingPong.Implementation.PongGame;

public class PaddleBallLaunchAimer : PongGameEntity
{
    private readonly GraphicsDevice _graphics;
    private readonly Color _color;
    private int _length;
    private int _angle;
    private readonly bool _isPointingUpwards = false;

    private readonly int _effectiveMinAngle = 0;
    private readonly int _effectiveMaxAngle = 0;

    private readonly int _maxHeight = 100;
    private readonly int _minHeight = 50;

    // Release the ball event
    public delegate void BallLaunchHandler(Vector2 launchDirection);
    public event BallLaunchHandler OnBallLaunch;

    readonly IGameScreenControllerManager _gameScreenControllerManager;

    private const float UnitToPixel = 100f; // Adjust as needed
    private const float PixelToUnit = 1f / UnitToPixel;

    public PaddleBallLaunchAimer(GraphicsDevice graphics, World world, IGameScreenControllerManager gameScreenControllerManager, Color color, int length, int angle,bool isPointingUpwards = true) : base(world)
    {
        _graphics = graphics;
        _color = color;
        _length = length;
        _angle = angle;
        _isPointingUpwards = isPointingUpwards;
        // Create paddle texture

        _gameScreenControllerManager = gameScreenControllerManager;


        OnBallLaunch += (launchDirection) =>
        {
            // Launch the ball
        };

        if (_isPointingUpwards)
        {
             _angle = 180;
            _effectiveMinAngle = 115;
            _effectiveMaxAngle = 245;

            // TODO: Comment the below out
            //effectiveMinAngle = int.MinValue;
            //effectiveMaxAngle = int.MaxValue;
        }
        else
        {
            _angle = 0;
        }

        length = 200;
        Texture = LineTexture.CreateLineTexture(_graphics, _color, length);
    }

    public void ChangeColor(Color color)
    {
        Texture = LineTexture.CreateLineTexture(_graphics, color, _length);
    }

    
    public void AimLeft()
    {
        if (_isPointingUpwards)
        {
            if (_angle <= _effectiveMinAngle)
            {
                return;
            }
        }

        _angle -= 1;

        // rotate the texture to point left
        
    }

    public void AimRight()
    {
        if (_isPointingUpwards)
        {
            if (_angle >= _effectiveMaxAngle)
            {
                return;
            }
        }
        _angle += 1;
    }

    private void ExtendAimLine()
    {
        if(_length < _maxHeight)
             _length++;
        Redraw();
    }

    private void ReduceAimLine()
    {
        if (_length > _minHeight)
            _length--;

        Redraw();
    }

    private void Redraw()
    {
        Texture = LineTexture.CreateLineTexture(_graphics, _color, _length);
    }

    public void LaunchBall()
    {
        // Launch the ball based on angle and rotation of the Texture
        Vector2 launchDirection = _isPointingUpwards 
            ? new Vector2((float)Math.Cos(MathHelper.ToRadians(_angle)), (float)Math.Sin(MathHelper.ToRadians(_angle))) 
            : new Vector2((float)Math.Cos(MathHelper.ToRadians(_angle)), (float)Math.Sin(MathHelper.ToRadians(_angle)));

        launchDirection *= PixelToUnit;

        OnBallLaunch.Invoke(launchDirection);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
    {
        spriteBatch.Draw(Texture, Position, null, color, MathHelper.ToRadians(_angle), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
    }


    public override void InitializePhysics(Vector2 initialPosition)
    {
        // throw new NotImplementedException();
    }

    public new void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        if (!_isPointingUpwards)
        {
            if (_gameScreenControllerManager.PlayerTwoKeyUp())
            {
                ExtendAimLine();
            }

            if (_gameScreenControllerManager.PlayerTwoKeyDown())
            {
                ReduceAimLine();
            }

            if (_gameScreenControllerManager.PlayerTwoKeyLeft())
            {
                AimLeft();
            }

            if (_gameScreenControllerManager.PlayerTwoKeyRight())
            {
                AimRight();
            }

            if (_gameScreenControllerManager.PlayerTwoKeyAction())
            {
                LaunchBall();
            }
        }
        else
        {
            if (_gameScreenControllerManager.PlayerOneKeyUp())
            {
                ExtendAimLine();
            }

            if (_gameScreenControllerManager.PlayerOneKeyDown())
            {
                ReduceAimLine();
            }

            if (_gameScreenControllerManager.PlayerOneKeyLeft())
            {
                AimLeft();
            }

            if (_gameScreenControllerManager.PlayerOneKeyRight())
            {
                AimRight();
            }

            if (_gameScreenControllerManager.PlayerOneKeyAction())
            {
                LaunchBall();
            }
        }
    }
}