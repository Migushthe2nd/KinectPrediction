using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOwlConsulting.JointPrediction
{
    public class Observation
    {
        /// <summary>
        /// Horizontal component
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Vertical component.
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Depth component.
        /// </summary>
        public double Z { get; set; }
        /// <summary>
        /// Time of the observation
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Time elapsing since some past observation, in seconds
        /// </summary>
        /// <param name="pastObservation">A observation occurring in the past. Future observations will return negative numbers.</param>
        /// <returns>Elapsed time</returns>
        public double TimeElapsed(Observation pastObservation)
        {
            return (DateTime - pastObservation.DateTime).TotalSeconds;
        }

        /// <summary>
        /// Computes the approximate/average velocity between the past observation and the current observation.
        /// </summary>
        /// <param name="pastObservation"></param>
        /// <returns></returns>
        public Velocity ApproximateVelocity(Observation pastObservation)
        {
            double timeElapsed = TimeElapsed(pastObservation);
            return new Velocity { 
                X = (X - pastObservation.X) / timeElapsed, 
                Y = (Y - pastObservation.Y) / timeElapsed,
                Z = (Z - pastObservation.Z) / timeElapsed,
                DateTime = this.DateTime };
        }

        /// <summary>
        /// Used for debugging.
        /// </summary>
        /// <returns>A string printing the object</returns>
        public override string ToString()
        {
            return String.Format("{0:0.000} {1:0.000} {2:0.000} {3}", X, Y, Z, DateTime);
        }
    }
}
