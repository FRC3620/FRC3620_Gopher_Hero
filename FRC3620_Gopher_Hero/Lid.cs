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

        public Lid (BaseMotorController lidMotor, InputPort lidSwitch)
        {
            m_lidMotor = lidMotor;
            m_lidSwitch = lidSwitch;
        }

        public void lidUp()
        {
            m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0.6);
        }

        public void lidDown()
        {
            if (!isLidSwitchPressed())
            {
                m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -0.6);
            } else
            {
                lidStop();
            }
        }
        public void lidStop()
        {
            m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0.0);
        }

        public Boolean isLidSwitchPressed()
        {
            return !m_lidSwitch.Read();
        }
    }
}
