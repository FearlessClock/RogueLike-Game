using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    class EntityController
    {
        static List<Moving_entity> enemies = new List<Moving_entity>();
        static public Player player;
        static public Random rand = new Random();

        static public Moving_entity[] GetEnemyArray()
        {
            return enemies.ToArray<Moving_entity>();
        }
        static public void SetupEntityController(ref Player playerInfo)
        {
            player = playerInfo;
            enemies = new List<Moving_entity>();
        }
        static public void AddEntity(Vector2 pos, Vector2 dir, int id)
        {
            enemies.Add(new Frog(pos, dir, id, 100, rand, 8000, rand.Next(400, 800), 1100));
        }

        static public void Update()
        {
            //Player update stuff
            CheckBulletHit((Moving_entity)player);
            //Mobs update stuff
            for(int i  = 0; i < enemies.Count; i++)
            {
                Frog frog = (Frog)enemies[i];
                CheckBulletHit(enemies[i]);
                frog.Update(player);
                //Remove dead
                if (!frog.isAlive)
                {
                    enemies.RemoveAt(i);
                    i--;
                }
            }
        }

        static private void CheckBulletHit(Moving_entity enemy)
        {
            BulletController.CheckCollisionWith(enemy);
        }

        static public bool CheckEntityCollision(Moving_entity ent)
        {
            for(int i = 0; i < enemies.Count; i++)
            {
                if(ent.id != enemies[i].id && ent.DistanceBetween(enemies[i].position) < ent.size)
                {
                    return true;
                }
            }
            return false;
        }

        static public GraphicsBuffer Draw(GraphicsBuffer buf)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                buf = enemies[i].Draw(buf);
            }
            return buf;
        }
        
    }
}
