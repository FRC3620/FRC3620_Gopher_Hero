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

        public static TalonSRX _lid;

        public static OutputPort _testOutputPort;

        public static OutputPort b0_supply, b0_tank, b0_shot;
        public static OutputPort b1_supply, b1_tank, b1_shot;
        public static AnalogInput b0_pressure_sensor;
        public static AnalogInput b1_pressure_sensor;
        public static AnalogInput voltage;

        public static InputPort lid_limit_switch;
        public static OutputPort lid_limit_switch_led;

        /* Gamepad */
        public static GameController _gamepad;

        /* Display */
        public static Display _display;

        static Hardware()
        {
            _leftDrive = new VictorSPX(1);
            _rightDrive = new VictorSPX(2);
            _lid = new TalonSRX(9);

            _leftDrive.ConfigFactoryDefault();
            _rightDrive.ConfigFactoryDefault();
            _lid.ConfigFactoryDefault();

            _lid.ConfigForwardLimitSwitchSource(CTRE.Phoenix.MotorControl.LimitSwitchSource.FeedbackConnector, CTRE.Phoenix.MotorControl.LimitSwitchNormal.NormallyOpen);
            _lid.ConfigReverseLimitSwitchSource(CTRE.Phoenix.MotorControl.LimitSwitchSource.FeedbackConnector, CTRE.Phoenix.MotorControl.LimitSwitchNormal.NormallyOpen);
            _lid.ConfigContinuousCurrentLimit(2);

            // on port 3: Hardware Pin 1 is CTRE.HERO.IO.Port3.Pin3.
            //            Hardware Pin 6 is CTRE.HERO.IO.Port3.Pin8.
            _testOutputPort = new OutputPort(CTRE.HERO.IO.Port5.Pin8, false);

            b0_supply = new OutputPort(CTRE.HERO.IO.Port3.Pin3, false);
            b0_tank = new OutputPort(CTRE.HERO.IO.Port3.Pin4, false);
            b0_shot = new OutputPort(CTRE.HERO.IO.Port3.Pin5, false);
            b1_supply = new OutputPort(CTRE.HERO.IO.Port3.Pin6, false);
            b1_tank = new OutputPort(CTRE.HERO.IO.Port3.Pin7, false);
            b1_shot = new OutputPort(CTRE.HERO.IO.Port3.Pin8, false);

            voltage = new AnalogInput(CTRE.HERO.IO.Port8.Analog_Pin5);

            lid_limit_switch = new InputPort(CTRE.HERO.IO.Port8.Pin6, false, Port.ResistorMode.PullUp);
            lid_limit_switch_led = new OutputPort(CTRE.HERO.IO.Port5.Pin3, false);

            UsbHostDevice usb = CTRE.Phoenix.UsbHostDevice.GetInstance();
            _gamepad = new GameController(usb);
            usb.SetSelectableXInputFilter(UsbHostDevice.SelectableXInputFilter.XInputDevices);

            _display = new Display();
        }
    }
}
