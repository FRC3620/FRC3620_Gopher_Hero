using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix;

namespace FRC3620_Gopher_Hero
{
    public class Program
    {
        public static void Main()
        {
            DifferentialDrive drive = new DifferentialDrive(Hardware._leftDrive, Hardware._rightDrive);

            Lid lid = new Lid(Hardware._lid, Hardware.lid_limit_switch);

            Shooter shooter = new Shooter();
            shooter.startup();

            CTRE.Phoenix.Controller.GameControllerValues gv = new CTRE.Phoenix.Controller.GameControllerValues();

            bool btn_a_was = false;
            bool btn_b_was = false;

            Timer tick = new Timer();
            tick.reset();
            tick.start();

            while (true)
            {
                bool enabled = false;
                // only run motors if Gamepad is attached
                if (Hardware._gamepad.GetConnectionStatus() == UsbDeviceConnection.Connected)
                {
                    enabled = true;
                    Hardware._display.updateConnected(true);
                } else
                {
                    Hardware._display.updateConnected(false);
                }

                if (enabled) { 
                    CTRE.Phoenix.Watchdog.Feed();
                }
                Hardware._display.updateEnabled(enabled);

                // read gamepad data all at once
                Hardware._gamepad.GetAllValues(ref gv);

                // drive the robot with F710 (left side[1] forward, reverse, right side[2] side to side)
                drive.arcadeDrive(gv.axes[1], gv.axes[2], false);

                // work the lid based on F710 POV
                int pov = gv.pov;
                if ((pov & 0x1) != 0)
                {
                    // POV UP
                    Hardware._display.updateLidStatus("Up");
                    lid.lidUp();
                } else if ((pov & 0x2) != 0)
                {
                    // POV DOWN
                    Hardware._display.updateLidStatus("Down");
                    lid.lidDown();
                } else
                {
                    Hardware._display.updateLidStatus("Stop");
                    lid.lidStop();
                }

                // look at BTN
                bool btn_a = (gv.btns & 0x1) != 0;
                if (btn_a && ! btn_a_was)
                {
                    Debug.Print ("btn_a hit");
                    if (!shooter.requestFill(0))
                    {
                        Debug.Print("tank 0 not available to fill");
                    }
                }
                btn_a_was = btn_a;

                bool btn_b = (gv.btns & 0x2) != 0;
                if (btn_b && ! btn_b_was)
                {
                    Debug.Print("btn_b hit");
                    if (!shooter.requestFill(1))
                    {
                        Debug.Print("tank 1 not available to fill");
                    }
                }
                btn_b_was = btn_b;

                //
                shooter.periodic(enabled);

                double v = Hardware.voltage.Read();
                Hardware._display.updateVoltage(v);

                bool limit = lid.isLidSwitchPressed();
                Hardware._display.updateLimitSwitch(limit);
                Hardware.lid_limit_switch_led.Write(limit);

                if (tick.hasPeriodPassed(5.0))
                {
                    Debug.Print("tick...");
                }

                /* wait 10 milliseconds and do it all over again */
                Thread.Sleep(10);
            }
        }
    }
}
