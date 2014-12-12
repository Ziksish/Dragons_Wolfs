#region Using Statements
using DrawComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Shooter;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dragons_Wolfs
{
    #region Struct of Megatexture
    public struct MegaTexture
    {
        public Texture2D Text;
        public Vector2 Origin;
        public int Width, Height;

        public MegaTexture(Texture2D text, Vector2 or)
        {
            Text = text;
            Origin = or;
            Width = text.Width;
            Height = text.Height;
        }
    }
    #endregion

    #region Class Game
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DragonsWolfs : Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        Viewport Camviewport;

        public Texture2D textureline, background;
        public SpriteFont font;
        Player player;
        FrameRateCounter fpsdraw;
        Matrix Campos;
        public Boolean showinvent;

        public Vector2 Camcenter;

        public MouseState previousmousestate, currentmousestate;
        public KeyboardState previouskeyboardstate, currentkeyboardstate;

        public MegaTexture playertext, shieldtext;
        MegaTexture[] enemyestext;
        public List<Enemy> Enemyes = new List<Enemy>();

        public DragonsWolfs()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
            IsMouseVisible = true;
        }

        #region Drawable Functions
        public void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color col)
        {
            Vector2 Edge = end - start;
            float angle = (float)Math.Atan2(Edge.Y, Edge.X);

            sb.Draw(textureline, new Rectangle((int)start.X, (int)start.Y, (int)Edge.Length(), 1), null, col, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public void DrawOutRectangle(SpriteBatch sb, Vector2 start, Vector2 end, Color col)
        {
            DrawLine(sb, start, new Vector2(end.X, start.Y), col);
            DrawLine(sb, new Vector2(start.X + 1, start.Y), new Vector2(start.X + 1, end.Y), col);
            DrawLine(sb, new Vector2(start.X, end.Y - 1), new Vector2(end.X, end.Y - 1), col);
            DrawLine(sb, new Vector2(end.X, start.Y), end, col);
        }


        #endregion

        #region Initialize and LoadContent

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            player = new Player();
            fpsdraw = new FrameRateCounter(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Camviewport = GraphicsDevice.Viewport;

            textureline = new Texture2D(GraphicsDevice, 1, 1);
            background = Content.Load<Texture2D>("image 107");
            textureline.SetData<Color>(new Color[] { Color.White });
            font = Content.Load<SpriteFont>("Font0");

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 50, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            //*
            shieldtext = new MegaTexture(Content.Load<Texture2D>("Shield"), new Vector2(16, 25));
            playertext = new MegaTexture(Content.Load<Texture2D>("player"), new Vector2(25, 25));
            enemyestext = new MegaTexture[2] { new MegaTexture(Content.Load<Texture2D>("player"), new Vector2(25, 25)), new MegaTexture(Content.Load<Texture2D>("image 380"), new Vector2(52, 38)) };
            //*

            player.Init(playertext, playerPosition, shieldtext);

            //Enemyes = new Enemy[2] { new Enemy(enemyestext[1], new Vector2(100, 100)), new Enemy(enemyestext[0], new Vector2(200, 130)) };
            Enemyes.Add(new Enemy(enemyestext[1], new Vector2(100, 100)));
            Enemyes.Add(new Enemy(enemyestext[0], new Vector2(200, 130)));

            // ShowInventory
            showinvent = false;

            currentmousestate = Mouse.GetState();
            previousmousestate = currentmousestate;
            currentkeyboardstate = Keyboard.GetState();
            previouskeyboardstate = currentkeyboardstate;

            // TODO: use this.Content to load your game content here

            base.LoadContent();
        }

        #endregion

        #region UploadContent

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || (Keyboard.GetState().IsKeyDown(Keys.Escape)))
                Exit();

            player.Update(this);

            previousmousestate = currentmousestate;
            currentmousestate = Mouse.GetState();
            previouskeyboardstate = currentkeyboardstate;
            currentkeyboardstate = Keyboard.GetState();

            fpsdraw.Update(gameTime);

            Camcenter = new Vector2(player.Position.X - 320, player.Position.Y - 240);
            Campos = Matrix.CreateScale(new Vector3(1, 1, 0)) * Matrix.CreateTranslation(new Vector3(-Camcenter.X, -Camcenter.Y, 0));

            foreach (Enemy enemyes in Enemyes)
            {
                if (enemyes != null)
                {
                    enemyes.Update(enemyes);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                if (Enemyes[0] != null)
                    Enemyes[0].Health -= 1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                foreach (Enemy enemyes in Enemyes)
                {
                    if (enemyes != null)
                        enemyes.Health += 1;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                player.Health -= 1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                player.Health += 1;
            }

            for (int i = 0; i < Enemyes.Count; i++)
            {
                if (Enemyes[i] != null)
                    if (Enemyes[i].Health <= 0) Enemyes[i] = null;
            }

            if ((previouskeyboardstate.IsKeyDown(Keys.I)) && (currentkeyboardstate.IsKeyUp(Keys.I)))
            {
                showinvent = !showinvent;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Color col = Color.Black;
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            // Start Drawing
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Campos);

            for (int i = 0; i <= (int)(640 / background.Width); i++)
            {
                for (int p = 0; p <= (int)(480 / background.Height); p++)
                {
                    spriteBatch.Draw(background, new Rectangle(i * background.Width, p * background.Height, background.Width, background.Height), Color.White);
                }
            }

            for (int i = 0; i <= (int)(640 / 32); i++)
                DrawLine(spriteBatch, new Vector2(i * 32, 0), new Vector2(i * 32, 480), Color.Yellow * 0.5f);
            for (int i = 0; i <= (int)(480 / 32); i++)
                DrawLine(spriteBatch, new Vector2(0, i * 32), new Vector2(640, i * 32), Color.Yellow * 0.5f);

            spriteBatch.Draw(textureline, new Rectangle((int)(player.Position.X - 16), (int)(player.Position.Y - 16), 32, 32), Color.Green * 0.7f);

            // Enemyes Sprites
            foreach (Enemy enemyes in Enemyes)
            {
                if (enemyes != null)
                {
                    spriteBatch.Draw(textureline, new Rectangle((int)(enemyes.Position.X - enemyes.EnemyTexture.Text.Width / 2), (int)(enemyes.Position.Y - enemyes.EnemyTexture.Text.Height / 2), enemyes.EnemyTexture.Text.Width, enemyes.EnemyTexture.Text.Height), Color.Red * 0.7f);
                    enemyes.Draw(spriteBatch);

                    col = Color.Black;

                    if (enemyes.Health >= 70)
                        col = Color.ForestGreen;
                    else if (enemyes.Health >= 30)
                        col = Color.Yellow;
                    else if (enemyes.Health > 0)
                        col = Color.Red;


                    spriteBatch.Draw(textureline, new Rectangle((int)(enemyes.Position.X - enemyes.EnemyTexture.Text.Width / 2f), (int)(enemyes.Position.Y - enemyes.EnemyTexture.Text.Height / 2f - enemyes.EnemyTexture.Text.Height / 10f), (int)(enemyes.EnemyTexture.Text.Width - enemyes.EnemyTexture.Text.Width / 100f * (100 - enemyes.Health)), (int)(enemyes.EnemyTexture.Text.Height / 10f)), col);
                    spriteBatch.Draw(textureline, new Rectangle((int)(enemyes.Position.X - enemyes.EnemyTexture.Text.Width / 2f), (int)(enemyes.Position.Y - enemyes.EnemyTexture.Text.Height / 2f), (int)(enemyes.EnemyTexture.Text.Width), (int)(enemyes.EnemyTexture.Text.Height / 10f)), Color.DarkOrange);
                    DrawOutRectangle(spriteBatch, new Vector2(enemyes.Position.X - enemyes.EnemyTexture.Text.Width / 2, enemyes.Position.Y - enemyes.EnemyTexture.Text.Height / 2f - enemyes.EnemyTexture.Text.Height / 10f), new Vector2(enemyes.Position.X + enemyes.EnemyTexture.Text.Width / 2, enemyes.Position.Y - (enemyes.EnemyTexture.Text.Height / 2) + (enemyes.EnemyTexture.Text.Height / 10)), Color.Black);
                }
            }

            player.Draw(spriteBatch, this);

            // HUD FOR ENEMYES
            foreach (Enemy enemyes in Enemyes)
            {
                if (enemyes != null)
                {
                    enemyes.DrawHUD(this);
                }
            }


            /* DRAW UP-HUD TEXT HP HERE */
            foreach (Enemy enemyes in Enemyes)
            {
                if (enemyes != null)
                {
                    float colx1 = enemyes.Position.X - enemyes.EnemyTexture.Origin.X;
                    float coly1 = enemyes.Position.Y - enemyes.EnemyTexture.Origin.Y;
                    float colx2 = colx1 + enemyes.EnemyTexture.Width;
                    float coly2 = coly1 + enemyes.EnemyTexture.Height;

                    if ((currentmousestate.Position.X + Camcenter.X >= colx1) && (currentmousestate.Position.Y + Camcenter.Y >= coly1) && (currentmousestate.Position.X + Camcenter.X < colx2) && (currentmousestate.Position.Y + Camcenter.Y < coly2))
                    {
                        spriteBatch.DrawString(font, enemyes + ".HP: " + enemyes.Health, new Vector2(10 + Camcenter.X, 120 + Camcenter.Y), Color.White);
                    }
                }
            }

            if (player.Health >= 70)
                col = Color.ForestGreen;
            else if (player.Health >= 30)
                col = Color.Yellow;
            else if (player.Health > 0)
                col = Color.Red;

            /*DRAW UP-HUD HERE */
            float energy_x = (float)(player.Position.X - player.PlayerTexture.Text.Width / 2);
            float energy_y = (float)(player.Position.Y - player.PlayerTexture.Text.Height / 2f);
            float energy_width = (float)(player.PlayerTexture.Text.Width - player.PlayerTexture.Text.Width / 100f * (100 - player.ShieldPower));
            float energy_height = (float)(player.PlayerTexture.Text.Height / 10f);
            float hp_y = (float)(player.Position.Y - (player.PlayerTexture.Text.Height / 2f + player.PlayerTexture.Text.Height / 10f));
            float hp_width = (float)(player.PlayerTexture.Text.Width - player.PlayerTexture.Text.Width / 100f * (100 - (player.Health)));
            float width = (float)(player.PlayerTexture.Text.Width);


            spriteBatch.Draw(textureline, new Rectangle((int)(energy_x), (int)(hp_y), (int)(hp_width), (int)(energy_height)), col);
            spriteBatch.Draw(textureline, new Rectangle((int)(energy_x), (int)(energy_y), (int)(energy_width), (int)(energy_height)), Color.Blue);
            DrawOutRectangle(spriteBatch, new Vector2(energy_x, hp_y), new Vector2(energy_x + width, hp_y + energy_height * 2), Color.Black);

            // HUD
            spriteBatch.DrawString(font, "Mouse X: " + (Mouse.GetState().Position.X + Camcenter.X) + " Mouse Y: " + (Mouse.GetState().Position.Y + Camcenter.Y) + " Count of Array: " + Enemyes[0], new Vector2(10 + Camcenter.X, 10 + Camcenter.Y), Color.White);
            //spriteBatch.DrawString(font, "Image: " + player.PlayerTexture, new Vector2(150, 150), Color.Red);
            spriteBatch.Draw(textureline, new Rectangle((int)(15 + Camcenter.X), (int)(450 + Camcenter.Y), 130, 25), Color.Black * 1f);
            if (player != null)
                fpsdraw.Draw(gameTime, spriteBatch, font, player.Health, (int)player.ShieldPower, player, Camcenter);
            else
                fpsdraw.Draw(gameTime, spriteBatch, font, player.Health, (int)player.ShieldPower, null, Camcenter);

            // Stop Drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
    #endregion

    #region Class Programm
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public const bool TypeOfControl = false;

        [STAThread]
        static void Main()
        {
            using (var game = new DragonsWolfs())
                game.Run();
        }
    }
#endif
    #endregion
}