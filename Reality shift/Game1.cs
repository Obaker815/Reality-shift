using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Reality_shift
{
    public class Game1 : Game
    {
        private Texture2D CurrentLevel;
        public Color[,] level;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _playerSpriteSheet;
        private Player player;
        private float scale = 0.5f;  // Scale factor

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Texture2D playerTexture = Content.Load<Texture2D>("blobby");
            player = new Player(playerTexture, new Vector2(50, 50), 140, 80, 2, 0.4f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _playerSpriteSheet = Content.Load<Texture2D>("blobby");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: Matrix.CreateScale(scale)); // Apply scaling

            player.Draw(_spriteBatch);


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void LoadLevel(string name)
        {
            CurrentLevel = Content.Load<Texture2D>(name);
            level = ConvertTextureTo2DArray(CurrentLevel);

            for (int i = 0;  i < level.GetLength(0)-1; i++)
            {
                for (int j = 0; j < level.GetLength(1)-1; j++)
                {
                    
                }
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
    }
}
