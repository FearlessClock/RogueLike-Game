using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RogueLikeMovement
{
    class HUD
    {
        static double health;
        static Vector2 position;
        static float HUDSize;
        static Heart[] hearts;
        static GameWindow window;
        static Vector2 HUDPos;

        /// <summary>
        /// Setup the HUD
        /// </summary>
        /// <param name="heal"></param>
        static public void SetupHUD(double heal, Vector2 pos, float HudSize, GameWindow wind)
        {
            health = heal;
            position = pos;
            HUDSize = HudSize;
            hearts = new Heart[(int)(health / 10)];
            window = wind;
            HUDPos = new Vector2(window.X * 3 - 160, 10);
            for (int i = 0; i < health / 10; i++)
            {
                pos.X += HUDSize * 2;
                hearts[i] = new Heart(pos);
            }

        }

        static public GraphicsBuffer[] Draw(GraphicsBuffer[] buf, Vector2 playerPos)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i].isAlive)
                    buf[0] = hearts[i].Draw(buf[0]);
            }
            buf[1] = HealthCoverDraw(buf[1]);
            buf[7] = MinimapOutlineDraw(buf[2]);
            GraphicsBuffer[] mini = new GraphicsBuffer[3];
            for(int i = 0; i < mini.Length; i++)
            {
                mini[i] = new GraphicsBuffer();
                mini[i].VBO = GL.GenBuffer();
                mini[i].IBO = GL.GenBuffer();
                mini[i].Empty();
            }
            mini = MinimapDraw(mini, playerPos);
            buf[3] = mini[0];
            buf[4] = mini[1];
            buf[5] = mini[2];
            buf[2] = buf[7];
            buf[6] = DrawEnemyEntities(buf[6], playerPos); 
            return buf;
        }

        static public void Update(double heal)
        {
            health = heal;
            for(int i = 0; i < hearts.Length; i++)
            {
                if(health/10 < i+1)
                {
                    hearts[i].isAlive = false;
                }
            }
        }

        static private GraphicsBuffer DrawEnemyEntities(GraphicsBuffer buf, Vector2 playerPos)
        {
            Moving_entity[] enemies = EntityController.GetEnemyArray();

            int sizeValue = 6;
            List<Vertex> vert = new List<Vertex>();
            vert.AddRange(buf.vertBuffer);
            List<uint> index = new List<uint>();
            index.AddRange(buf.indexBuffer);

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].DistanceBetween(playerPos) < 14000)
                {
                    Vector2 vec = new Vector2(enemies[i].position.X - playerPos.X, enemies[i].position.Y- playerPos.Y);
                    vec /= 2;
                    Vector2 center = HUDPos;
                    center.X += (sizeValue * 10);
                    center.Y += (sizeValue * 10);
                    vec += center;
                    if (playerPos.Y / 10 - 10 < 0)
                        vec.Y -= (playerPos.Y / 10 - 10) * 6;
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
                }
            }
            buf.vertBuffer = vert.ToArray<Vertex>();
            buf.indexBuffer = index.ToArray<uint>();
            return buf;
        }
        static private GraphicsBuffer HealthCoverDraw(GraphicsBuffer buf)
        {
            Vector2 HUDPos = new Vector2(-10, position.Y - 40);
            int sizeValue = (int)(HUDSize*HUDSize) * 10;
            List<Vertex> vert = new List<Vertex>();
            vert.AddRange(buf.vertBuffer);
            List<uint> index = new List<uint>();
            index.AddRange(buf.indexBuffer);

            Vector2 vec = new Vector2(HUDPos.X, HUDPos.Y);
            vert.Add(new Vertex(vec, new Vector2(0, 0)));
            vec = new Vector2(vec.X + sizeValue, vec.Y);
            vert.Add(new Vertex(vec, new Vector2(1, 0)));
            vec = new Vector2(vec.X, vec.Y + HUDSize*11);
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
        static private GraphicsBuffer MinimapOutlineDraw(GraphicsBuffer buf)
        {
            int sizeValue = 125;
            List<Vertex> vert = new List<Vertex>();
            vert.AddRange(buf.vertBuffer);
            List<uint> index = new List<uint>();
            index.AddRange(buf.indexBuffer);

            Vector2 vec = new Vector2(HUDPos.X, HUDPos.Y);
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
        static private GraphicsBuffer[] MinimapDraw(GraphicsBuffer[] buf, Vector2 playerPos)
        {
            int sizeValue = 6;

            List<Vertex>[] vert = new List<Vertex>[3];
            for (int i = 0; i < vert.Length; i++)
            {
                vert[i] = new List<Vertex>();
                vert[i].AddRange(buf[i].vertBuffer);
            }

            List<uint>[] index = new List<uint>[3];

            for (int i = 0; i < index.Length; i++)
            {
                index[i] = new List<uint>();
                index[i].AddRange(buf[i].indexBuffer);
            }

            Vector2 vec = HUDPos;
            Vector2 center = HUDPos;
            center.X += sizeValue * 10;
            center.Y += sizeValue * 10;
            Vector2 placePlayer = center;
            if (playerPos.Y / 10 - 10 < 0)
                vec.Y -= (playerPos.Y / 10 - 10)*6;
            //placePlayer = Center(placePlayer, playerPos, sizeValue);
            int counter = 0;
            //Console.WriteLine("Camera pos: " + Camera.cameraPos.ToString() + " : " + playerPos.ToString());
            for (int i = (int)(playerPos.X / 10 - 10); i < playerPos.X / 10 + 10; i++)
            {
                for (int j = (int)(playerPos.Y / 10 - 10); j < (playerPos.Y / 10 + 10); j++)
                {
                    if ((i >= 0 && i < Map.GetMap.GetLength(0) && j >= 0 && j < Map.GetMap.GetLength(1)))
                    {
                        if (Math.Pow(center.X - vec.X, 2) + Math.Pow(center.Y - vec.Y, 2) < 3000)
                        {
                            if (Map.GetMap[i, j].walkable)
                            {
                                vert[0].Add(new Vertex(vec, new Vector2(0, 0)));
                                vec.X += sizeValue;
                                vert[0].Add(new Vertex(vec, new Vector2(1, 0)));
                                vec.Y += sizeValue;
                                vert[0].Add(new Vertex(vec, new Vector2(1, 1)));
                                vec.X -= sizeValue;
                                vert[0].Add(new Vertex(vec, new Vector2(0, 1)));

                                #region index
                                if (index[0].Count > 0)
                                {
                                    int count = index[0].Count - 1;
                                    index[0].Add((uint)(index[0][count] + 1));
                                    index[0].Add((uint)(index[0][count] + 2));
                                    index[0].Add((uint)(index[0][count] + 3));
                                    index[0].Add((uint)(index[0][count] + 4));
                                }
                                else
                                {
                                    index[0].Add((uint)(0));
                                    index[0].Add((uint)(1));
                                    index[0].Add((uint)(2));
                                    index[0].Add((uint)(3));
                                }
                                #endregion
                            }
                            else
                            {
                                vert[1].Add(new Vertex(vec, new Vector2(0, 0)));
                                vec.X += sizeValue;
                                vert[1].Add(new Vertex(vec, new Vector2(1, 0)));
                                vec.Y += sizeValue;
                                vert[1].Add(new Vertex(vec, new Vector2(1, 1)));
                                vec.X -= sizeValue;
                                vert[1].Add(new Vertex(vec, new Vector2(0, 1)));
                                #region Index
                                if (index[1].Count > 0)
                                {
                                    int count = index[1].Count - 1;
                                    index[1].Add((uint)(index[1][count] + 1));
                                    index[1].Add((uint)(index[1][count] + 2));
                                    index[1].Add((uint)(index[1][count] + 3));
                                    index[1].Add((uint)(index[1][count] + 4));
                                }
                                else
                                {
                                    index[1].Add((uint)(0));
                                    index[1].Add((uint)(1));
                                    index[1].Add((uint)(2));
                                    index[1].Add((uint)(3));
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            vec.Y += sizeValue;
                        }
                        counter++;
                    }
                }
                vec.Y -= sizeValue*counter;
                vec.X += sizeValue;
                counter = 0;
            }
            vec = placePlayer;
            vert[2].Add(new Vertex(vec, new Vector2(0, 0)));
            vec.X += sizeValue;
            vert[2].Add(new Vertex(vec, new Vector2(1, 0)));
            vec.Y += sizeValue;
            vert[2].Add(new Vertex(vec, new Vector2(1, 1)));
            vec.X -= sizeValue;
            vert[2].Add(new Vertex(vec, new Vector2(0, 1)));

            #region index
            if (index[2].Count > 0)
            {
                int count = index[0].Count - 1;
                index[2].Add((uint)(index[0][count] + 1));
                index[2].Add((uint)(index[0][count] + 2));
                index[2].Add((uint)(index[0][count] + 3));
                index[2].Add((uint)(index[0][count] + 4));
            }
            else
            {
                index[2].Add((uint)(0));
                index[2].Add((uint)(1));
                index[2].Add((uint)(2));
                index[2].Add((uint)(3));
            }
            #endregion


            for (int i = 0; i < vert.Length; i++)
            {
                buf[i].vertBuffer = vert[i].ToArray<Vertex>();
                buf[i].indexBuffer = index[i].ToArray<uint>();
            }
            return buf;
        }

        static private Vector2 Center(Vector2 center, Vector2 playerPos, int sizeValue)
        {
            if (playerPos.X / 10 < 10)
            {
                center.X += playerPos.X;
            }
            else if (playerPos.X / 10 > Map.GetSize.X - 10)
            {
                center.X += sizeValue * 10;
            }
            else
            {
                center.X += sizeValue * 10;
            }
            if (playerPos.Y / 10 < 10)
            {
                center.Y += playerPos.Y;
            }
            else if (playerPos.Y / 10 > Map.GetSize.Y - 10)
            {
                center.Y += sizeValue * 10;
            }
            else
            {
                center.Y += sizeValue * 10;
            }
            return center;
        }
    }
}
