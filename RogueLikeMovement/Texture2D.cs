using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace RogueLikeMovement
{
    struct Texture2D
    {
        private int id;
        private Vector2 size;
        public int ID { get { return id; } }
        public Vector2 Size { get { return size; } }
        public int Width { get { return (int)size.X; } }
        public int Height { get { return (int)size.Y; } }

        public Texture2D(int id, Vector2 size)
        {
            this.id = id;
            this.size = size;
        }
    }
}
