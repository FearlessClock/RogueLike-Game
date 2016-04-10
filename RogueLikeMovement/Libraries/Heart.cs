using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RogueLikeMovement
{
    class Heart : Entity
    {
        public Heart(Vector2 pos)
        {
            position = pos;
            isAlive = true;
            size = 10;
        }
    }
}
