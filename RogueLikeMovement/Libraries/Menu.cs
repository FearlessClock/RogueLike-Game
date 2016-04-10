using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeMovement
{
    /// <summary>
    /// Inheritance class used to create menus
    /// </summary>
    class Menu
    {
        //List to store all the buttons
        List<Button> buttons = new List<Button>();

        //Textures for the different buttons
        Texture2D[] menuTex;

        //Buffers for the different buttons and background
        GraphicsBuffer[] menuBuf;
        
        public Menu(int nmbrOfButtons, Vector2[] positions, int[] widths, int[] heights, string[] textures, Func<bool>[] func)
        {
            if (nmbrOfButtons != positions.Length || nmbrOfButtons != widths.Length || nmbrOfButtons != heights.Length || nmbrOfButtons != textures.Length-1)
                throw new Exception("The number of buttons don't corrispond to the number of parameters");

            menuTex = new Texture2D[nmbrOfButtons + 1]; //+1 for the background
            menuBuf = new GraphicsBuffer[nmbrOfButtons + 1]; //+1 for the background

            //For the background
            menuTex[0] = ContentPipe.LoadTexture(textures[0]);
            menuBuf[0] = new GraphicsBuffer();
            menuBuf[0].IBO = GL.GenBuffer();
            menuBuf[0].VBO = GL.GenBuffer();
            menuBuf[0].Empty();

            for (int i = 0; i < nmbrOfButtons; i++)
            {
                menuTex[i+1] = ContentPipe.LoadTexture(textures[i+1]);

                menuBuf[i+1] = new GraphicsBuffer();
                menuBuf[i+1].IBO = GL.GenBuffer();
                menuBuf[i+1].VBO = GL.GenBuffer();
                menuBuf[i+1].Empty();

                buttons.Add(new Button(positions[i], widths[i], heights[i], menuTex[i+1], func[i]));
            }
        }

        public void Update()
        {
            for(int i = 0; i < buttons.Count; i++)
            {
                if (Camera.HasClicked(OpenTK.Input.MouseButton.Left) && buttons[i].isClicked(Camera.cursorPos))
                    break;
            }
        }

        public void Draw()
        {
            Camera.ResetCamera();
            Camera.MoveCamera();

            menuBuf[0].vertBuffer = new Vertex[4] { new Vertex(new Vector2(0,0), new Vector2(0,0)),
                                                    new Vertex(new Vector2(Camera.ScreenWidth, 0), new Vector2(1, 0)),
                                                    new Vertex(new Vector2(Camera.ScreenWidth, Camera.ScreenHeight), new Vector2(1, 1)),
                                                    new Vertex(new Vector2(0, Camera.ScreenHeight), new Vector2(0, 1)) };
            menuBuf[0].indexBuffer = new uint[4] { 0, 1, 2, 3 };
            BufferFill(menuBuf[0]);
            GL.BindTexture(TextureTarget.Texture2D, menuTex[0].ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, menuBuf[0].VBO);
            GL.VertexPointer(2, VertexPointerType.Float, Vertex.SizeInBytes, (IntPtr)0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes));
            GL.ColorPointer(4, ColorPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes * 2));
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, menuBuf[0].IBO);
            GL.DrawElements(PrimitiveType.Quads, menuBuf[0].indexBuffer.Length, DrawElementsType.UnsignedInt, 0);

            //Drawing for the buttons
            for (int i = 1; i < buttons.Count+1; i++)
            {
                menuBuf[i].Empty();
                menuBuf[i] = buttons[i-1].Draw(menuBuf[i]);
                BufferFill(menuBuf[i]);
            }

            for (int i = 1; i < menuBuf.Length; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, buttons[i-1].GetTexture.ID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, menuBuf[i].VBO);
                GL.VertexPointer(2, VertexPointerType.Float, Vertex.SizeInBytes, (IntPtr)0);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes));
                GL.ColorPointer(4, ColorPointerType.Float, Vertex.SizeInBytes, (IntPtr)(Vector2.SizeInBytes * 2));
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, menuBuf[i].IBO);
                GL.DrawElements(PrimitiveType.Quads, menuBuf[i].indexBuffer.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        private void BufferFill(GraphicsBuffer buf)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf.VBO);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertex.SizeInBytes * buf.vertBuffer.Length), buf.vertBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf.IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * (buf.indexBuffer.Length)), buf.indexBuffer, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
