using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace idle
{
    public class Button
    {
        public List<String> lines;
        public Vector2 textPos1;
        public Vector2 textPos2;
        public Rectangle rectangle;
        private Texture2D texture;

        String name;
        public float basecost;
        public float cost => (float)(basecost * System.Math.Pow(1.05, PurchasedCount));
        public float rate;
        int PurchasedCount;

        public Button(string name, float basecost, float rate,Rectangle rectangle, List<String> lines, Texture2D texture)
        {
            this.texture = texture;
            this.rectangle = rectangle;
            this.lines = lines;
            this.name=name;
            this.basecost=basecost;
            this.rate=rate;
            PurchasedCount=0;
        }

        public void Purchase()
        {
            PurchasedCount++;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // Draw the background
            spriteBatch.Draw(texture, rectangle, Color.LightGray);

            string formattedCost = cost > 999 
                ? cost.ToString("E2") 
                : cost.ToString("F2");

            // Draw the building name, cost, and CPS
            spriteBatch.DrawString(
                font,
                $"{name} - {formattedCost} Dollars ({rate} D/s)",
                new Vector2(rectangle.X + 5, rectangle.Y + 5),
                Color.Black
            );
            // Draw the purchased count
            spriteBatch.DrawString(
                font,
                $"Purchased: {PurchasedCount}",
                new Vector2(rectangle.X + 5, rectangle.Y + 25),
                Color.Black
            );
        }
    }
}
