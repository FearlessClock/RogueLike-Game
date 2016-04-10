using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    /// <summary>
    /// Class used to make buttons
    /// </summary>
    class Button
    {
        //Position of the button
        Vector2 position;
        //values for the size of the button
        int width;
        int height;
        //Texture of the button
        Texture2D tex;
        //Callback function
        Func<bool> callback;

        public Texture2D GetTexture
        {
            get { return tex; }
        }
        public Button(Vector2 pos, int w, int h, Texture2D tex, Func<bool> func)
        {
            position = pos;
            width = w;
            height = h;
            this.tex = tex;
            callback = func;
        }

        public bool isClicked(Vector2 pos)
        {
            //I need to add deligates to make this work well
            if((pos.X > position.X && pos.X < position.X + width) && (pos.Y > position.Y && pos.Y < position.Y + height))
            {
                Console.WriteLine(tex.ID + " Is the id of the texture of the button that was clicked");
                callback();
                return true;
            }
            return false;
        }

        public GraphicsBuffer Draw(GraphicsBuffer buf)
        {
            List<Vertex> vert = new List<Vertex>();
            vert.AddRange(buf.vertBuffer);
            List<uint> index = new List<uint>();
            index.AddRange(buf.indexBuffer);

            Vector2 vec = new Vector2(position.X, position.Y);
            vert.Add(new Vertex(vec, new Vector2(0, 0)));
            vec = new Vector2(vec.X + width, vec.Y);
            vert.Add(new Vertex(vec, new Vector2(1, 0)));
            vec = new Vector2(vec.X, vec.Y + height);
            vert.Add(new Vertex(vec, new Vector2(1, 1)));
            vec = new Vector2(vec.X - width, vec.Y);
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
    }
}
