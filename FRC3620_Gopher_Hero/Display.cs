using System;
using Microsoft.SPOT;
using CTRE.Gadgeteer.Module;

namespace FRC3620_Gopher_Hero
{
    public class Display
    {
        DisplayModule.LabelSprite _labelConnected, _labelEnabled, _labelVoltage, _label_limit;
        DisplayModule.LabelSprite[] _labelBarrelStatus, _labelBarrelPSI;

        ChangeDetector cd_connected, cd_enabled, cd_voltage, cd_limit;
        ChangeDetector[] cd_barrelStatus, cd_barrelPSI;

        public Display ()
        {
            cd_connected = new ChangeDetector();
            cd_enabled = new ChangeDetector();
            cd_voltage = new ChangeDetector();
            cd_limit = new ChangeDetector();
            cd_barrelStatus = new ChangeDetector[Shooter.NUMBER_OF_BARRELS];
            cd_barrelPSI = new ChangeDetector[Shooter.NUMBER_OF_BARRELS];
            for (int i = 0; i < Shooter.NUMBER_OF_BARRELS; i++)
            {
                cd_barrelStatus[i] = new ChangeDetector();
                cd_barrelPSI[i] = new ChangeDetector();
            }

            DisplayModule _displayModule = new DisplayModule(CTRE.HERO.IO.Port1, DisplayModule.OrientationType.Portrait);

            /* lets pick a font */
            Font _bigFont = Properties.Resources.GetFont(Properties.Resources.FontResources.Freesans_bold_18);
            int size = 20;
            int f_y = 0;

            _labelConnected = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 0, f_y, 128, size);
            f_y += size;
            _labelEnabled = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 0, f_y, 128, size);
            f_y += size;
            _labelVoltage = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 0, f_y, 128, size);
            f_y += size;
            _label_limit = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 0, f_y, 128, size);

            _labelBarrelStatus = new DisplayModule.LabelSprite[Shooter.NUMBER_OF_BARRELS];
            _labelBarrelPSI = new DisplayModule.LabelSprite[Shooter.NUMBER_OF_BARRELS];

            f_y = 80;
            _labelBarrelStatus[0] = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 0, f_y, 64, size);
            _labelBarrelStatus[1] = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 64, f_y, 64, size);
            f_y += size;
            _labelBarrelPSI[0] = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 0, f_y, 64, size);
            _labelBarrelPSI[1] = _displayModule.AddLabelSprite(_bigFont, DisplayModule.Color.White, 64, f_y, 64, size);

            if (false)
            {
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
        }

        public void updateConnected (bool connected)
        {
            String value = connected ? "Connected" : "No Gamepad";
            if (cd_connected.valueChanged(value))
            {
                _labelConnected.SetText(value);
                _labelConnected.SetColor(connected ? DisplayModule.Color.Green : DisplayModule.Color.Red);
            }
        }

        public void updateEnabled(bool enabled)
        {
            String value = enabled ? "Enabled" : "Disabled";
            if (cd_enabled.valueChanged(value))
            {
                _labelEnabled.SetText(value);
                _labelEnabled.SetColor(enabled ? DisplayModule.Color.Green : DisplayModule.Color.Red);
            }
        }

        public void updateLimitSwitch(bool hit)
        {
            String value = hit ? "hit" : "not hit";
            if (cd_limit.valueChanged(value))
            {
                _label_limit.SetText(value);
            }
        }

        public void updateVoltage (double v)
        {
            String s = v.ToString("F1");
            if (cd_voltage.valueChanged(s))
            {
                _labelVoltage.SetText(s);
            }
        }

        public void updateBarrelStatus (int b, String s)
        {
            if (cd_barrelStatus[b].valueChanged(s))
            {
                _labelBarrelStatus[b].SetText(s);
                updateBarrelPSI(b, "");
            }
        }

        public void updateBarrelPSI(int b, String s)
        {
            if (cd_barrelPSI[b].valueChanged(s))
            {
                _labelBarrelPSI[b].SetText(s);
            }
        }

    }

    class ChangeDetector
    {
        string lastValue;

        public ChangeDetector():this("")
        {
        }

        public ChangeDetector(string initialValue)
        {
            this.lastValue = initialValue;
        }

        public bool valueChanged(string v)
        {
            bool rv = ! this.lastValue.Equals(v);
            lastValue = v;
            return rv;
        }
    }
}
