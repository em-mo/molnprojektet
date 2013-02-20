using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace molnprojektet
{
    interface Window
    {
        void Initialize(SpriteBatch batch);
        void Update();
        void Draw();
    }
}
