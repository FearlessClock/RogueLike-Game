using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace RogueLikeMovement
{
    class ContentPipe
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">Defaults to Content</param>
        /// <param name="pixelated"></param>
        /// <returns></returns>
        public static Texture2D LoadTexture(string filePath, bool pixelated = false)
        {
            filePath = "Content/" + filePath;
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist at " + filePath + ".");
            }
            Bitmap bmp = new Bitmap(filePath);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, pixelated ? (int)TextureMinFilter.Nearest : (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, pixelated ? (int)TextureMagFilter.Nearest : (int)TextureMagFilter.Linear);

            return new Texture2D(id, new OpenTK.Vector2(bmp.Width, bmp.Height));
        }
    }
}
