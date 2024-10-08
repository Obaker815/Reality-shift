using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

    public PlayerState CurrentState { get; set; }
    private bool isFacingRight;
    private bool grounded;

    public Player(Texture2D texture, Vector2 startPosition, int frameWidth, int frameHeight, int totalFrames, float frameTime)
    {
        this.spritesheet = texture;
        this.position = startPosition;
        this.velocity.X = 0f;
        this.velocity.Y = 0f;
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
        this.totalFrames = totalFrames;
        this.frameTime = frameTime;
        this.elapsedTime = 0f;
        this.currentFrame = 0;
        this.CurrentState = PlayerState.Idle;
        this.isFacingRight = true; // Initially facing right
        this.grounded = true;
    }

    public void Update(GameTime gameTime)
    {
        // Handle input for player movement and change state
        var keyboardState = Keyboard.GetState();

        // Example movement logic
        if (keyboardState.IsKeyDown(Keys.A))
        {
            velocity.X -= 5f;
            CurrentState = PlayerState.Walking;
            isFacingRight = false; // Facing left
        }
        else if (keyboardState.IsKeyDown(Keys.D))
        {
            velocity.X += 5f;
            CurrentState = PlayerState.Walking;
            isFacingRight = true; // Facing right
        }
        else if (keyboardState.IsKeyDown(Keys.Space))
        {
            velocity.Y += 10f;
        }
        else
        {
            CurrentState = PlayerState.Idle;
        }

        if (!grounded) CurrentState = PlayerState.Airborne;

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
