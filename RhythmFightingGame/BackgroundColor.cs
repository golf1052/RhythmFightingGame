using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace RhythmFightingGame
{
    public class BackgroundColor : Sprite
    {
        private Dictionary<string, Color> colors;
        private string _currentColorName;
        public string currentColorName
        {
            get
            {
                return _currentColorName;
            }
            set
            {
                if (colors.ContainsKey(value))
                {
                    _currentColorName = value;
                    color = currentColor;
                }
                else
                {
                    throw new Exception("That color doesn't exist");
                }
            }
        }

        public Color currentColor
        {
            get
            {
                return colors[_currentColorName];
            }
            set
            {
                colors[_currentColorName] = value;
                color = value;
            }
        }

        public BackgroundColor(GraphicsDeviceManager graphics) : base(graphics)
        {
            colors = new Dictionary<string, Color>();
            _currentColorName = "";
        }

        public bool AddColor(string name, Color color)
        {
            if (!colors.ContainsKey(name))
            {
                colors.Add(name, color);
                if (colors.Count == 1)
                {
                    currentColorName = name;
                }
                return true;
            }
            return false;
        }

        public bool RemoveColor(string name, Color color)
        {
            if (colors.ContainsKey(name))
            {
                colors.Remove(name);
                if (colors.Count == 0)
                {
                    _currentColorName = "";
                }
                return true;
            }
            return false;
        }
    }
}
