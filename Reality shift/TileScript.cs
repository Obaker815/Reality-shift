using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

public static class TileList
{
    public static List<Tile> tiles = new List<Tile>();
    public static Color bgColor;
}

public class Tile
{
    Keys realityBind = Keys.LeftShift;

    private Texture2D spritesheet;
    private Vector2 position; // This will be the tile's physical position
    private int[] TexturePos;

    private int layerOffset = 1200;
    private int frameWidth;
    private int frameHeight;
    private int column;
    private int row;

    private int x;
    private int y;

    bool goingUp = true;
    bool EDown;
    int alt;

    public Tile(Texture2D spritesheet, Vector2 position, int frameWidth, int frameHeight, int[] levelPos, int alt)
    {
        this.alt = alt;
        this.spritesheet = spritesheet;
        this.position = position; // Set initial position
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
        this.TexturePos = levelPos;

        TileList.tiles.Add(this);
    }

    public int FrameWidth { get { return frameWidth; } }
    public int FrameHeight { get { return frameHeight; } }
    public Vector2 Position { get { return position; } }

    public void CalculateConnections(Color[,] level)
    {
        x = TexturePos[0];
        y = TexturePos[1];

        // Define connection states
        bool up = y > 0 && level[x, y - 1] == level[x, y];
        bool down = y < level.GetLength(1) - 1 && level[x, y + 1] == level[x, y];
        bool left = x > 0 && level[x - 1, y] == level[x, y];
        bool right = x < level.GetLength(0) - 1 && level[x + 1, y] == level[x, y];

        // + pieces (all connections)
        if (up && down && left && right)
        {
            column = 3;
            row = 1;
            return;
        }

        // T pieces
        if (up && down && right)
        {
            column = 2;
            row = 1;
            return;
        }
        if (up && down && left)
        {
            column = 4;
            row = 1;
            return;
        }
        if (left && right && up)
        {
            column = 3;
            row = 2;
            return;
        }
        if (left && right && down)
        {
            column = 3;
            row = 0;
            return;
        }

        // Straight middle pieces
        if (up && down)
        {
            column = 1;
            row = 1;
            return;
        }
        if (left && right)
        {
            column = 6;
            row = 0;
            return;
        }

        // Corner pieces
        if (right && down)
        {
            column = 2;
            row = 0;
            return;
        }
        if (left && down)
        {
            column = 4;
            row = 0;
            return;
        }
        if (right && up)
        {
            column = 2;
            row = 2;
            return;
        }
        if (left && up)
        {
            column = 4;
            row = 2;
            return;
        }

        // Straight ends
        if (up)
        {
            column = 1;
            row = 2;
            return;
        }
        if (down)
        {
            column = 1;
            row = 0;
            return;
        }
        if (left)
        {
            column = 7;
            row = 0;
            return;
        }
        if (right)
        {
            column = 5;
            row = 0;
        }
    }

    public void Update(Vector2 cameraOffset)
    {
        // Update the tile's position based on the camera offset
        position -= cameraOffset; // Move tile according to camera

        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(realityBind) && !EDown)
        {
            if (goingUp)
            {
                position.Y -= layerOffset;
                EDown = true;
                goingUp = false;
                TileList.bgColor = Color.MediumAquamarine;
            }
            else
            {
                position.Y += layerOffset;
                EDown = true;
                goingUp = true;
                TileList.bgColor = Color.Coral;
            }
        }

        if (EDown && !keyboardState.IsKeyDown(realityBind))
        {
            EDown = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Calculate the source rectangle for the current frame
        Rectangle sourceRectangle = new Rectangle(column * frameWidth, (row * frameHeight + (alt * frameHeight * 3)), frameWidth, frameHeight);

        // Draw the tile with the appropriate sprite effect
        spriteBatch.Draw(spritesheet, position, sourceRectangle, Color.White);
    }
}
