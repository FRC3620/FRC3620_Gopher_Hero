using System;
using Microsoft.SPOT;

namespace FRC3620_Gopher_Hero
{
    public class Timer
    {
        private double m_startTime;
        private double m_accumulatedTime;
        private bool m_running;

        /** Lock for synchronization. */
        private object m_lock = new object();

        public Timer() {
            reset();
        }

        private double getMsClock()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        /**
         * Get the current time from the timer. If the clock is running it is derived from the current
         * system clock the start time stored in the timer class. If the clock is not running, then return
         * the time when it was last stopped.
         *
         * @return Current time value for this timer in seconds
         */
        public double get()
        {
            lock(m_lock) {
                if (m_running)
                {
                    return m_accumulatedTime + (getMsClock() - m_startTime) / 1000.0;
                }
                else
                {
                    return m_accumulatedTime;
                }
            }
        }

        /**
         * Reset the timer by setting the time to 0. Make the timer startTime the current time so new
         * requests will be relative now
         */
        public void reset()
        {
            lock(m_lock) {
                m_accumulatedTime = 0;
                m_startTime = getMsClock();
            }
        }

        /**
         * Start the timer running. Just set the running flag to true indicating that all time requests
         * should be relative to the system clock. Note that this method is a no-op if the timer is
         * already running.
         */
        public void start()
        {
            lock(m_lock) {
                if (!m_running)
                {
                    m_startTime = getMsClock();
                    m_running = true;
                }
            }
        }

        /**
         * Stop the timer. This computes the time as of now and clears the running flag, causing all
         * subsequent time requests to be read from the accumulated time rather than looking at the system
         * clock.
         */
        public void stop()
        {
            lock(m_lock) {
                m_accumulatedTime = get();
                m_running = false;
            }
        }

        /**
         * Check if the period specified has passed.
         *
         * @param seconds The period to check.
         * @return Whether the period has passed.
         */
        public bool hasElapsed(double seconds)
        {
            lock(m_lock) {
                return get() > seconds;
            }
        }

        /**
         * Check if the period specified has passed and if it has, advance the start time by that period.
         * This is useful to decide if it's time to do periodic work without drifting later by the time it
         * took to get around to checking.
         *
         * @param period The period to check for (in seconds).
         * @return Whether the period has passed.
         */
        public bool hasPeriodPassed(double period)
        {
            return advanceIfElapsed(period);
        }

        /**
         * Check if the period specified has passed and if it has, advance the start time by that period.
         * This is useful to decide if it's time to do periodic work without drifting later by the time it
         * took to get around to checking.
         *
         * @param seconds The period to check.
         * @return Whether the period has passed.
         */
        public bool advanceIfElapsed(double seconds)
        {
            lock(m_lock) {
                if (get() > seconds)
                {
                    // Advance the start time by the period.
                    // Don't set it to the current time... we want to avoid drift.
                    m_startTime += seconds * 1000;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
