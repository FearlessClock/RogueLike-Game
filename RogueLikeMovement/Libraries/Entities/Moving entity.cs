using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RogueLikeMovement
{
    class Moving_entity : Entity
    {
        public float movementSpeed = 1F;
        public MessageStack commandList = new MessageStack(false);

        public Random rand;
        public void Kill()
        {
            isAlive = false;
        }
        public void FindTexture(Vector2 dir)
        {
            if (!isAlive)
                selectedTexture = 8; //Player is dead
            else if (dir.X > 0 && dir.Y > 0)
                selectedTexture = 2;    //Down right
            else if (dir.X == 0 && dir.Y > 0)
                selectedTexture = 0;    //Down
            else if (dir.X < 0 && dir.Y > 0)
                selectedTexture = 1;    //down Left
            else if (dir.X > 0 && dir.Y == 0)
                selectedTexture = 4;    //Right
            else if (dir.X > 0 && dir.Y < 0)
                selectedTexture = 7;    //Up right
            else if (dir.X < 0 && dir.Y == 0)
                selectedTexture = 3;    //Left
            else if (dir.X == 0 && dir.Y < 0)
                selectedTexture = 5;    //Up
            else if (dir.X < 0 && dir.Y < 0)
                selectedTexture = 6;    //Up left

        }

        public void LauchAttack(Vector2 pos, Vector2 direction, int damage)
        {
            BulletController.AddBullet(pos, direction, damage);
        }

        public bool CollisionCheckV2()
        {
            Vector2 pos;
            pos.X = position.X + direction.X;
            pos.Y = position.Y;
            bool check = false;
            if (IsInsideBounds())
            {
                //Console.WriteLine(this.id + " : " + pos.X + " : " + pos.Y + " ::: " + direction.X + " : " + direction.Y);
                if (!Map.GetMap[(int)((pos.X) / 10), (int)((pos.Y) / 10)].walkable)
                {
                    check = true;
                    direction.X /= 2;
                }
                else if (!Map.GetMap[(int)((pos.X + size) / 10), (int)((pos.Y) / 10)].walkable)
                {
                    check = true;
                    direction.X /= 2;
                }
                else if (!Map.GetMap[(int)((pos.X + size) / 10), (int)((pos.Y + size) / 10)].walkable)
                {
                    check = true;
                    direction.X /= 2;
                }
                else if (!Map.GetMap[(int)((pos.X) / 10), (int)((pos.Y + size) / 10)].walkable)
                {
                    check = true;
                    direction.X /= 2;
                }

                pos.X = position.X;
                pos.Y = position.Y + direction.Y;
                if (!Map.GetMap[(int)((pos.X) / 10), (int)((pos.Y) / 10)].walkable)
                {
                    check = true;
                    direction.Y /= 2;
                }
                else if (!Map.GetMap[(int)((pos.X + size) / 10), (int)((pos.Y) / 10)].walkable)
                {
                    check = true;
                    direction.Y /= 2;
                }
                else if (!Map.GetMap[(int)((pos.X + size) / 10), (int)((pos.Y + size) / 10)].walkable)
                {
                    check = true;
                    direction.Y /= 2;
                }
                else if (!Map.GetMap[(int)((pos.X) / 10), (int)((pos.Y + size) / 10)].walkable)
                {
                    check = true;
                    direction.Y /= 2;
                }
            }
            //if(!check)
            //bool entCheck = EntityController.CheckEntityCollision(this);
            return check;
        }
        private bool IsInsideBounds()
        {
            Vector2 pos = position / 10;
            if (pos.X < 0 || pos.X > Map.GetMap.GetLength(0) - 1)
                return false;
            if (pos.Y < 0 || pos.Y > Map.GetMap.GetLength(1) - 1)
                return false;
            pos.X = (position.X + size) / 10;
            if (pos.X < 0 || pos.X > Map.GetMap.GetLength(0) - 1)
                return false;
            if (pos.Y < 0 || pos.Y > Map.GetMap.GetLength(1) - 1)
                return false;
            pos.Y = (position.Y + size) / 10;
            if (pos.X < 0 || pos.X > Map.GetMap.GetLength(0) - 1)
                return false;
            if (pos.Y < 0 || pos.Y > Map.GetMap.GetLength(1) - 1)
                return false;
            pos.X = (position.X - size) / 10;
            return true;
        }

        public float DistanceBetween(Vector2 a)
        {
            return (position.X - a.X) * (position.X - a.X) + (position.Y - a.Y) * (position.Y - a.Y);
        }
    }
}
