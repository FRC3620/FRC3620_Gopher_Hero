using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace FRC3620_Gopher_Hero
{
    public class Shooter
    {
        static int NUMBER_OF_BARRELS = 2;

        AirMaster fillMaster = new AirMaster();
        BarrelPrototype[] barrels = new BarrelPrototype[NUMBER_OF_BARRELS];

        public Shooter()
        {
            barrels[0] = new BarrelPrototype(Hardware.b1_supply, Hardware.b1_tank, Hardware.b1_shot,
                Hardware.b1_pressure_sensor, fillMaster, "b1");
            if (NUMBER_OF_BARRELS > 1)
            {
                barrels[1] = new BarrelPrototype(Hardware.b2_supply, Hardware.b2_tank, Hardware.b2_shot,
                    Hardware.b2_pressure_sensor, fillMaster, "b2");
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
        BarrelPrototype whoIsUsingIt = null;

        public bool reserve(BarrelPrototype b)
        {
            if (whoIsUsingIt == null)
            {
                whoIsUsingIt = b;
                Debug.Print (b.getBarrelName() + " reserved the air supply");
                return true;
            }
            else
            {
                return false;
            }
        }
        public void free(BarrelPrototype b)
        {
            whoIsUsingIt = null;
            Debug.Print (b.getBarrelName() + " freed the air supply");
        }

    }

    class BarrelPrototype
    {
        bool fillRequested = false;
        bool shotRequested = false;
        internal bool readyToFill = false;
        internal bool readyToShoot = false;

        BarrelState idleState, waitFillState, seatingState, fillingState, waitFireState, preFireState, firingState;

        BarrelState currentBarrelState;

        public BarrelPrototype(OutputPort supply, OutputPort tank, OutputPort shot, AnalogInput pressure, AirMaster fillMaster, String barrelName)
        {
            this.supplyValve = supply;
            this.tankValve = tank;
            this.shotValve = shot;
            this.pressureSensor = pressure;
            this.fillMaster = fillMaster;
            this.barrelName = barrelName;

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

        OutputPort supplyValve, tankValve, shotValve;
        AnalogInput pressureSensor;
        AirMaster fillMaster;
        string barrelName;

        public string getBarrelName()
        {
            return barrelName;
        }

        public bool isReadyToFill()
        {
            return readyToFill;
        }

        public void startup()
        {
            currentBarrelState = idleState;
            //logger.info("{}: moving to state {}", barrelName, currentBarrelState.getStateName());
            Debug.Print(barrelName + ": moving to state " + currentBarrelState.getStateName());
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
                Debug.Print(barrelName + ": moving to state " + newState.getStateName() + " from " + currentBarrelState.getStateName());
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
            public void logAndBeginState(BarrelPrototype barrel)
            {
                // SmartDashboard.putString(barrelName + " status", getStateName());
                beginState(barrel);
            }

            public virtual void beginState(BarrelPrototype barrel)
            {
            }

            abstract public BarrelState running(BarrelPrototype barrel);

            public virtual void endState(BarrelPrototype barrel)
            {
            }

            String stateName = null;
            public String getStateName()
            {
                if (stateName == null)
                {
                    stateName = this.GetType().Name;
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
            override public void beginState(BarrelPrototype barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                barrel.fillRequested = false;
                barrel.readyToFill = true;
            }

            override public BarrelState running(BarrelPrototype barrel)
            {
                if (barrel.fillRequested)
                {
                    return barrel.waitFillState;
                }
                return null;
            }

            override public void endState(BarrelPrototype barrel)
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
            override public void beginState(BarrelPrototype barrel)
            {
                // probably redundant
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);
            }

            override public BarrelState running(BarrelPrototype barrel)
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

            override public void endState(BarrelPrototype barrel)
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

            override public void beginState(BarrelPrototype barrel)
            {
                barrel.openValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                timer.reset();
                timer.start();
            }

            override public BarrelState running(BarrelPrototype barrel)
            {
                if (timer.hasPeriodPassed(0.50))
                {
                    return barrel.fillingState;
                }
                return null;
            }

            override public void endState(BarrelPrototype barrel)
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

            override public void beginState(BarrelPrototype barrel)
            {
                barrel.openValve(barrel.supplyValve);
                barrel.openValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                barrel.readyToShoot = false;

                timer.reset();
                timer.start();
            }

            override public BarrelState running(BarrelPrototype barrel)
            {
                if (timer.hasPeriodPassed(3.0))
                {
                    return barrel.waitFireState;
                }
                /*
                 * if (pressureSensor.getVoltage() > 2.5) { return waitFireState; }
                 */
                return null;
            }

            override public void endState(BarrelPrototype barrel)
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
            override public void beginState(BarrelPrototype barrel)
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

            override public BarrelState running(BarrelPrototype barrel)
            {
                if (timer.hasPeriodPassed(0.5))
                {
                    return barrel.preFireState;
                }
                return null;
            }

            override public void endState(BarrelPrototype barrel)
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

            override public void beginState(BarrelPrototype barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);

                timer.reset();
                timer.start();
            }

            override public BarrelState running(BarrelPrototype barrel)
            {
                if (timer.hasPeriodPassed(0.5))
                {
                    return barrel.firingState;
                }
                return null;
            }

            override public void endState(BarrelPrototype barrel)
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

            override public void beginState(BarrelPrototype barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.openValve(barrel.shotValve);

                timer.reset();
                timer.start();
            }

            override public BarrelState running(BarrelPrototype barrel)
            {
                if (timer.hasPeriodPassed(1.5))
                {
                    return barrel.idleState;
                }
                return null;
            }

            override public void endState(BarrelPrototype barrel)
            {
                barrel.closeValve(barrel.supplyValve);
                barrel.closeValve(barrel.tankValve);
                barrel.closeValve(barrel.shotValve);
            }
        }
    }
}
