using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Reality_shift
{
    public class Game1 : Game
    {
        private Texture2D CurrentLevel;
        public Color[,] level;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _playerSpriteSheet;
        private Texture2D _tileSpriteSheet;
        private Player player;
        private float scale = 1f;  // Scale factor

        private Vector2 windowSize = new Vector2(1600, 800);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            AllocConsole();
            Texture2D playerTexture = Content.Load<Texture2D>("blobby");
            player = new Player(playerTexture, new Vector2(50, 50), 140, 80, 2, 0.4f);
            TileList.bgColor = Color.Coral;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _playerSpriteSheet = Content.Load<Texture2D>("blobby");
            _tileSpriteSheet = Content.Load<Texture2D>("Tilemap");

            LoadLevel("Level0");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            player.Update(gameTime, windowSize);
            foreach (Tile tile in TileList.tiles)
            {
                tile.Update();
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(TileList.bgColor);

            _spriteBatch.Begin(transformMatrix: Matrix.CreateScale(scale)); // Apply scaling

            player.Draw(_spriteBatch);

            // Draw all tiles
            foreach (var tile in TileList.tiles)
            {
                tile.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void LoadLevel(string name)
        {
            // Load level from the Levels subdirectory
            CurrentLevel = Content.Load<Texture2D>($"Levels/{name}");
            level = ConvertTextureTo2DArray(CurrentLevel);

            // Create tiles based on the level data
            for (int i = 0; i < level.GetLength(0); i++)
            {
                for (int j = 0; j < level.GetLength(1); j++)
                {
                    // Check for the specific color to create a tile
                    if (level[i, j].R == 0 && level[i, j].G == 0 && level[i, j].B == 255)
                    {
                        new Tile(_tileSpriteSheet, new Vector2(i * 80, j * 80), 80, 80, new int[] { i, j }, 0);
                    }
                    else if (level[i, j].R == 255 && level[i, j].G == 128 && level[i, j].B == 0)
                    {
                        new Tile(_tileSpriteSheet, new Vector2(i * 80, j * 80), 80, 80, new int[] { i, j }, 1);
                    }
                }
            }

            for (int i = 0; i < TileList.tiles.Count; i++)
            {
                TileList.tiles[i].CalculateConnections(level);
            }

        }

        public Color[,] ConvertTextureTo2DArray(Texture2D texture)
        {
            // Ensure the texture is not null
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            // Get the texture width and height
            int width = texture.Width;
            int height = texture.Height;

            // Create a 2D array to hold the colors
            Color[,] colorArray = new Color[width, height];

            // Get the pixel data from the texture
            Color[] pixelData = new Color[width * height];
            texture.GetData(pixelData);

            // Fill the 2D array with color data
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorArray[x, y] = pixelData[x + y * width];
                }
            }

            return colorArray;
        }

        public Vector2 WindowSize
        {
            get { return windowSize; }
        }
    }   
        
}
