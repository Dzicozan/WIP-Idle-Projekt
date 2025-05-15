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
        public float cost => (float)(basecost * Math.Pow(1.05, PurchasedCount));
        public float atk;
        public float def;
        int PurchasedCount;

        public Button(string name, float basecost, float atk, float def, Rectangle rectangle, List<String> lines, Texture2D texture)
        {
            this.texture = texture;
            this.rectangle = rectangle;
            this.lines = lines;
            this.name=name;
            this.basecost=basecost;
            this.atk=atk;
            this.def=def;
            PurchasedCount =0;
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


            string displayAtk = atk != 0 ? $"{atk} Attack Power ":"";
            string displayDef = def != 0 ? $"{def} Defense Power ":"";

            // Draw the building name, cost, and CPS
            spriteBatch.DrawString(
                font,
                $"{name} - {formattedCost} Gold ({displayAtk}{displayDef})",
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
