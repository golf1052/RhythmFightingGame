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
        public int enemiesKilled;

        TimeSpan spawnTimer;
        bool justSpawned;

        public EnemyHandler(GraphicsDeviceManager graphics, SpriteSheetInfo spriteSheetInfo, List<Texture2D> spriteSheets, GameTimeWrapper gameTime)
        {
            enemies = new List<Enemy>();
            for (int i = 0; i < 10; i++)
            {
                Enemy newEnemy = new Enemy(graphics, spriteSheetInfo, gameTime);
                newEnemy.animations["hover"] = new SpriteSheet(spriteSheets[0], spriteSheetInfo, 8, 8, 1, SpriteSheet.Direction.LeftToRight, 500, true);
                newEnemy.Ready(graphics);
                enemies.Add(newEnemy);
            }
            enemyPositions = new HashSet<float>();
        }

        public void SetUpEnemies()
        {
            enemiesAlive = 0;
            spawnTimer = TimeSpan.Zero;
            justSpawned = false;
            enemiesKilled = 0;
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
                    enemy.pos.X = player.endDashPos.X + (500 * World.random.Next(-4, 5)) + enemy.tex.Width / 2 + 138;
                    while (enemyPositions.Contains(enemy.pos.X))
                    {
                        enemy.pos.X = player.endDashPos.X + (500 * World.random.Next(-4, 5)) + enemy.tex.Width / 2 + 138;
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
                if (spawnTimer >= TimeSpan.FromSeconds(3))
                {
                    justSpawned = false;
                    spawnTimer = TimeSpan.Zero;
                }
            }
            if (enemiesAlive < Math.Min(5, 10 - enemiesKilled))
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
                    if (enemy.hoverUp)
                    {
                        if (enemy.hoverDistance == enemy.hoverMax)
                        {
                            enemy.hoverUp = false;
                        }
                        else
                        {
                            enemy.pos.Y -= enemy.hoverStep;
                            enemy.hoverDistance += enemy.hoverStep;
                        }
                    }
                    else
                    {
                        if (enemy.hoverDistance == 0)
                        {
                            enemy.hoverUp = true;
                        }
                        else
                        {
                            enemy.pos.Y += enemy.hoverStep;
                            enemy.hoverDistance -= enemy.hoverStep;
                        }
                    }
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
