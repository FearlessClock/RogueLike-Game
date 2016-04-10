using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    /// <summary>
    /// Used to create explosions
    /// </summary>
    class Explosion : Moving_entity
    {
        //How long till it despawns
        int cooldown;
        public Explosion(Vector2 pos, int cooldownTimer)
        {
            cooldown = cooldownTimer;
            position = pos;
            isAlive = true;
        }

        public new GraphicsBuffer Draw(GraphicsBuffer buf)
        {

            List<Vertex> vert = new List<Vertex>();
            vert.AddRange(buf.vertBuffer);
            List<uint> index = new List<uint>();
            index.AddRange(buf.indexBuffer);
            float sizeValue = Math.Abs((float)(-0.0711*(cooldown-15)* (cooldown - 15)+16));

            Vector2 vec = new Vector2(position.X - sizeValue/3, position.Y- sizeValue/3);
            vert.Add(new Vertex(vec, new Vector2(0, 0)));
            vec = new Vector2(vec.X + sizeValue, vec.Y);
            vert.Add(new Vertex(vec, new Vector2(1, 0)));
            vec = new Vector2(vec.X, vec.Y + sizeValue);
            vert.Add(new Vertex(vec, new Vector2(1, 1)));
            vec = new Vector2(vec.X - sizeValue, vec.Y);
            vert.Add(new Vertex(vec, new Vector2(0, 1)));

            if (index.Count > 0)
            {
                int count = index.Count - 1;
                index.Add((uint)(index[count] + 1));
                index.Add((uint)(index[count] + 2));
                index.Add((uint)(index[count] + 3));
                index.Add((uint)(index[count] + 4));
            }
            else
            {
                index.Add((uint)(0));
                index.Add((uint)(1));
                index.Add((uint)(2));
                index.Add((uint)(3));
            }
            buf.vertBuffer = vert.ToArray<Vertex>();
            buf.indexBuffer = index.ToArray<uint>();
            return buf;
        }

        public void Update()
        {
            cooldown--;
            if(cooldown < 0)
            {
                isAlive = false;
            }
        }
    }
}
