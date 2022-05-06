using System;
using CTRE.Phoenix.MotorControl.CAN;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;


namespace FRC3620_Gopher_Hero
{
    public class Lid
    {
        BaseMotorController m_lidMotor;
        InputPort m_lidSwitch;
        OutputPort m_lidSwitchled;

        public Lid (BaseMotorController lidMotor, InputPort lidSwitch, OutputPort lidSwitchled)
        {
            m_lidMotor = lidMotor;
            m_lidSwitch = lidSwitch;
            m_lidSwitchled = lidSwitchled;
        }

        public void lidUp()
        {
            m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0.2);
        }

        public void lidDown()
        {
            Boolean b = m_lidSwitch.Read();
            m_lidSwitchled.Write(b);
            if (b)
            {
                m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -0.2);
            } else
            {
                lidStop();
            }
        }
        public void lidStop()
        {
            m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0.0);
        }
    }
}
