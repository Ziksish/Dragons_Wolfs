#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shooter;
#endregion

namespace DrawComponents
{
    class FrameRateCounter : DrawableGameComponent
    {
        int FrameRate = 0;
        int FrameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Game game)
            : base(game) { }

        public override void Update(GameTime gameTime) // (OVERRIDE vs NEW)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                FrameRate = FrameCounter;
                FrameCounter = 0;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont, int Health, int ShieldPower, Player player, Vector2 Campos)
        {
            FrameCounter++;
            string fps = string.Format("FPS: {0}", FrameRate);
            string hp = string.Format("Health: {0}", Health);
            string power = string.Format("Shield: {0}", ShieldPower);
            Color col = Color.Black;

            if (Health >= 70)
                col = Color.ForestGreen;
            else if (Health >= 30)
                col = Color.Yellow;
            else if (Health > 0)
                col = Color.Red;

            spriteBatch.DrawString(spriteFont, fps, new Vector2(10, 30) + Campos, Color.White);
            spriteBatch.DrawString(spriteFont, hp, new Vector2(20, 450) + Campos, col);
            spriteBatch.DrawString(spriteFont, "|", new Vector2(145, 450) + Campos, Color.Tomato);
            spriteBatch.DrawString(spriteFont, power, new Vector2(150, 450) + Campos, Color.Blue);
        }
    }
}
