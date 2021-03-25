using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace FRC3620_Gopher_Hero
{
    public class Shooter
    {
        public const int NUMBER_OF_BARRELS = 2;

        AirMaster fillMaster = new AirMaster();
        Barrel[] barrels = new Barrel[NUMBER_OF_BARRELS];

        public Shooter()
        {
            barrels[0] = new Barrel(Hardware.b0_supply, Hardware.b0_tank, Hardware.b0_shot,
                Hardware.b0_pressure_sensor, fillMaster, 0);
            if (NUMBER_OF_BARRELS > 1)
            {
                barrels[1] = new Barrel(Hardware.b1_supply, Hardware.b1_tank, Hardware.b1_shot,
                    Hardware.b1_pressure_sensor, fillMaster, 1);
            }
        }

        public void startup()
        {
            for (int i = 0; i < NUMBER_OF_BARRELS; i++)
            {
                barrels[i].startup();
            }
        }

        public void init()
        {
            for (int i = 0; i < NUMBER_OF_BARRELS; i++)
            {
                barrels[i].clearRequests();
            }
        }

        public void periodic(bool enabled)
        {
            for (int i = 0; i < NUMBER_OF_BARRELS; i++)
            {
                barrels[i].makeTheBarrelWork();
            }

            /*
             * work the compressor
             */

            /*
            if (currentMode == RobotMode.TELEOP || currentMode == RobotMode.AUTONOMOUS)
            {
                setRelayValue(getCompressorSwitch() ? Relay.Value.kOn : Relay.Value.kOff);
            }
            else if (currentMode == RobotMode.TEST)
            {
                // let the test dashboard
            }
            else
            {
                setRelayValue(Relay.Value.kOff);
            }
            */
        }

        public bool requestFill(int i)
        {
            if (i <= NUMBER_OF_BARRELS && barrels[i].readyToFill)
            {
                barrels[i].requestFill();
                return true;
            }
            return false;
        }

        public bool requestShot(int i)
        {
            if (i <= NUMBER_OF_BARRELS && barrels[i].readyToShoot)
            {
                barrels[i].requestShot();
                return true;
            }
            return false;
        }

    }

    class AirMaster
    {
        Barrel whoIsUsingIt = null;

        public bool reserve(Barrel b)
        {
            if (whoIsUsingIt == null)
            {
                whoIsUsingIt = b;
                Debug.Print ("b" + b.getBarrelNumber() + " reserved the air supply");
                return true;
            }
            else
            {
                return false;
            }
        }
        public void free(Barrel b)
        {
            whoIsUsingIt = null;
            Debug.Print ("b" + b.getBarrelNumber() + " freed the air supply");
        }

    }

    class Barrel
    {
        bool fillRequested = false;
        bool shotRequested = false;
        internal bool readyToFill = false;
        internal bool readyToShoot = false;

        BarrelState idleState, waitFillState, seatingState, fillingState, waitFireState, preFireState, firingState;

        BarrelState currentBarrelState;

        OutputPort supplyValve, tankValve, shotValve;
        AnalogInput pressureSensor;
        AirMaster fillMaster;
        int barrelNumber;

        public Barrel(OutputPort supply, OutputPort tank, OutputPort shot, AnalogInput pressure, AirMaster fillMaster, int barrelNumber)
        {
            this.supplyValve = supply;
            this.tankValve = tank;
            this.shotValve = shot;
            this.pressureSensor = pressure;
            this.fillMaster = fillMaster;
            this.barrelNumber = barrelNumber;

            closeValve(supplyValve);
            closeValve(tankValve);
            closeValve(shotValve);

            idleState = new IdleState();
            waitFillState = new WaitFillState();
            seatingState = new SeatingState();
            fillingState = new FillingState();
            waitFireState = new WaitFireState();
            preFireState = new PreFireState();
            firingState = new FiringState();
        }

        internal int getBarrelNumber()
        {
            return barrelNumber;
        }

        public bool isReadyToFill()
        {
            return readyToFill;
        }

        public void startup()
        {
            currentBarrelState = idleState;
            //logger.info("{}: moving to state {}", barrelName, currentBarrelState.getStateName());
            Debug.Print("b" + barrelNumber + ": moving to state " + currentBarrelState.getStateName());
            currentBarrelState.logAndBeginState(this);
        }

        void openValve(OutputPort v)
        {
            v.Write(true);
        }

        void closeValve(OutputPort v)
        {
            v.Write(false);
        }

        public void makeTheBarrelWork()
        {
            BarrelState newState = currentBarrelState.running(this);
            if (newState != null && newState != currentBarrelState)
            {
                //logger.info("{}: moving to state {} from {}", barrelName, newState.getStateName(), currentBarrelState.getStateName());
                Debug.Print("b" + barrelNumber + ": moving to state " + newState.getStateName() + " from " + currentBarrelState.getStateName());
                currentBarrelState.endState(this);
                newState.logAndBeginState(this);
                currentBarrelState = newState;
            }
        }

        public void requestFill()
        {
            fillRequested = true;
        }

        public void requestShot()
        {
            shotRequested = true;
        }

        public void clearRequests()
        {
            shotRequested = false;
            fillRequested = false;
        }

        abstract class BarrelState
        {
            public void logAndBeginState(Barrel barrel)
            {
                // SmartDashboard.putString(barrelName + " status", getStateName());
                Hardware._display.updateBarrelStatus(barrel.barrelNumber, getStateName());
                beginState(barrel);
            }

            public virtual void beginState(Barrel barrel)
            {
            }

            abstract public BarrelState running(Barrel barrel);

            public virtual void endState(Barrel barrel)
            {
            }

            String stateName = null;
            public String getStateName()
            {
                if (stateName == null)
                {
                    stateName = this.GetType().Name;
                    stateName = stateName.Substring(0, stateName.Length - 5);
                }
                return stateName;
            }
        }


        /*
        * The state 'Idle': empty tank, waiting for fill to be requested.
        * 
        * Entry conditions are either entering for the first time or after the Firing state.
        * 
        * No code is
        * run by the running() method except for possibly State related jargon.
        * 
        * Exit conditions are either automatic exit to the 'Waiting to Fill' state
        * or user input.
        */

        class IdleState : BarrelState
        {
            override public void beginState(Barrel barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                barrel.fillRequested = false;
                barrel.readyToFill = true;
            }

            override public BarrelState running(Barrel barrel)
            {
                if (barrel.fillRequested)
                {
                    return barrel.waitFillState;
                }
                return null;
            }

            override public void endState(Barrel barrel)
            {
                barrel.readyToFill = false;
            }
        }

        /*
         * The state 'WaitFill' waits for the shared air components tpo become 
         * available.
         * 
         * Entry conditions is either automatic from 'Idle' or accomplished with user input.
         * 
         * The running() method will verify that no Barrel is currently filling.
         * 
         * Exit condition is that no other Barrel is filling.
         */
        class WaitFillState : BarrelState
        {
            override public void beginState(Barrel barrel)
            {
                // probably redundant
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);
            }

            override public BarrelState running(Barrel barrel)
            {
                if (barrel.fillMaster.reserve(barrel))
                {
                    return barrel.seatingState;
                }
                else
                {
                    return null;
                }
            }

            override public void endState(Barrel barrel)
            {
            }
        }

        /*
         * The state 'SeatingValve' runs code to seat the valve before Filling.
         * 
         * The running() method runs code to seat the
         * piston for a Barrel.
         * 
         * Exit condition is automatic exit to the
         * 'Filling' state once the valve is seated.
         */
        class SeatingState : BarrelState
        {
            Timer timer = new Timer();

            override public void beginState(Barrel barrel)
            {
                barrel.openValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                timer.reset();
                timer.start();
            }

            override public BarrelState running(Barrel barrel)
            {
                if (timer.hasPeriodPassed(0.50))
                {
                    return barrel.fillingState;
                }
                return null;
            }

            override public void endState(Barrel barrel)
            {
                barrel.closeValve(barrel.supplyValve);
            }
        }

        /*
         * The state 'Filling': we are filling the tank.
         * 
         * The running() method runs code to fill the tank.
         * 
         * We exit to 'WaitFire' state once filling is complete.
         */
        class FillingState : BarrelState
        {
            Timer timer = new Timer();

            override public void beginState(Barrel barrel)
            {
                barrel.openValve(barrel.supplyValve);
                barrel.openValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                barrel.readyToShoot = false;

                timer.reset();
                timer.start();
            }

            override public BarrelState running(Barrel barrel)
            {
                if (timer.hasPeriodPassed(3.0))
                {
                    return barrel.waitFireState;
                }
                double t = timer.get();
                Hardware._display.updateBarrelPSI(barrel.barrelNumber, t.ToString("F1"));
                /*
                 * if (pressureSensor.getVoltage() > 2.5) { return waitFireState; }
                 */
                return null;
            }

            override public void endState(Barrel barrel)
            {
                {
                    barrel.readyToShoot = true;

                    barrel.closeValve(barrel.supplyValve);
                    barrel.closeValve(barrel.tankValve);
                    barrel.closeValve(barrel.shotValve);

                    barrel.fillMaster.free(barrel);
                }
            }
        }

        /*
        * The state 'WaitFire', is the fourth state to be run. Entry condition is
        * that the Filling state is complete. The running() method will run code
        * that waits for user input. Exit condition is specific user input.
        */

        class WaitFireState : BarrelState
        {
            Timer timer = new Timer();
            override public void beginState(Barrel barrel)
            {
                // keep the tank valve open so the piston does not move
                barrel.closeValve(barrel.supplyValve);
                barrel.openValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                barrel.shotRequested = false;
                barrel.readyToShoot = true;

                timer.reset();
                timer.start();
            }

            override public BarrelState running(Barrel barrel)
            {
                if (timer.hasPeriodPassed(0.5))
                {
                    return barrel.preFireState;
                }
                return null;
            }

            override public void endState(Barrel barrel)
            {
                barrel.readyToShoot = false;

                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);
            }
        }

        class PreFireState : BarrelState
        {
            Timer timer = new Timer();

            override public void beginState(Barrel barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                timer.reset();
                timer.start();
            }

            override public BarrelState running(Barrel barrel)
            {
                if (timer.hasPeriodPassed(0.5))
                {
                    return barrel.firingState;
                }
                return null;
            }

            override public void endState(Barrel barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);
            }
        }

        /*
         * The state 'Firing', is the last state to be run. Entry condition is exit
         * from the 'WaitFire' state The running() method runs code to fire the
         * specified Barrel Exit condition is automatic exit to the 'Idle' state
         * once firing is complete.
         */
        class FiringState : BarrelState
        {
            Timer timer = new Timer();

            override public void beginState(Barrel barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.openValve(barrel.shotValve);

                timer.reset();
                timer.start();
            }

            override public BarrelState running(Barrel barrel)
            {
                if (timer.hasPeriodPassed(1.5))
                {
                    return barrel.idleState;
                }
                return null;
            }

            override public void endState(Barrel barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);
            }
        }
    }
}
