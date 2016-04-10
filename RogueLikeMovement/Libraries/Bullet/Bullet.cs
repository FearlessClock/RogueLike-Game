using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    class Bullet : Moving_entity
    {
        //ID representing the bullet
        long ID;

        //Created time
        long time;

        //Damage that the bullet inflics
        int damage;
        /// <summary>
        /// Get the damage that the bullet will inflic
        /// </summary>
        public int GetDamage
        {
            get { return damage; }
        }

        /// <summary>
        /// Minimal distance for a hit
        /// </summary>
        public float GetHitRadius
        {
            get { return 60; }
        }

        //Lifetime of the bullet
        long lifeTime = 1000;
        
        /// <summary>
        /// Find out if the bullet is alive
        /// </summary>
        public bool IsAlive
        {
            get { return isAlive; }
        }

        /// <summary>
        /// Bullet class that represents every bullet currently in play
        /// </summary>
        /// <param name="pos">Position of the bullet when it is spawned</param>
        /// <param name="dir">Direction the bullet will travel in</param>
        /// <param name="id">The identification of the bullet</param>
        /// <param name="dam">The damage that the bullet will inflic</param>
        public Bullet(Vector2 pos, Vector2 dir, long id, int dam, int speed)
        {
            position = pos;
            direction = dir*speed;
            ID = id;
            damage = dam;
            time = Time.currentTime;
            isAlive = true;
        }

        /// <summary>
        /// Update the bullet location and check for collisions
        /// </summary>
        public void Update()
        {
            bool check = CollisionCheckV2();
            if (check)
            {
                isAlive = false;
                BulletController.AddExplosion(position);
            }
            position += direction;

            FindTexture(direction);
        }

        /// <summary>
        /// Check if the bullet has been in play for long enough
        /// </summary>
        /// <returns>Yes or no if the bullet has reached its limit</returns>
        public bool IsOutOfTime()
        {
            if (Time.currentTime - time > lifeTime)
                return true;
            return false;
        }

        public int GetTextureIndex
        {
            get { return this.selectedTexture; }
        }
    }
}
