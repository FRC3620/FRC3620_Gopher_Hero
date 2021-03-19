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
            Lid lid = new Lid(Hardware._lid);
            CTRE.Phoenix.Controller.GameControllerValues gv = new CTRE.Phoenix.Controller.GameControllerValues();

            while (true)
            {
                // only run motors if Gamepad is attached
                if (Hardware._gamepad.GetConnectionStatus() == UsbDeviceConnection.Connected)
                {
                    CTRE.Phoenix.Watchdog.Feed();
                }

                // read gamepad data all at once
                Hardware._gamepad.GetAllValues(ref gv);

                // drive the robot with F710 right side joystick
                drive.arcadeDrive(gv.axes[3], gv.axes[2], false);

                // work the lid based on F710 POV
                int pov = gv.pov;
                if ((pov & 0x1) != 0)
                {
                    // POV UP
                    lid.lidUp();
                } else if ((pov & 0x2) != 0)
                {
                    // POV DOWN
                    lid.lidDown();
                } else
                {
                    lid.lidStop();
                }

                // look at BTN
                Boolean btn_a = (gv.btns & 0x1) != 0;
                Debug.Print("Btn_A: " + btn_a);
                Hardware._testOutputPort.Write(btn_a);

                /* wait 10 milliseconds and do it all over again */
                Thread.Sleep(10);
            }
        }
    }
}
