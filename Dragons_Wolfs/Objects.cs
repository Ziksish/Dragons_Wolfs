#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Dragons_Wolfs;
#endregion

namespace Shooter
{
    #region Class of Player
    public class Player
    {
        public MegaTexture PlayerTexture, ShieldTexture;
        public Vector2 Position;
        public double Direction, ShieldPower;
        public int Health;
        public bool CanShield;
        public int[] Inventory;

        //public string LocatedInMap;

        public Player()
        {
        }

        public void Init(MegaTexture texture, Vector2 pos, MegaTexture shieldtext)
        {
            PlayerTexture = texture;
            Direction = Math.PI;
            Position = pos;
            Health = 100;
            ShieldPower = 100;
            ShieldTexture = shieldtext;
            CanShield = true;
            Inventory = new int[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        #region Update and Draw

        public void Update(DragonsWolfs game)
        {
            MouseState previousmousestate = game.previousmousestate, currentmousestate = game.currentmousestate;
            KeyboardState previouskeyboardstate = game.previouskeyboardstate, currentkeyboardstate = game.currentkeyboardstate;

            if (Program.TypeOfControl == false)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    Position.Y -= 2;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    Position.X -= 2;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    Position.Y += 2;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    Position.X += 2;
            }
            else
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.W)) && (Keyboard.GetState().IsKeyDown(Keys.A)))
                    MoveToPoint(Direction + Math.PI - Math.PI / 4);
                else if ((Keyboard.GetState().IsKeyDown(Keys.W)) && (Keyboard.GetState().IsKeyDown(Keys.D)))
                    MoveToPoint(Direction - Math.PI + Math.PI / 4);
                else if ((Keyboard.GetState().IsKeyDown(Keys.S)) && (Keyboard.GetState().IsKeyDown(Keys.A)))
                    MoveToPoint(Direction + Math.PI / 4);
                else if ((Keyboard.GetState().IsKeyDown(Keys.S)) && (Keyboard.GetState().IsKeyDown(Keys.D)))
                    MoveToPoint(Direction - Math.PI / 4);
                else if (Keyboard.GetState().IsKeyDown(Keys.A))
                    MoveToPoint(Direction + Math.PI / 2);
                else if (Keyboard.GetState().IsKeyDown(Keys.D))
                    MoveToPoint(Direction - Math.PI / 2);
                else if (Keyboard.GetState().IsKeyDown(Keys.W))
                    MoveToPoint(Direction - Math.PI);
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                    MoveToPoint(Direction);
            }

            // Magical shield
            if ((Keyboard.GetState().IsKeyDown(Keys.Space)) && (CanShield))
            {
                if (ShieldPower > 0)
                    ShieldPower -= 0.3;
                else if (ShieldPower <= 0)
                    CanShield = false;
            }
            else if ((previouskeyboardstate.IsKeyDown(Keys.Space)) && (currentkeyboardstate.IsKeyUp(Keys.Space)) && (ShieldPower < 30))
                CanShield = false;
            else
            {
                if (ShieldPower < 100)
                {
                    ShieldPower += 0.1;
                    if ((ShieldPower >= 30) && (!CanShield))
                        CanShield = true;
                }
            }

            // SPELL #1
            if ((previouskeyboardstate.IsKeyDown(Keys.D1)) && (currentkeyboardstate.IsKeyUp(Keys.D1)))
            {
                ;
            }

            Direction = Point_Direction(currentmousestate.X + game.Camcenter.X, currentmousestate.Y + game.Camcenter.Y);
        }

        public void Draw(SpriteBatch spriteBatch, DragonsWolfs game)
        {
            spriteBatch.Draw(PlayerTexture.Text, Position, null, Color.White, (float)(Direction + Math.PI / 2), PlayerTexture.Origin, 1f, SpriteEffects.None, 0f);
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && (CanShield))
                spriteBatch.Draw(game.shieldtext.Text, Position, null, Color.White, (float)(Direction - Math.PI / 2), ShieldTexture.Origin, 1f, SpriteEffects.None, 0f);
        }

        #endregion

        public double Point_Direction(double X1, double Y1)
        {
            return Math.Atan2((Position.Y - Y1), (Position.X - X1));
        }

        public void MoveToPoint(double Dir)
        {
            Position = new Vector2(Position.X + (float)Math.Cos(Dir) * 2, Position.Y + (float)Math.Sin(Dir) * 2);
        }
    }
    #endregion

    public class Enemy
    {
        public MegaTexture EnemyTexture;
        public Vector2 Position;
        public double Direction, AngryPoints;
        public int Health;
        //public bool TypeofEnemy;

        public Enemy(MegaTexture texture, Vector2 pos)
        {
            EnemyTexture = texture;
            Position = pos;
            Direction = Math.PI;
            Health = 100;
            AngryPoints = 0f;
        }

        public void Update(Enemy enemy)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //DrawOutRectangle(spriteBatch, new Vector2(150, 150), new Vector2(160, 160), Color.Red);
            spriteBatch.Draw(EnemyTexture.Text, Position, null, Color.White, (float)(Direction + Math.PI / 2), EnemyTexture.Origin, 1f, SpriteEffects.None, 0f);
        }

        public void DrawHUD(DragonsWolfs game)
        {
            Color col = Color.Black;
            if (Health >= 70)
                col = Color.ForestGreen;
            else if (Health >= 30)
                col = Color.Yellow;
            else if (Health > 0)
                col = Color.Red;

            game.spriteBatch.Draw(game.textureline, new Rectangle((int)(Position.X - EnemyTexture.Text.Width / 2f), (int)(Position.Y - EnemyTexture.Text.Height / 2f - EnemyTexture.Text.Height / 10f), (int)(EnemyTexture.Text.Width - EnemyTexture.Text.Width / 100f * (100 - Health)), (int)(EnemyTexture.Text.Height / 10f)), col);
            game.spriteBatch.Draw(game.textureline, new Rectangle((int)(Position.X - EnemyTexture.Text.Width / 2f), (int)(Position.Y - EnemyTexture.Text.Height / 2f), (int)(EnemyTexture.Text.Width), (int)(EnemyTexture.Text.Height / 10f)), Color.DarkOrange);
            game.DrawOutRectangle(game.spriteBatch, new Vector2(Position.X - EnemyTexture.Text.Width / 2, Position.Y - EnemyTexture.Text.Height / 2f - EnemyTexture.Text.Height / 10f), new Vector2(Position.X + EnemyTexture.Text.Width / 2, Position.Y - (EnemyTexture.Text.Height / 2) + (EnemyTexture.Text.Height / 10)), Color.Black);
        }
    }
}
