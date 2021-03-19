using System;
using CTRE.Phoenix.MotorControl.CAN;
using Microsoft.SPOT;

namespace FRC3620_Gopher_Hero
{
    public class Lid
    {
        BaseMotorController m_lidMotor;

        public Lid (BaseMotorController lidMotor)
        {
            m_lidMotor = lidMotor;
        }

        public void lidUp()
        {
            m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0.2);
        }

        public void lidDown()
        {
            m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -0.2);
        }
        public void lidStop()
        {
            m_lidMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0.0);
        }
    }
}
