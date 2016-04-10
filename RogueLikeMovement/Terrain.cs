using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    struct Terrain
    {
        Texture2D texture;
        public bool walkable;

        public Terrain(Texture2D tex, bool walk)
        {
            texture = tex;
            walkable = walk;
        }

        public override string ToString()
        {
            return walkable ? "Is walkable" : "Not walkable";
        }
    }
}
