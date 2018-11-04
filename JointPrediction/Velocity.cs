using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOwlConsulting.JointPrediction
{
    /// <summary>
    /// This class is responsible for storing rates of movement on X Y Z coordinates, as well as the date time of calculation.
    /// </summary>
    public class Velocity
    {
        /// <summary>
        /// Horizontal component.
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Vertical component.
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Depth component
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Timestamp of the calculated velocity
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Used for debugging
        /// </summary>
        /// <returns>String of X Y Z datetime</returns>
        public override string ToString()
        {
            return String.Format("{0:0.000} {1:0.000} {2:0.000} {3}", X, Y, Z, DateTime);
        }
    }
}
