using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    struct GraphicsBuffer
    {
        public Vertex[] vertBuffer;
        public uint[] indexBuffer;
        public int VBO;
        public int IBO;
        public GraphicsBuffer(Vertex[] vert, uint[] index, int vbo, int ibo)
        {
            vertBuffer = vert;
            indexBuffer = index;
            VBO = vbo;
            IBO = ibo;
        }

        public void Empty()
        {
            vertBuffer = new Vertex[0];
            indexBuffer = new uint[0];
        }
    }
}
