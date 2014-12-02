using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GLX;

namespace RhythmFightingGame
{
    public class EnemyHandler
    {
        public List<Enemy> enemies;
        public HashSet<float> enemyPositions;
        public int enemiesAlive;

        TimeSpan spawnTimer;
        bool justSpawned;

        public EnemyHandler(GraphicsDeviceManager graphics, SpriteSheetInfo spriteSheetInfo, List<Texture2D> spriteSheets, GameTimeWrapper gameTime)
        {
            enemies = new List<Enemy>();
            for (int i = 0; i < 10; i++)
            {
                Enemy newEnemy = new Enemy(graphics, spriteSheetInfo, gameTime);
                newEnemy.animations["hover"] = new SpriteSheet(spriteSheets[0], spriteSheetInfo, 8, 4, 2, SpriteSheet.Direction.LeftToRight, 500, true);
                newEnemy.Ready(graphics);
                enemies.Add(newEnemy);
            }
            enemyPositions = new HashSet<float>();
            enemiesAlive = 0;
            spawnTimer = TimeSpan.Zero;
            justSpawned = false;
        }

        public void SpawnEnemy(GraphicsDeviceManager graphics, Player player)
        {
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.visible)
                {
                    enemiesAlive++;
                    enemy.visible = true;
                    enemy.GenerateAttacks();
                    enemy.pos.X = player.endDashPos.X + (500 * World.random.Next(-2, 3)) + enemy.tex.Width / 2 + 25;
                    while (enemyPositions.Contains(enemy.pos.X) || enemy.pos.X < 0 || enemy.pos.X > 4000)
                    {
                        enemy.pos.X = player.endDashPos.X + (500 * World.random.Next(-2, 3)) + enemy.tex.Width / 2 + 25;
                    }
                    enemyPositions.Add(enemy.pos.X);
                    enemy.pos.Y = graphics.GraphicsDevice.Viewport.Height - enemy.tex.Height / 2;
                    justSpawned = true;
                    break;
                }
            }
        }

        public void Update(GameTimeWrapper gameTime, GraphicsDeviceManager graphics, Player player)
        {
            if (justSpawned)
            {
                spawnTimer += gameTime.ElapsedGameTime;
                if (spawnTimer >= TimeSpan.FromSeconds(10))
                {
                    justSpawned = false;
                    spawnTimer = TimeSpan.Zero;
                }
            }
            if (enemiesAlive < 3)
            {
                if (!justSpawned)
                {
                    SpawnEnemy(graphics, player);
                }
            }
            foreach (Enemy enemy in enemies)
            {
                if (enemy.visible)
                {
                    enemy.Update(gameTime, graphics);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.visible)
                {
                    enemy.Draw(spriteBatch);
                }
            }
        }
    }
}
