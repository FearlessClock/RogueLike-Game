using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    class Entity
    {
        public Vector2 position;
        public Texture2D[] texture;
        public int selectedTexture;
        public Texture2D GetTexture
        {
            get { return texture[selectedTexture]; }
        }
        public Vector2 GetPosition
        {
            get { return position; }
        }
        public Vector2 direction;
        public Vector2 facing;
        public Vector2 movement;
        public int size = 7;

        //Identification
        public int id;

        //Health of the entity
        public double health;

        //Is the entity alive
        public bool isAlive;

        /// <summary>
        /// Load in all the textures needed for the entity
        /// </summary>
        /// <param name="textureFilePaths">File paths for all the textures used</param>
        public void LoadTextures(int nmbrOfPaths, string[] textureFilePaths)
        {
            texture = new Texture2D[nmbrOfPaths];
            for(int i = 0; i < nmbrOfPaths; i++)
                texture[i] = ContentPipe.LoadTexture(textureFilePaths[i]);

        }
        /// <summary>
        /// Draw the entity onto the screen
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public GraphicsBuffer Draw(GraphicsBuffer buf)
        {
            List<Vertex> vert = new List<Vertex>();
            vert.AddRange(buf.vertBuffer);
            List<uint> index = new List<uint>();
            index.AddRange(buf.indexBuffer);

            vert.Add(new Vertex(position, new Vector2(0, 0)));
            Vector2 vec = new Vector2(position.X + size, position.Y);
            vert.Add(new Vertex(vec, new Vector2(1, 0)));
            vec = new Vector2(position.X + size, position.Y + size);
            vert.Add(new Vertex(vec, new Vector2(1, 1)));
            vec = new Vector2(position.X, position.Y + size);
            vert.Add(new Vertex(vec, new Vector2(0, 1)));

            //uint[] indexes = new uint[4] { 0, 1, 2, 3 };
            //index.addrange(indexes);
            if (index.Count > 0)
            {
                int count = index.Count-1;
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
