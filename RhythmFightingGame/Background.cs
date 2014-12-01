using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace RhythmFightingGame
{
    public class Background : Sprite
    {
        public Background(Texture2D loadedTex) : base(loadedTex)
        {
            origin = Vector2.Zero;
        }
    }
}
