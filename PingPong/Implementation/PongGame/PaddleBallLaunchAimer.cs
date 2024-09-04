using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PingPong.SimpleSprite;
using System;

namespace PingPong.Implementation.PongGame;

public class PaddleBallLaunchAimer : PongGameEntity
{
    private readonly float _inputCooldown = 0.2f; // 200ms cooldown for input
    private float _timeSinceLastInput;
    private readonly GraphicsDevice _graphics;
    private readonly Color _color;
    private readonly int _length;
    private int _angle;
    private bool _isPointingUpwards = false;

    private int effectiveMinAngle = 0;
    private int effectiveMaxAngle = 0;

    public PaddleBallLaunchAimer(GraphicsDevice graphics, Color color, int length, int angle,bool isPointingUpwards = true)
    {
        _graphics = graphics;
        _color = color;
        _length = length;
        _angle = angle;
        _isPointingUpwards = isPointingUpwards;
        // Create paddle texture

        if (_isPointingUpwards)
        {
            _angle = 180;
            effectiveMinAngle = 135;
            effectiveMaxAngle = 225;

            Position = new Vector2(Position.X, Position.Y + 500);
        }
        else
        {
            
        }
        Texture = LineTexture.CreateLineTexture(_graphics, _color, length);
    }

    
    private void HandleInput(Keys key, ButtonState buttonState, float thumbstickDirection, Action moveAction, float threshold = 0.5f)
    {
        var keyboardState = Keyboard.GetState();

        if ((_timeSinceLastInput <= 0) && (keyboardState.IsKeyDown(key) || buttonState == ButtonState.Pressed || Math.Abs(thumbstickDirection) > threshold))
        {
            moveAction();
        }

        // Check select option
        if ((_timeSinceLastInput <= 0) && (keyboardState.IsKeyDown(key) || buttonState == ButtonState.Pressed || Math.Abs(thumbstickDirection) > threshold))
        {
            // Release the ball
        }
    }

    public void AimLeft()
    {
        if (_isPointingUpwards)
        {
            if (_angle <= effectiveMinAngle)
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
            if (_angle >= effectiveMaxAngle)
            {
                return;
            }
        }
        _angle += 1;
    }

    public void LaunchBall()
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
    {
        spriteBatch.Draw(Texture, Position, null, color, MathHelper.ToRadians(_angle), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
    }

    public override void Update(GameTime gameTime)
    {
       
        // TODO: Add logic that checks if the controller is for player 1 or 2
        var gamePadState = GamePad.GetState(PlayerIndex.One);

        if (_timeSinceLastInput > 0)
        {
            _timeSinceLastInput -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (IsActive)
        {
            HandleInput(Keys.Left, gamePadState.DPad.Down, gamePadState.ThumbSticks.Left.Y < -0.5 ? -1 : 0, AimLeft);
            HandleInput(Keys.Right, gamePadState.DPad.Up, gamePadState.ThumbSticks.Left.Y > 0.5 ? 1 : 0, AimRight);
            HandleInput(Keys.Enter, gamePadState.Buttons.A, gamePadState.Triggers.Right, LaunchBall, 0.5f);
            //UpdateSprite();
        }
    }
}