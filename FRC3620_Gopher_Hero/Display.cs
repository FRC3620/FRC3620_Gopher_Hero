using System;
using Microsoft.SPOT;
using CTRE.Gadgeteer.Module;

namespace FRC3620_Gopher_Hero
{
    public class Display
    {
        DisplayModule.LabelSprite _labelTitle, _labelBtn;
        public Display ()
        {
            DisplayModule _displayModule = new DisplayModule(CTRE.HERO.IO.Port1, DisplayModule.OrientationType.Portrait);

            /* lets pick a font */
            Font _smallFont = Properties.Resources.GetFont(Properties.Resources.FontResources.small);
            Font _bigFont = Properties.Resources.GetFont(Properties.Resources.FontResources.NinaB);
            _labelTitle = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 0, 0, 80, 16);
            _labelBtn = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 0, 16, 100, 16);

            // if we are Landscape, then DisplayHeight is the X screen dimension, and DisplayWidth is the Y dimension
            for (int x = 0; x < _displayModule.DisplayWidth; x += 16)
            {
                _displayModule.AddRectSprite(DisplayModule.Color.White, x, 0, 1, _displayModule.DisplayHeight);
            }
            for (int y = 0; y < _displayModule.DisplayHeight; y += 16)
            {
                _displayModule.AddRectSprite(DisplayModule.Color.White, 0, y, _displayModule.DisplayWidth, 1);
            }
        }

        public void updateConnected (bool connected)
        {
            if (connected)
            {
                _labelTitle.SetText("Connected");
                _labelTitle.SetColor(DisplayModule.Color.Green);
            }
            else
            {
                _labelTitle.SetText("No Gamepad");
                _labelTitle.SetColor(DisplayModule.Color.Red);
            }
        }

        public void updateEnabled(bool enabled)
        {
            if (enabled)
            {
                _labelBtn.SetText("Enabled");
                _labelBtn.SetColor(DisplayModule.Color.Green);
            }
            else
            {
                _labelBtn.SetText("Disabled");
                _labelBtn.SetColor(DisplayModule.Color.Red);
            }
        }

    }
}
