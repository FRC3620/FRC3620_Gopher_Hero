using System;
using Microsoft.SPOT;
using CTRE.Phoenix.MotorControl.CAN;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix;

namespace FRC3620_Gopher_Hero
{
    public static class Hardware
    {
        /* Create our drivetrain in here */
        public static BaseMotorController _rightDrive;
        public static BaseMotorController _leftDrive;

        public static BaseMotorController _lid;

        /* Gamepad */
        public static GameController _gamepad;

        static Hardware()
        {
            _leftDrive = new VictorSPX(1);
            _rightDrive = new VictorSPX(2);
            _lid = new VictorSPX(9);

            _leftDrive.ConfigFactoryDefault();
            _rightDrive.ConfigFactoryDefault();
            _lid.ConfigFactoryDefault();

            UsbHostDevice usb = CTRE.Phoenix.UsbHostDevice.GetInstance();
            _gamepad = new GameController(usb);
            usb.SetSelectableXInputFilter(UsbHostDevice.SelectableXInputFilter.XInputDevices);
        }
    }
}
