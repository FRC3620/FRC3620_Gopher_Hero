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

                // drive the robot
                drive.arcadeDrive(gv.axes[1], gv.axes[0], false);

                /* wait 10 milliseconds and do it all over again */
                Thread.Sleep(10);
            }
        }
    }
}
