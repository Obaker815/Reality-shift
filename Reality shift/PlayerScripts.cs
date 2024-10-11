using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reality_shift;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;

public enum PlayerState
{
    Idle,
    Walking,
    Airborne
}

public class Player
{
    private Texture2D spritesheet;
    private Vector2 position;

    private Vector2 velocity;

    private int frameWidth;
    private int frameHeight;
    private int currentFrame;
    private int totalFrames;

    private float frameTime;
    private float elapsedTime;

    // Gravity constants
    private const float Gravity = 0.5f; // Gravity force
    private const float JumpForce = -13.5f; // Jump force

    public PlayerState CurrentState { get; set; }
    private bool isFacingRight;
    private bool grounded;
    private bool lastFrameGrounded;

    public Player(Texture2D texture, Vector2 startPosition, int frameWidth, int frameHeight, int totalFrames, float frameTime)
    {
        this.spritesheet = texture;
        this.position = new Vector2(startPosition.X + 30, startPosition.Y);
        this.velocity = Vector2.Zero;
        this.frameWidth = frameWidth - 60;
        this.frameHeight = frameHeight;
        this.totalFrames = totalFrames;
        this.frameTime = frameTime;
        this.elapsedTime = 0f;
        this.currentFrame = 0;
        this.CurrentState = PlayerState.Idle;
        this.isFacingRight = true;
        this.grounded = false;
        this.lastFrameGrounded = false;
    }

    public void Update(GameTime gameTime, Vector2 windowSize)
    {
        List<Tile> tiles = TileList.tiles;
        var keyboardState = Keyboard.GetState();

        bool scrolling = (position.X > (windowSize.X / 80 * 100) - 200 - frameWidth && isFacingRight) || (position.X < 200 && !isFacingRight);

        // Apply friction only if grounded
        if (grounded)
        {
            if (0f < velocity.X && velocity.X < 0.2f) velocity.X = 0f;
            if (0f > velocity.X && velocity.X > -0.2f) velocity.X = 0f;
            if (velocity.X > 0f) velocity.X -= 0.1f;
            if (velocity.X < 0f) velocity.X += 0.1f;
        }

        // Handle input for player movement
        if (keyboardState.IsKeyDown(Keys.A))
        {
            if (isFacingRight) velocity.X *= -1; // Flip velocity if facing the wrong way
            if (velocity.X > -5f) velocity.X -= 1f;

            CurrentState = PlayerState.Walking;
            isFacingRight = false; // Facing left
        }
        else if (keyboardState.IsKeyDown(Keys.D))
        {
            if (!isFacingRight) velocity.X *= -1; // Flip velocity if facing the wrong way
            if (velocity.X < 5f) velocity.X += 1f;

            CurrentState = PlayerState.Walking;
            isFacingRight = true; // Facing right
        }
        else
        {
            CurrentState = PlayerState.Idle;
        }

        // Jumping logic
        if (keyboardState.IsKeyDown(Keys.Space) && grounded)
        {
            velocity.Y = JumpForce; // Set upward velocity
            grounded = false; // Player is now airborne
        }

        // Apply gravity
        if (!grounded)
        {
            velocity.Y += Gravity; // Apply gravity force to vertical velocity
        }

        // Hard cap velocity
        velocity.X = MathHelper.Clamp(velocity.X, -5f, 5f);

        Vector2 nextPosition = Vector2.Zero;

        // Check for collision before updating position
        if (scrolling)
        {
            foreach (Tile tile in TileList.tiles)
            {
                tile.NextPosition = new Vector2(tile.Position.X - velocity.X, tile.NextPosition.Y);
            }
            nextPosition = Position;
        }
        else
        {
            nextPosition.X = position.X + velocity.X;
        }

        nextPosition.Y = position.Y + velocity.Y;

        // Check for collision with tiles
        bool collision = false;
        bool collisionX = false;
        bool collisionY = false;

        foreach (Tile tile in tiles)
        {
            Rectangle playerRect = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, frameWidth, frameHeight);
            Rectangle tileRect = new Rectangle((int)tile.NextPosition.X, (int)tile.NextPosition.Y, tile.FrameWidth, tile.FrameHeight);

            if (playerRect.Intersects(tileRect))
            {
                float difRL = Math.Abs(nextPosition.X + frameWidth - tile.NextPosition.X);
                float difLR = Math.Abs(nextPosition.X - tile.NextPosition.X - tile.FrameWidth);
                float difBT = Math.Abs(nextPosition.Y + frameHeight - tile.NextPosition.Y);
                float difTB = Math.Abs(nextPosition.Y - tile.NextPosition.X - tile.FrameHeight);

                float minDif = Math.Min(Math.Min(Math.Min(difRL, difLR), difBT), difTB);

                if (minDif == difRL) nextPosition.X = tile.NextPosition.X - frameWidth;         collisionX = true;  velocity.X = 0;
                if (minDif == difLR) nextPosition.X = tile.NextPosition.X + tile.FrameWidth;    collisionX = true;  velocity.X = 0;
                if (minDif == difBT) nextPosition.Y = tile.NextPosition.Y - frameHeight;        collisionY = true;  velocity.Y = 0; grounded = true;
                if (minDif == difTB) nextPosition.Y = tile.NextPosition.Y + tile.FrameHeight;   collisionY = true;  velocity.Y = 0; grounded = true;
            }
        }
        // Only apply velocity if no collision
        if (!collision)
        {
            if (!collisionX)
            {
                if (scrolling)
                {
                    foreach (Tile tile in TileList.tiles)
                    {
                        tile.Position = tile.NextPosition;
                    }
                }
                else
                {
                    position.X = nextPosition.X;
                }
            }
            if (!collisionY)
            {
                position.Y = nextPosition.Y;
            }
        }

        // Check if there is a platform 1 pixel below the player
        Rectangle checkRect = new Rectangle((int)position.X, (int)position.Y + frameHeight, frameWidth, 1);
        bool hasPlatformBelow = false; // Track if there's a platform below

        foreach (Tile tile in tiles)
        {
            Rectangle tileRect = new Rectangle((int)tile.NextPosition.X, (int)tile.NextPosition.Y, tile.FrameWidth, tile.FrameHeight);
            if (checkRect.Intersects(tileRect))
            {
                hasPlatformBelow = true; // Found a platform below
                break; // Exit loop if we found a tile
            }
        }

        // Set grounded status based on the presence of a tile 10 pixels below
        if (!hasPlatformBelow)
        {
            grounded = false; // Only unground if there's no tile below
        }

        // Update state based on grounded status
        if (!grounded)
        {
            CurrentState = PlayerState.Airborne;
        }

        if (lastFrameGrounded && !grounded)
        {
            position.X += 30; frameWidth -= 60;
            lastFrameGrounded = false;
        } 
        else if (!lastFrameGrounded && grounded)
        {
            position.X -= 30; frameWidth += 60;
            lastFrameGrounded = true;
        }

        // Update animation
        elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (elapsedTime >= frameTime)
        {
            currentFrame++;
            if (currentFrame >= totalFrames)
            {
                currentFrame = 0; // Loop back to the first frame
            }
            elapsedTime = 0f; // Reset the timer
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        // Calculate the source rectangle for the current frame
        int column = (int)CurrentState; // Use enum value to determine column
        Rectangle sourceRectangle = new Rectangle(column * frameWidth + 1, currentFrame * frameHeight + 1, frameWidth -2, frameHeight -2);

        // Determine sprite effect based on the facing direction
        SpriteEffects spriteEffect = isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        // Draw the current frame of the player with the appropriate sprite effect
        spriteBatch.Draw(spritesheet, position, sourceRectangle, Color.White, 0f, Vector2.Zero, Vector2.One, spriteEffect, 0f);
    }

    public Vector2 Position
    {
        get { return position; }
    }
}
