using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Reality_shift;
using SharpDX.Direct2D1.Effects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;


public static class TileList
{
    public static List<Tile> tiles = new List<Tile>();
}

public class Tile
{
    private Texture2D spritesheet;
    private Vector2 position;
    private Color[,] level;

    private int frameWidth;
    private int frameHeight;
    private int column;
    private int row;
        
    public Tile(Texture2D spritesheet, Vector2 position, int frameWidth, int frameheight)
    {
        this.spritesheet = spritesheet;
        this.position = position;
        this.frameWidth = frameWidth;
        this.frameHeight = frameheight;
        TileList.tiles.Add(this);
    }

    public void CalculateTile(Color[,] load)
    {
        this.level = load;



    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Calculate the source rectangle for the current frame
        Rectangle sourceRectangle = new Rectangle(column * frameWidth + 1, row * frameHeight, frameWidth - 1, frameHeight);

        // Draw the current frame of the player with the appropriate sprite effect
        spriteBatch.Draw(spritesheet, position, sourceRectangle, Color.White);
    }

}

