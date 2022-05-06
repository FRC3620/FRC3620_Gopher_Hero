using CTRE.Phoenix.MotorControl.CAN;
using Microsoft.SPOT;

namespace FRC3620_Gopher_Hero
{
    public class DifferentialDrive
    {
        BaseMotorController m_leftMotor, m_rightMotor;
        public DifferentialDrive (BaseMotorController left, BaseMotorController right)
        {
            this.m_leftMotor = left;
            this.m_rightMotor = right;
        }

        private double m_leftSideInvertMultiplier = -1.0;
        public static double kDefaultDeadband = 0.02;
        public static double kDefaultMaxOutput = 1.0;

        protected double m_deadband = kDefaultDeadband;
        protected double m_maxOutput = kDefaultMaxOutput;

        public void arcadeDrive (double _xSpeed, double _zRotation, bool squareInputs) {

            double xSpeed = MathUtil.clamp(_xSpeed, -1.0, 1.0);
            xSpeed = MathUtil.applyDeadband(xSpeed, m_deadband);

            double zRotation = MathUtil.clamp(_zRotation, -1.0, 1.0);
            zRotation = MathUtil.applyDeadband(zRotation, m_deadband);

            // Square the inputs (while preserving the sign) to increase fine control
            // while permitting full power.
            if (squareInputs)
            {
                xSpeed = MathUtil.copySign(xSpeed * xSpeed, xSpeed);
                zRotation = MathUtil.copySign(zRotation * zRotation, zRotation);
            }

            // stolen from https://stemrobotics.cs.pdx.edu/node/4736
            // fixed for some sign mismatches

            double leftMotorOutput = -zRotation - xSpeed;
            double rightMotorOutput = -zRotation + xSpeed;

            double leftPower = MathUtil.clamp(leftMotorOutput, -1.0, 1.0) * m_leftSideInvertMultiplier;
            double rightPower = MathUtil.clamp(rightMotorOutput, -1.0, 1.0);
            m_leftMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, leftPower);
            m_rightMotor.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, rightPower);

            // Debug.Print("Joy: " + _xSpeed + " -> " + xSpeed + ", " + _zRotation + " -> " + zRotation + "; Motor: " + leftPower + ", " + rightPower);
        }

    }
}
