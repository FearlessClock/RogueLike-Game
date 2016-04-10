using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace RogueLikeMovement
{
    class Camera
    {
        //States to check keyboard key presses
        static KeyboardState key;
        static KeyboardState lastKey;

        //States to check Mouse movement and button press states
        static MouseState mouse;
        static MouseState lastMouse;

        //Camera position, rotation, and scale
        public static Vector3 cameraPos = new Vector3(0, 0, 0);
        public static Vector2 CameraPosV2
        {
            get { return new Vector2(cameraPos.X, cameraPos.Y); }
        }

        public static Vector3 rot = new Vector3(0, 0, 0);
        public static Vector3 scale = new Vector3(2, 2, 0);
        //Camera movement speed
        static int step = 5;
        //Sprint speed of the camera
        static int booster = 1;

        //Position of the cursor on the screen
        public static Vector2 cursorPos = new Vector2(0, 0);
        //Size of the cursor
        static float cursorSize = 15;
        //Sensitivity of the mouse
        static double senX = 0.9;
        static double senY = 0.9;

        //Draw debug information
        public static bool DEBUGDraw = true;

        //Position that was last clicked
        static Vector2 lastClickPosition = new Vector2(-1, -1);
        //Position that was clicked now
        static Vector2 clickedPosition = new Vector2(-1, -1);
        //Which mouse has been clicked 1 Left, 2 Right
        static int WhichButtonPressed = -1;
        //Which button was last pressed
        static int lastButtonPressed = -1;

        //Copy of the window for each check
        static GameWindow window;

        //Var checking the shooting of the player
        static bool isShooting;
        static public bool IsShooting
        {
            get
            {
                bool shoot = isShooting;
                isShooting = false;
                return shoot; 
            }
        }

        //Vector storing the direction information
        static Vector2 movementDirecion;

        static public Vector2 GetKeyPressDirection
        {
            get { return movementDirecion; }
        }

        //Get the last clicked position
        static public Vector2 GetClickPos
        {
            get { return lastClickPosition; }
        }

        static public int ScreenWidth
        {
            get { return window.Width; }
        }
        static public int ScreenHeight
        {
            get { return window.Height; }
        }

        //Flag for ingame menu
        static bool inGameMenu = false;
        public static bool InGameMenuActive
        {
            get { return inGameMenu; }
            set { inGameMenu = value; }
        }

        /// <summary>
        /// Run when the window is loaded
        /// </summary>
        /// <param name="gw">Game window of the current session</param>
        /// <param name="curSize">Size of the cursor in the program</param>
        public static void SetupCamera(GameWindow gw, float curSize)
        {
            window = gw;
            cursorSize = curSize;
        }

        public static void ResetCamera()
        {
            cameraPos = Vector3.Zero;
            scale = new Vector3(1, 1, 0);
            rot = Vector3.Zero;
        }
        public static void GameReadyCamera()
        {
            scale = new Vector3(2.5F, 2.5F, 0);
        }

        /// <summary>
        /// Setup everything for the camera
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="curSize"></param>
        public static void SetupCamera(GameWindow gw, float curSize, Vector3 camPos, Vector3 camScale, Vector3 camRot)
        {
            window = gw;
            cursorSize = curSize;

            cameraPos = camPos;
            rot = camRot;
            scale = camScale;
        }

        static bool end = true;
        /// <summary>
        /// Check if any of the buttons or keys have been checked
        /// </summary>
        public static void CheckPresses()
        {
            //Get the newest state of the keyboard
            key = Keyboard.GetState();

            //Check if the key Q is pressed and close the program if it is
            if (key.IsKeyDown(Key.Q))
                window.Close();

            movementDirecion = new Vector2(0, 0);
            //Checks for the movement direction
            if (key.IsKeyDown(Key.Up) )//&& lastKey.IsKeyUp(Key.Up))
                movementDirecion.Y--;
            if (key.IsKeyDown(Key.Down) )//&& lastKey.IsKeyUp(Key.Down))
                movementDirecion.Y++;
            if (key.IsKeyDown(Key.Left) )//&& lastKey.IsKeyUp(Key.Left))
                movementDirecion.X--;
            if (key.IsKeyDown(Key.Right) )//&& lastKey.IsKeyUp(Key.Right))
                movementDirecion.X++;
           /* //Checks for nullification
            if (key.IsKeyUp(Key.Up) && lastKey.IsKeyDown(Key.Up))
                movementDirecion.Y = 0;
            if (key.IsKeyUp(Key.Down) && lastKey.IsKeyDown(Key.Down))
                movementDirecion.Y = 0;
            if (key.IsKeyUp(Key.Left) && lastKey.IsKeyDown(Key.Left))
                movementDirecion.X = 0;
            if (key.IsKeyUp(Key.Right) && lastKey.IsKeyDown(Key.Right))
                movementDirecion.X = 0;*/

            //Check if the camera needs speeding up
            if (key.IsKeyDown(Key.ShiftRight) && lastKey.IsKeyUp(Key.ShiftRight))
                booster = 2;
            if (key.IsKeyUp(Key.ShiftRight) && lastKey.IsKeyDown(Key.ShiftRight))
                booster = 1;

            //Release the mouse from the cameras grip
            if (key.IsKeyDown(Key.C) && lastKey.IsKeyUp(Key.C))
                end = !end;

            //Open up the in game menu
            if (key.IsKeyDown(Key.Escape) && lastKey.IsKeyUp(Key.Escape))
                inGameMenu = !inGameMenu;

            //Shoot
            if (key.IsKeyDown(Key.Space) && lastKey.IsKeyUp(Key.Space))
                isShooting = true;
            if (key.IsKeyUp(Key.Space) && lastKey.IsKeyDown(Key.Space))
                isShooting = false;

            //Move the Camera if the corrisponding button is pressed
            if (key.IsKeyDown(Key.Keypad5))
                cameraPos.Y -= step * booster;
            else if (key.IsKeyDown(Key.Keypad8))
                cameraPos.Y += step * booster;
            if (key.IsKeyDown(Key.Keypad4))
                cameraPos.X -= step * booster;
            else if (key.IsKeyDown(Key.Keypad6))
                cameraPos.X += step * booster;

            //Rotate the screen by 1 degree around the Z axis (Out of screen)
            if (key.IsKeyDown(Key.Keypad7))
                rot.Z += MathHelper.DegreesToRadians(1);
            else if (key.IsKeyDown(Key.Keypad9))
                rot.Z -= MathHelper.DegreesToRadians(1);

            //"Move" the camera towards/Away from the screen
            if (key.IsKeyDown(Key.Keypad1))
            {
                scale.X += 0.04F;
                scale.Y += 0.04F;
            }
            else if (key.IsKeyDown(Key.Keypad3))
            {
                scale.X -= 0.04F;
                scale.Y -= 0.04F;
            }

            //Activate debug drawing
            if (key.IsKeyDown(Key.D) && lastKey.IsKeyUp(Key.D))
            {
                DEBUGDraw = !DEBUGDraw;
            }

            //Set the
            lastKey = key;

            UpdateMouseState();

            //Move the cursor by the distance that the mouse has moved from the center position that it is set to
            cursorPos.X += (float)((mouse.X - window.Width / 2) / senX);
            cursorPos.X = Constraint(cursorPos.X, 0, window.Width);
            cursorPos.Y += (float)((mouse.Y - window.Height / 2) / senY);
            cursorPos.Y = Constraint(cursorPos.Y, 0, window.Height);

            //Check the mouse button presses and set the cursor positions
            #region Mouse Button presses
            if ((mouse.IsButtonDown(MouseButton.Left) && lastMouse.IsButtonUp(MouseButton.Left)))
            {
                clickedPosition = new Vector2(cursorPos.X - cameraPos.X, cursorPos.Y - cameraPos.Y);
                WhichButtonPressed = 1;
            }
            else if (mouse.IsButtonDown(MouseButton.Right) && lastMouse.IsButtonUp(MouseButton.Right))
            {
                clickedPosition = new Vector2(cursorPos.X - cameraPos.X, cursorPos.Y - cameraPos.Y);
                WhichButtonPressed = 2;
            }
            else if (mouse.IsButtonUp(MouseButton.Right) && lastMouse.IsButtonDown(MouseButton.Right))
            {
                lastClickPosition = clickedPosition;
                clickedPosition = new Vector2(-1, -1);
                lastButtonPressed = WhichButtonPressed;
                WhichButtonPressed = -1;
            }
            else if (mouse.IsButtonUp(MouseButton.Left) && lastMouse.IsButtonDown(MouseButton.Left))
            {
                lastClickPosition = clickedPosition;
                clickedPosition = new Vector2(-1, -1);
                lastButtonPressed = WhichButtonPressed;
                WhichButtonPressed = -1;
            }
            #endregion

        }


        /// <summary>
        /// Limit val between 2 values high and low
        /// </summary>
        /// <param name="val">Value to limit</param>
        /// <param name="low">Lowest posible value</param>
        /// <param name="high">Highest posible value</param>
        /// <returns>The limited value if val \< low return low, </returns>
        private static float Constraint(float val, float low, float high)
        {
            return val > high ? high : val < low ? low : val;
        }


        /// <summary>
        /// Draw the cursor to the screen
        /// </summary>
        /// <param name="buf">Buffer holding the current graphic information</param>
        /// <param name="size">Size of the mouse cursor</param>
        /// <param name="outBuf">What the results need to be placed in</param>
        public static void DrawCursorPosition(GraphicsBuffer buf, float size, out GraphicsBuffer outBuf)
        {
            List<Vertex> vert = new List<Vertex>();
            vert.AddRange(buf.vertBuffer);
            List<uint> index = new List<uint>();
            index.AddRange(buf.indexBuffer);

            Vector2 vec = new Vector2(cursorPos.X - cameraPos.X, cursorPos.Y - cameraPos.Y);

            vert.Add(new Vertex(vec, new Vector2(0, 0)));
            vec.X += size;
            vert.Add(new Vertex(vec, new Vector2(1, 0)));
            vec.Y += size;
            vert.Add(new Vertex(vec, new Vector2(1, 1)));
            vec.X -= size;
            vert.Add(new Vertex(vec, new Vector2(0, 1)));

            for (int i = 4; i > 0; i--)
            {
                index.Add((uint)(vert.Count - i));
            }

            outBuf = new GraphicsBuffer();
            outBuf = buf;
            outBuf.vertBuffer = vert.ToArray<Vertex>();
            outBuf.indexBuffer = index.ToArray<uint>();
        }

        /// <summary>
        /// Draw where the mouse clicked 
        /// </summary>
        /// <param name="buf">Buffer holding the current graphic information</param>
        /// 
        /// <param name="outBuf">What the results need to be placed in</param>
        private static void DrawClickPositions(GraphicsBuffer buf, out GraphicsBuffer outBuf)
        {

            List<Vertex> vert = new List<Vertex>();
            vert.AddRange(buf.vertBuffer);
            List<uint> index = new List<uint>();
            index.AddRange(buf.indexBuffer);
            Vector2 v = clickedPosition;

            vert.Add(new Vertex(v, new Vector2(0, 0)));
            Vector2 vec = v;
            vec.X += 10;
            vert.Add(new Vertex(vec, new Vector2(0, 1)));
            vec.Y += 10;
            vert.Add(new Vertex(vec, new Vector2(1, 1)));
            vec.X -= 10;
            vert.Add(new Vertex(vec, new Vector2(1, 0)));

            for (int i = 4; i > 0; i--)
            {
                index.Add((uint)(vert.Count - i));
            }

            outBuf = new GraphicsBuffer();
            outBuf.vertBuffer = vert.ToArray<Vertex>();
            outBuf.indexBuffer = index.ToArray<uint>();
        }

        public static void CenterCamera(Vector2 pos, Vector2 screenSize, Vector2 mapSize)
        {
            pos.X *= scale.X;
            pos.Y *= scale.Y;
            mapSize.X *= scale.X;
            mapSize.Y *= scale.Y;
            if (pos.X < screenSize.X / 2)
            {
                cameraPos.X = 0;
            }
            else if (pos.X > (mapSize.X - (Vector2.Divide(screenSize, 2).X)))
            {
                cameraPos.X = -(mapSize.X - screenSize.X);
            }
            else
            {
                cameraPos.X = -(pos.X - (int)(screenSize.X / 2));
            }

            if (pos.Y < screenSize.Y / 2)
            {
                cameraPos.Y = 0;
            }
            else if (pos.Y > (mapSize.Y - (Vector2.Divide(screenSize, 2).Y)))
            {
                cameraPos.Y = -(mapSize.Y - screenSize.Y);
            }
            else
            {
                cameraPos.Y = -pos.Y + (int)(screenSize.Y / 2);
            }
        }


        /// <summary>
        /// Move the camera by doing all the matrix math
        /// </summary>
        public static void MoveCamera()
        {
            Matrix4 proj = Matrix4.CreateOrthographicOffCenter(0, window.Width, window.Height, 0, 0, 1);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref proj);
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 mat = Matrix4.CreateTranslation(cameraPos);
            GL.LoadMatrix(ref mat);                 //Load the translation matrix into the modelView matrix
            mat = Matrix4.CreateScale(scale);
            GL.MultMatrix(ref mat);
            mat = Matrix4.CreateRotationX(rot.X);
            GL.MultMatrix(ref mat);
            mat = Matrix4.CreateRotationY(rot.Y);
            GL.MultMatrix(ref mat);
            mat = Matrix4.CreateRotationZ(rot.Z);
            GL.MultMatrix(ref mat);
        }

        public static bool HasClicked(MouseButton ms)
        {
            if (mouse.IsButtonDown(ms) && lastMouse.IsButtonUp(ms))
            {
                lastClickPosition = new Vector2(cursorPos.X, cursorPos.Y);
                return true;
            }
            else if (mouse.IsButtonUp(ms) && lastMouse.IsButtonUp(ms))
            {
                lastClickPosition = new Vector2(-1, -1);
            }
            return false;
        }

        private static void UpdateMouseState()
        {
            lastMouse = mouse;
            mouse = Mouse.GetCursorState();
            if (end)
                Mouse.SetPosition(window.Width / 2, window.Height / 2);
        }
        private static void UpdateKeyboardState()
        {
            lastKey = key;
            key = Keyboard.GetState();
        }

        /// <summary>
        /// Update all the camera functions and run the mouse and keyboard check
        /// </summary>
        /// <returns>The graphic buffers for the mouse</returns>
        public static GraphicsBuffer[] CameraUpdate()
        {
            GraphicsBuffer[] buffer = new GraphicsBuffer[2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i].vertBuffer = new Vertex[0];
                buffer[i].indexBuffer = new uint[0];
                buffer[i].IBO = GL.GenBuffer();
                buffer[i].VBO = GL.GenBuffer();
            }
            UpdateMouseState();
            UpdateKeyboardState();
            CheckPresses();
            DrawCursorPosition(buffer[0], cursorSize, out buffer[0]);
            DrawClickPositions(buffer[1], out buffer[1]);
            MoveCamera();
            return buffer;
        }

        public static void FullScreenDrawMode(bool OnOff)
        {
            if (OnOff)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
            }
            else
            {
                MoveCamera();
            }
        }
    }
}
