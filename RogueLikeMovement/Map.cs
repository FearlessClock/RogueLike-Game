using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    class Map
    {
        static Terrain[,] map;
        static int tileSize = 10;
        static public int TileSize
        {
            get { return tileSize; }
        }
        public static Terrain[,] GetMap
        {
            get { return map; }
        }
        public static Vector2 GetSize
        {
            get { return new Vector2(map.GetLength(0), map.GetLength(1)); }
        }
        public static void LoadMap(string fileName)
        {
            Texture2D floor = ContentPipe.LoadTexture("Floor.bmp");
            Texture2D wall = ContentPipe.LoadTexture("Wall.bmp");
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    string[] size;
                    line = sr.ReadLine();
                    size = line.Split(',');
                    map = new Terrain[Convert.ToInt32(size[0]), Convert.ToInt32(size[1])];
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    int y = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] == '0')
                            {
                                map[i, y] = new Terrain(floor, true);
                            }
                            else if (line[i] == '1')
                            {
                                map[i, y] = new Terrain(wall, false);
                            }
                        }
                        y++;
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public static GraphicsBuffer[] FillBuffer(GraphicsBuffer[] buf)
        {
            List<Vertex>[] vert = new List<Vertex>[buf.Length];
            List<uint>[] index = new List<uint>[buf.Length];
            for (int i = 0; i < vert.Length; i++)
            {
                vert[i] = new List<Vertex>();

                if (buf[i].vertBuffer != null)
                    vert[i].AddRange(buf[i].vertBuffer);
            }
            for (int i = 0; i < index.Length; i++)
            {
                index[i] = new List<uint>();
                if (buf[i].indexBuffer != null)
                    index[i].AddRange(buf[i].indexBuffer);
            }

            uint countWalkable = 0;
            uint countNonWalkable = 0;
            int size = 10;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j].walkable)
                    {
                        vert[0].Add(new Vertex(new Vector2(size * i, size * j), new Vector2(0, 0)));
                        vert[0].Add(new Vertex(new Vector2(size * i + size, size * j), new Vector2(1, 0)));
                        vert[0].Add(new Vertex(new Vector2(size * i + size, size * j + size), new Vector2(1, 1)));
                        vert[0].Add(new Vertex(new Vector2(size * i, size * j + size), new Vector2(0, 1)));

                        index[0].Add(countWalkable++);
                        index[0].Add(countWalkable++);
                        index[0].Add(countWalkable++);
                        index[0].Add(countWalkable++);
                    }
                    else
                    {
                        vert[1].Add(new Vertex(new Vector2(size * i, size * j), new Vector2(0, 0)));
                        vert[1].Add(new Vertex(new Vector2(size * i + size, size * j), new Vector2(1, 0)));
                        vert[1].Add(new Vertex(new Vector2(size * i + size, size * j + size), new Vector2(1, 1)));
                        vert[1].Add(new Vertex(new Vector2(size * i, size * j + size), new Vector2(0, 1)));

                        index[1].Add(countNonWalkable++);
                        index[1].Add(countNonWalkable++);
                        index[1].Add(countNonWalkable++);
                        index[1].Add(countNonWalkable++);
                    }
                }
            }
            for (int i = 0; i < vert.Length; i++)
            {
                buf[i].vertBuffer = vert[i].ToArray<Vertex>();
                buf[i].indexBuffer = index[i].ToArray<uint>();
            }
            return buf;
        }
    }
}
