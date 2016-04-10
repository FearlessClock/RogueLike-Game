using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace RogueLikeMovement
{
    struct Vertex
    {
        public Vector2 position;
        public Vector2 texCoord;
        public Vector4 color;

        public Color Color
        {
            get
            {
                return Color.FromArgb((int)(255 * color.W), (int)(255 * color.X), (int)(255 * color.Y), (int)(255 * color.Z));
            }
            set
            {
                this.color = new Vector4(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
            }

        }
        static public int SizeInBytes
        {
            get { return Vector2.SizeInBytes * 2 + Vector4.SizeInBytes; }
        }

        public Vertex(Vector2 position, Vector2 texCoord)
        {
            this.position = position;
            this.texCoord = texCoord;
            this.color = new Vector4(1, 1, 1, 1);
        }

        public override string ToString()
        {
            return position.X + ":" + position.Y;
        }


    }
    class Game
    {
        Random rand = new Random();
        public GameWindow window;
        Texture2D wallTex;
        Texture2D floorTex;
        Texture2D bulletTex;
        Texture2D frogTex;
        Texture2D explosionTex;
        Texture2D[] HUDTex = new Texture2D[8];

        string[] mainMenuTextureStrings = new string[3] { "HUD\\Background.png", "HUD\\Start.png", "HUD\\Quit.png" };
        string[] inGameMenuTextureStrings = new string[4] { "HUD\\InGameMenu\\Background.png", "HUD\\InGameMenu\\Resume.png", "HUD\\InGameMenu\\Restart.png", "HUD\\InGameMenu\\Quit.png" };
        //Start of the vertex buffer
        GraphicsBuffer buffer = new GraphicsBuffer();

        //Bullets
        GraphicsBuffer[] bulletBuffer = new GraphicsBuffer[8];

        //Heads up display
        GraphicsBuffer[] HUDBuf = new GraphicsBuffer[8];

        //Buffer for the mouse cursor
        GraphicsBuffer mouseBuf = new GraphicsBuffer();
        //Time keeping
        Stopwatch sw = new Stopwatch();

        //Main menu object
        Vector2[] MMbuttonPos = new Vector2[2] { new Vector2(310, 150), new Vector2(320, 250) };
        int[] MMbuttonWidth = new int[2] { 150, 130 };
        int[] MMbuttonHeight = new int[2] { 75, 100 };
        Menu mainMenu;
        Vector2[] IGMButtonPos = new Vector2[3] { new Vector2(310, 150), new Vector2(305, 250), new Vector2(310, 350) };
        int[] IGMButtonWidth = new int[3] { 100, 90, 80 };
        int[] IGMButtonHeight = new int[3] { 80, 80, 80 };
        Menu inGameMenu;

        //Time ellapsed calculations
        long Elappsedtime = 0;
        int updateInterval;


        public Game(GameWindow windowInput)
        {
            window = windowInput;

            window.Load += Window_Load;
            window.RenderFrame += Window_RenderFrame;
            window.UpdateFrame += Window_UpdateFrame;
            window.Closing += Window_Closing;
            
            Camera.SetupCamera(window, 30, new Vector3(0, 0, 0), new Vector3(1, 1, 0), new Vector3(0,0,0));
            
        }


        GraphicsBuffer[] buf;
        Player player;

        int id = 0;
        int nmbrOfFrogs = 20;
        GraphicsBuffer frogBuf;
        GraphicsBuffer explosionBuf;

        int gameState = 0;
        #region Button Functions
        //Functions for the main menu
        public bool Start()
        {
            Camera.scale.X = 2;
            Camera.scale.Y = 2;
            gameState = 1;
            return true;
        }
        public bool Quit()
        {
            window.Close();
            return true;
        }

        //Functions for the ingame menu
        //QUIT
        public bool Resume()
        {
            Camera.InGameMenuActive = false;
            gameState = 1;
            return true;
        }
        public bool Restart()
        {
            player = new Player(new Vector2(200, 300), new Vector2(0, 0), Map.GetMap);
            gameState = 1;
            Camera.InGameMenuActive = false;
            EntityController.SetupEntityController(ref player);
            for (int i = 0; i < nmbrOfFrogs; i++)
            {
                EntityController.AddEntity((new Vector2(rand.Next(0, (int)Map.GetSize.X * 10), rand.Next(0, (int)Map.GetSize.Y * 10))), new Vector2((float)rand.NextDouble(), (float)rand.NextDouble()), id);
                id++;
            }
            HUD.SetupHUD(player.health, new Vector2(30, window.Height - 15), 5, window);
            Time.currentTime = 0;

            sw.Restart();
            return true;
        }
        #endregion
        private void Window_Load(object sender, EventArgs e)
        {
            floorTex = ContentPipe.LoadTexture("Floor.bmp");
            wallTex = ContentPipe.LoadTexture("Wall.bmp");
            bulletTex = ContentPipe.LoadTexture("Bullet\\BulletUp.png");
            frogTex = ContentPipe.LoadTexture("Frog\\Frog.png");
            explosionTex = ContentPipe.LoadTexture("Bullet\\Explosion.png");
            //Hud textures
            HUDTex[0] = ContentPipe.LoadTexture("HUD\\Heart.png");
            HUDTex[1] = ContentPipe.LoadTexture("HUD\\HeartCover.png");
            HUDTex[7] = ContentPipe.LoadTexture("HUD\\Minimap.png");
            HUDTex[2] = ContentPipe.LoadTexture("HUD\\BackCover.png");
            HUDTex[3] = floorTex;
            HUDTex[4] = wallTex;
            Map.LoadMap("Content\\map.txt");
            player = new Player(new Vector2(200, 300), new Vector2(0, 0), Map.GetMap);
            HUDTex[5] = player.GetTexture;
            HUDTex[6] = ContentPipe.LoadTexture("HUD\\EnemyEntity.png");
            buffer.vertBuffer = new Vertex[0];

            Func<bool>[] func = new Func<bool>[2] { Start, Quit };
            //Main menu setup
            mainMenu = new Menu(2, MMbuttonPos, MMbuttonWidth, MMbuttonHeight, mainMenuTextureStrings, func);
            //In game menu setup
            func = new Func<bool>[3] { Resume, Restart, Quit };
            inGameMenu = new Menu(3, IGMButtonPos, IGMButtonWidth, IGMButtonHeight, inGameMenuTextureStrings, func);
            EntityController.SetupEntityController(ref player);
            for(int i = 0; i < nmbrOfFrogs; i++)
            {
                EntityController.AddEntity((new Vector2(rand.Next(0, (int)Map.GetSize.X*10), rand.Next(0, (int)Map.GetSize.Y*10))), new Vector2((float)rand.NextDouble(), (float)rand.NextDouble()), id);
                id++;
            }

            frogBuf = new GraphicsBuffer();
            frogBuf.VBO = GL.GenBuffer();
            frogBuf.IBO = GL.GenBuffer();
            frogBuf.Empty();

            explosionBuf = new GraphicsBuffer();
            explosionBuf.VBO = GL.GenBuffer();
            explosionBuf.IBO = GL.GenBuffer();
            explosionBuf.Empty();

            buffer.VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer.VBO);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * buffer.vertBuffer.Length), buffer.vertBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            buffer.indexBuffer = new uint[0];

            buffer.IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer.IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * (buffer.indexBuffer.Length)), buffer.indexBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            window.CursorVisible = false;
            buf = new GraphicsBuffer[2];
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = new GraphicsBuffer();
                buf[i].VBO = GL.GenBuffer();
                buf[i].IBO = GL.GenBuffer();
                buf[i].Empty();
            }

            for (int i = 0; i < bulletBuffer.Length; i++)
            {
                bulletBuffer[i] = new GraphicsBuffer();
                bulletBuffer[i].VBO = GL.GenBuffer();
                bulletBuffer[i].IBO = GL.GenBuffer();
                bulletBuffer[i].Empty();
            }

            //Bullet controller setup
            BulletController.LoadTextures();
            HUD.SetupHUD(player.health, new Vector2(30, window.Height - 15), 5, window);
            for (int i = 0; i < HUDBuf.Length; i++)
            {
                HUDBuf[i] = new GraphicsBuffer();
                HUDBuf[i].VBO = GL.GenBuffer();
                HUDBuf[i].IBO = GL.GenBuffer();
                HUDBuf[i].Empty();
            }

            mouseBuf.VBO = GL.GenBuffer();
            mouseBuf.IBO = GL.GenBuffer();
            mouseBuf.Empty();

            updateInterval = 20;
            sw.Start();
        }

        private void BufferFill(GraphicsBuffer buf)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf.VBO);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * buf.vertBuffer.Length), buf.vertBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf.IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * (buf.indexBuffer.Length)), buf.indexBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        
        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            
            //Add update with time function
            Elappsedtime += sw.ElapsedMilliseconds - Time.currentTime;
            Time.currentTime = sw.ElapsedMilliseconds;

            if (Elappsedtime >= updateInterval)
            {
                GameStateCheck();
                if (gameState == 0)
                {
                    Camera.CheckPresses();
                    mouseBuf.Empty();
                    mainMenu.Update();
                    Camera.DrawCursorPosition(mouseBuf, 20, out mouseBuf);
                    BufferFill(mouseBuf);
                }
                else if (gameState == 1)
                {
                    #region Game state 1
                    for (int i = 0; i < buf.Length; i++)
                    {
                        buf[i].Empty();
                    }
                    Camera.CenterCamera(player.GetPosition, new Vector2(window.Width, window.Height), new Vector2(Map.GetMap.GetLength(0) * 10, Map.GetMap.GetLength(1) * 10));
                    Camera.CameraUpdate();
                    buf = Map.FillBuffer(buf);
                    foreach (GraphicsBuffer b in buf)
                    {
                        BufferFill(b);
                    }
                    buffer.Empty();
                    frogBuf.Empty();
                    if (player.isAlive)
                        player.Update();
                    buffer = player.Draw(buffer);
                    for (int i = 0; i < bulletBuffer.Length; i++)
                    {
                        bulletBuffer[i].Empty();
                    }
                    EntityController.Update();
                    frogBuf = EntityController.Draw(frogBuf);
                    explosionBuf.Empty();
                    for (int i = 0; i < HUDBuf.Length; i++)
                    {
                        HUDBuf[i].Empty();
                    }
                    BulletController.Update();
                    bulletBuffer = BulletController.Draw(bulletBuffer);
                    explosionBuf = BulletController.DrawExplosion(explosionBuf);
                    HUD.Update(player.health);
                    HUDBuf = HUD.Draw(HUDBuf, player.position);
                    foreach (GraphicsBuffer b in HUDBuf)
                    {
                        BufferFill(b);
                    }
                    BufferFill(explosionBuf);
                    BufferFill(frogBuf);
                    foreach (GraphicsBuffer b in bulletBuffer)
                    {
                        BufferFill(b);
                    }
                    BufferFill(buffer);
                    #endregion
                }
                else if(gameState == 2)
                {
                    Camera.CheckPresses();
                    mouseBuf.Empty();
                    inGameMenu.Update();
                    Camera.DrawCursorPosition(mouseBuf, 20, out mouseBuf);
                    BufferFill(mouseBuf);
                }
                Elappsedtime -= updateInterval;
            }


        }
        /// <summary>
        /// Check in what state the game should be
        /// </summary>
        private void GameStateCheck()
        {
            if(gameState == 0)
            {

            }
            else if(gameState == 1)
            {
                gameState = Camera.InGameMenuActive ? 2 : 1;
                //Camera.ResetCamera();
            }
            else if(gameState == 2)
            {
                gameState = Camera.InGameMenuActive ? 2 : 1;
                Camera.GameReadyCamera();
            }
        }

        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            //Clear screen color
            GL.ClearColor(Color.FromArgb(0,0,0));
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Enable color blending, which allows transparency
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            //Blending everything for transparency
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


            //Enable all the different arrays
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            if (gameState == 0)
            {
                mainMenu.Draw();
                DrawToScreen(mouseBuf, player.GetTexture);
            }
            else if(gameState == 2)
            {
                inGameMenu.Draw();
                DrawToScreen(mouseBuf, player.GetTexture);

            }
            else if (gameState == 1)
            {
                //Bind the texture that will be used
                DrawToScreen(buf[0], floorTex);
                DrawToScreen(buf[1], wallTex);
                for (int i = 0; i < bulletBuffer.Length; i++)
                {
                    DrawToScreen(bulletBuffer[i], BulletController.GetTexture(i));
                }

                DrawToScreen(frogBuf, frogTex);
                DrawToScreen(buffer, player.GetTexture);
                DrawToScreen(explosionBuf, explosionTex);

                //Start of HUD drawing. Without all the scaling stuff
                Camera.FullScreenDrawMode(true);
                HUDTex[5] = player.GetTexture;
                for (int i = 0; i < HUDBuf.Length; i++)
                {
                    DrawToScreen(HUDBuf[i], HUDTex[i]);
                }
                Camera.FullScreenDrawMode(false);
                //To draw the different facing sprites, I could create an array that store the 
                //different squares for the different directions and then I for loop to draw them on the screen. 
                //If there is nothing inside, it will simply not draw anything.  
            }
            //Flush everything 
            GL.Flush();
            //Write the new buffer to the screen
            window.SwapBuffers();
        }

        private void DrawToScreen(GraphicsBuffer buf, Texture2D tex)
        {
            GL.BindTexture(TextureTarget.Texture2D, tex.ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf.VBO);
            GL.VertexPointer(2, VertexPointerType.Float, Vertex.SizeInBytes, (IntPtr)0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes));
            GL.ColorPointer(4, ColorPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes * 2));
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf.IBO);
            GL.DrawElements(PrimitiveType.Quads, buf.indexBuffer.Length, DrawElementsType.UnsignedInt, 0);

        }
    }
}
