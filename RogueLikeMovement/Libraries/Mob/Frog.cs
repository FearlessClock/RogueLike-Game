using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    class Frog : Moving_entity
    {
        int fireCooldown = 155;
        int cooldown = 0;

        int dirChangeTimer = 155;
        int dirCoolDown = 0;

        int maxViewDistance;
        int maxAttackDistance;

        int minReachDistance;
        int minAttackDistance;
        /// <summary>
        /// Class used to create the frog entities
        /// </summary>
        /// <param name="pos">Spawn position of the frog</param>
        /// <param name="dir">Starting direction of the frog</param>
        /// <param name="ID">The identification of the frog</param>
        /// <param name="heal">Health of the frog</param>
        /// <param name="random">Random object</param>
        /// <param name="maxViewDistance">The distance where the frog will react to the player</param>
        /// <param name="minReachDistance">Distance from which the frog will stop when chasing the player</param>
        /// <param name="maxAttackDistance">Distance before the frog will start attacking, the min is the same as the min reach</param>
        /// <param name="fireCooldown">Cooldown before the frog can fire</param>
        /// <param name="dirChangeTimer">Cooldown before the frog will change direction</param>
        public Frog(Vector2 pos, Vector2 dir, int ID, int heal, Random random,
                    int maxViewDistance, int minReachDistance, int maxAttackDistance,
                    int fireCooldown = 150, int dirChangeTimer = 200)
        {
            position = pos;
            direction = dir;
            id = ID;
            isAlive = true;
            health = heal;
            rand = random;

            this.maxViewDistance = maxViewDistance;
            this.maxAttackDistance = maxAttackDistance;
            this.minReachDistance = minReachDistance;
            this.minAttackDistance = minReachDistance - 2;

            this.fireCooldown = fireCooldown;
            this.dirChangeTimer = dirChangeTimer;
        }

        public override string ToString()
        {
            return id + ": " + position.ToString();
        }

        /// <summary>
        /// Update the frog to make it move, attack, die, etc
        /// </summary>
        /// <param name="player">The player</param>
        public void Update(Player player)
        {
            //If there are commands that needs to be read and used like being attacked
            if (commandList.Count > 0)
                TreatCommands();
            double dis = DistanceBetween(player.position, position);
            //If the player is alive or in view, calculate the direction
            if (player.isAlive || dis > maxViewDistance)
            {
                //If the player is in the view range
                if (dis < maxViewDistance && dis > minReachDistance)
                    //make a normalized vector pointing to the player
                    direction = new Vector2(player.position.X - position.X, player.position.Y - position.Y).Normalized();
                //If it's next to the player, don't move
                else if (dis < minReachDistance-1)
                    direction = new Vector2(0, 0);
                //If the frog is within attack range, attack
                if (dis > minAttackDistance && dis < maxAttackDistance && cooldown < 3)
                {
                    Vector2 vec = position + direction;
                    LauchAttack(vec, direction, 10);
                    cooldown += fireCooldown;
                }
                else
                    cooldown--;
            }
            else
            {
                //Wander around the map
                if (dirCoolDown < 3)
                {
                    direction = new Vector2((float)rand.NextDouble(), (float)rand.NextDouble());
                    //Slight point to the center
                    direction += new Vector2(Map.GetSize.X*Map.TileSize / 2 - position.X, Map.GetSize.Y* Map.TileSize / 2 - position.Y).Normalized()/2;
                    dirCoolDown = dirChangeTimer;
                }
                else
                    dirCoolDown--;
            }
            //Check for collisions 
            if (!CollisionCheckV2())
                position += direction;
        }
        /// <summary>
        /// Distance between 2 vectors to the power 2: c² = a²+b²
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double DistanceBetween(Vector2 a, Vector2 b)
        {
            return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        }

        /// <summary>
        /// Go throught the list of messages one message per pass
        /// </summary>
        private void TreatCommands()
        {
            Message mess = commandList.Pop();
            switch(mess.command)
            {
                case Command.Hit:
                    health-= mess.value;
                    if (health < 0)
                        isAlive = false;
                    break;
                default:
                    break;
            }
        }
    }
}
