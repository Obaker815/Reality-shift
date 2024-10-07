using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public enum PlayerState
{
    Idle,
    Walking,
    Jumping
}

public class Player
{
    private Texture2D spritesheet;
    private Vector2 position;

    private int frameWidth;
    private int frameHeight;
    private int currentFrame;
    private int totalFrames;

    private float frameTime;
    private float elapsedTime;

    public PlayerState CurrentState { get; set; }
    private bool isFacingRight; // New variable to track the facing direction

    public Player(Texture2D texture, Vector2 startPosition, int frameWidth, int frameHeight, int totalFrames, float frameTime)
    {
        this.spritesheet = texture;
        this.position = startPosition;
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
        this.totalFrames = totalFrames;
        this.frameTime = frameTime;
        this.elapsedTime = 0f;
        this.currentFrame = 0;
        this.CurrentState = PlayerState.Idle;
        this.isFacingRight = true; // Initially facing right
    }

    public void Update(GameTime gameTime)
    {
        // Handle input for player movement and change state
        var keyboardState = Keyboard.GetState();

        // Example movement logic
        if (keyboardState.IsKeyDown(Keys.A))
        {
            position.X -= 5f;
            CurrentState = PlayerState.Walking;
            isFacingRight = false; // Facing left
        }
        else if (keyboardState.IsKeyDown(Keys.D))
        {
            position.X += 5f;
            CurrentState = PlayerState.Walking;
            isFacingRight = true; // Facing right
        }
        else
        {
            CurrentState = PlayerState.Idle;
        }

        // Update the animation frame timing
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
        Rectangle sourceRectangle = new Rectangle(column * frameWidth + 1, currentFrame * frameHeight, frameWidth - 1, frameHeight);

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
