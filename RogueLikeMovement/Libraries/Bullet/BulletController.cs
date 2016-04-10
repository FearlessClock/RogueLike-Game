using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    class BulletController
    {
        static List<Bullet> bullets = new List<Bullet>();
        static long bulletCount = 0;
        static string[] bulletTex = new string[8]
        {
            "Bullet\\BulletDown.png", "Bullet\\BulletDownLeft.png",
            "Bullet\\BulletDownRight.png", "Bullet\\BulletLeft.png",
            "Bullet\\BulletRight.png", "Bullet\\BulletUp.png",
            "Bullet\\BulletUpLeft.png", "Bullet\\BulletUpRight.png"
        };

        static Texture2D[] texture;
        static public Texture2D GetTexture(int texIndex)
        {
            if(texture != null)
                return texture[texIndex];
            return new Texture2D();
        }

        /// <summary>
        /// Load in all the textures needed for the entity
        /// </summary>
        /// <param name="textureFilePaths">File paths for all the textures used</param>
        static public void LoadTextures()
        {
            texture = new Texture2D[bulletTex.Length];
            for (int i = 0; i < bulletTex.Length; i++)
                texture[i] = ContentPipe.LoadTexture(bulletTex[i]);

        }

        //List of explosions to keep track of where the are and to remove them when they are done
        static List<Explosion> explosions = new List<Explosion>();

        static public void Update()
        {
            foreach (Bullet b in bullets)
            {
                b.Update();
            }
            foreach (Explosion e in explosions)
            {
                e.Update();
            }
            RemoveDeadBullet();
            RemoveDeadExplosion();
        }

        /// <summary>
        /// Add a bullet to the list of bullets currently in play 
        /// </summary>
        /// <param name="pos">Spawn position of the bullet</param>
        /// <param name="dir">Direction the bullet will travel in</param>
        /// <param name="damage">The damage the bullet will inflic</param>
        static public void AddBullet(Vector2 pos, Vector2 dir, int damage)
        {
            pos += dir * 8;
            bullets.Add(new Bullet(pos, dir, bulletCount, damage, 3));
            bulletCount++;
        }

        /// <summary>
        /// Add an explosion to a certain position in game
        /// </summary>
        /// <param name="pos">Position of the explosion</param>
        static public void AddExplosion(Vector2 pos)
        {
            explosions.Add(new Explosion(pos, 30));
        }

        /// <summary>
        /// Remove all the bullets that have been destroyed
        /// </summary>
        static public void RemoveDeadBullet()
        {
            for (int i = 0; i < bullets.Count; i++)
                if (!bullets[i].IsAlive || bullets[i].IsOutOfTime())
                {
                    bullets.RemoveAt(i);
                    i--;
                }
        }

        /// <summary>
        /// Remove all the explosions that have finished their explosion
        /// </summary>
        static public void RemoveDeadExplosion()
        {
            for (int i = 0; i < explosions.Count; i++)
            {
                if (!explosions[i].isAlive)
                {
                    explosions.RemoveAt(i);
                    i--;
                }
            }
        }

        static public void CheckCollisionWith(Moving_entity ent)
        {


            if (ent.GetType() == typeof(Frog))
            {
                Frog frog = (Frog)ent;
                for (int i = 0; i < bullets.Count; i++)
                {
                    if (bullets[i].isAlive)
                    {
                        if (DistanceBetween(frog.position, bullets[i].position) < bullets[i].GetHitRadius)
                        {
                            AddExplosion(frog.position);
                            frog.commandList.AddMessage(Command.Hit, 10);
                            bullets[i].Kill();
                        }
                    }
                }
            }
            else if (ent.GetType() == typeof(Player))
            {
                Player player = (Player)ent;
                for (int i = 0; i < bullets.Count; i++)
                {
                    if (bullets[i].isAlive)
                    {
                        if (DistanceBetween(player.position, bullets[i].position) < bullets[i].GetHitRadius)
                        {
                            AddExplosion(player.position);
                            player.commandList.AddMessage(Command.Hit, 10);
                            bullets[i].Kill();
                        }
                    }
                }
            }
        }

        static private float DistanceBetween(Vector2 a, Vector2 b)
        {
            return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        }

        static public GraphicsBuffer[] Draw(GraphicsBuffer[] buf)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                int texPos = bullets[i].GetTextureIndex;
                buf[texPos] = bullets[i].Draw(buf[texPos]);
            }
            return buf;
        }

        static public GraphicsBuffer DrawExplosion(GraphicsBuffer buf)
        {
            for (int i = 0; i < explosions.Count; i++)
                buf = explosions[i].Draw(buf);
            return buf;
        }

        static public string toString()
        {
            return bulletCount.ToString();
        }
    }
}
