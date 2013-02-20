using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace molnprojektet
{

    class WindowHandler
    {
        private Window currentWindow;

        public void UpdateWindow()
        {
            currentWindow.Update();
        }

        public void DrawWindowGraphics()
        {
            currentWindow.Draw();
        }
        
        public void ChangeWindow(Window newWindow)
        {
            currentWindow = newWindow;
        }
    }
}
