using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    class Player : Moving_entity
    {
        
        string[] filepaths = new string[9] 
        {
            "Player\\playerDown.png", "Player\\playerDownLeft.png",
            "Player\\playerDownRight.png", "Player\\playerLeft.png",
            "Player\\playerRight.png", "Player\\playerUp.png",
            "Player\\playerUpLeft.png", "Player\\playerUpRight.png",
            "Player\\playerDead.png" };
        
        public Player(Vector2 pos, Vector2 dir, Terrain[,] carte)
        {
            position = pos;
            direction = dir;
            facing = new Vector2(0, 1);
            movement = new Vector2(0, 0);
            LoadTextures(filepaths.Length, filepaths);
            isAlive = true;
            health = 100;
            movementSpeed = 2f;
            FindTexture(dir);
        }


        public void Update()
        {
            if (commandList.Count > 0)
                TreatCommands();
            if (health <= 0)
                HeHasFinallyDied();
            movement = Camera.GetKeyPressDirection;
            if (!(movement.X == 0 && movement.Y == 0))
                facing = movement;
            direction = movement * movementSpeed;
            bool check = CollisionCheckV2();
            while(check)
            {
                check = CollisionCheckV2();
            }
            position += direction;
            
            FindTexture(direction);
            if(Camera.IsShooting) //Check if the player is shooting
            {
                LauchAttack(position, facing, 25);
                //BulletController.AddBullet(position, facing);
            }

            /*
            {
                //I know that (int) pos + dir != walkable.
                //So if I don't move, then I will be on an open spot. 
                //Si je suis à 12 et je passe à 9, je suis dans une position non walkable.
                //If i take the diff I get the movement speed
                //If i take (int)9/10 = 0 I need to be on the square (0+1)*10 = 10 which is walkable
                //So if I take (pos+dir+1)*10 I should find the square that I need to be on
                //This gives me the cordinates that aren't correct for the other componant so it messes everything up...
                //I need to find a vector that points from one corner to the edge that is troublesome.

            }*/
        }
        private void TreatCommands()
        {
            Message mess = commandList.Pop();
            switch (mess.command)
            {
                case Command.Hit:
                    health -= mess.value;
                    if (health < 0)
                        isAlive = false;
                    break;
                default:
                    break;
            }
        }

        private void HeHasFinallyDied()
        {
            isAlive = false;
        }
    }
}
