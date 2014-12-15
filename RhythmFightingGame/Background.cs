using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace RhythmFightingGame
{
    public class Background
    {
        Sprite left;
        Sprite right;

        public Background(Sprite left, Sprite right)
        {
            this.left = left;
            this.right = right;
            this.left.origin = Vector2.Zero;
            this.right.origin = Vector2.Zero;
            this.right.pos.X = this.left.tex.Width;
        }

        public void Update(Camera camera)
        {
            if (camera.viewport.Bounds.Left < left.pos.X &&
                camera.viewport.Bounds.Right > left.pos.X)
            {
                right.pos.X = left.pos.X - right.tex.Width;
            }
            else if (camera.viewport.Bounds.Left < right.pos.X &&
                camera.viewport.Bounds.Right > right.pos.X)
            {
                left.pos.X = right.pos.X - right.tex.Width;
            }
            else if (camera.viewport.Bounds.Right > left.pos.X + left.tex.Width &&
                camera.viewport.Bounds.Left < left.pos.X + left.tex.Width)
            {
                right.pos.X = left.pos.X + left.tex.Width;
            }
            else if (camera.viewport.Bounds.Right > right.pos.X + right.tex.Width &&
                camera.viewport.Bounds.Left < right.pos.X + right.tex.Width)
            {
                left.pos.X = right.pos.X + right.tex.Width;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            left.Draw(spriteBatch);
            right.Draw(spriteBatch);
        }
    }
}
