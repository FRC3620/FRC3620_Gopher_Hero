using System;
using Microsoft.SPOT;
using CTRE.Phoenix.MotorControl.CAN;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix;
using Microsoft.SPOT.Hardware;

namespace FRC3620_Gopher_Hero
{
    public static class Hardware
    {
        /* Create our drivetrain in here */
        public static BaseMotorController _rightDrive;
        public static BaseMotorController _leftDrive;

        public static BaseMotorController _lid;

        public static OutputPort _testOutputPort;

        public static OutputPort b1_supply, b1_tank, b1_shot;
        public static OutputPort b2_supply, b2_tank, b2_shot;
        public static AnalogInput b1_pressure_sensor;
        public static AnalogInput b2_pressure_sensor;

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

            // on port 3: Hardware Pin 1 is CTRE.HERO.IO.Port3.Pin3.
            //            Hardware Pin 6 is CTRE.HERO.IO.Port3.Pin8.
            _testOutputPort = new OutputPort(CTRE.HERO.IO.Port5.Pin8, false);

            b1_supply = new OutputPort(CTRE.HERO.IO.Port3.Pin3, false);
            b1_tank = new OutputPort(CTRE.HERO.IO.Port3.Pin4, false);
            b1_shot = new OutputPort(CTRE.HERO.IO.Port3.Pin5, false);
            b2_supply = new OutputPort(CTRE.HERO.IO.Port3.Pin6, false);
            b2_tank = new OutputPort(CTRE.HERO.IO.Port3.Pin7, false);
            b2_shot = new OutputPort(CTRE.HERO.IO.Port3.Pin8, false);

            UsbHostDevice usb = CTRE.Phoenix.UsbHostDevice.GetInstance();
            _gamepad = new GameController(usb);
            usb.SetSelectableXInputFilter(UsbHostDevice.SelectableXInputFilter.XInputDevices);
        }
    }
}
