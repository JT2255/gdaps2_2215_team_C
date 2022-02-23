using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace MurderMystery
{
    interface IInteractable
    {
        void Click(Rectangle position);

        void SingleMousePress();
    }
}
