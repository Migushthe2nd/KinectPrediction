using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOwlConsulting.JointPrediction
{
    /// <summary>
    /// A class which is responsible for predicting future movement based on past movement using an online algorithm
    /// for calculating exponentially weighted moving averages of velocity along three dimensions in space.
    /// </summary>
    public class JointPredictor
    {
        /// <summary>
        /// Stores the last observation to be ingested by the online algorithm
        /// </summary>
        Observation lastObservation;
        /// <summary>
        /// Stores the current value of the exponentially weighted moving average velocity
        /// </summary>
        public Velocity EwmaVelocity { get; private set; }
        /// <summary>
        /// Stores the decay constant to be used by the algorithm. Larger (+) values mean that past observations have much
        /// less sway over the current EwmaVelocity value. Values close to zero mean that past observations will have a
        /// lingering effect.
        /// </summary>
        public Double DecayConstant { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="decay">Value of the decay constant</param>
        public JointPredictor(double decay)
        {
            DecayConstant = decay;
        }

        /// <summary>
        /// Update ingests a new observation an is responsible for calculating, online, the update to the EwmaVelocity
        /// based on the new observation.
        /// </summary>
        /// <param name="newObservation">The new observation.</param>
        public void Update(Observation newObservation)
        {
            if (lastObservation == null)
            {
                lastObservation = newObservation;
                EwmaVelocity = new Velocity { X = 0, Y = 0, Z = 0, DateTime = lastObservation.DateTime };
            }
            else
            {
                Velocity currentVelocity = newObservation.ApproximateVelocity(lastObservation);
                double timeElapsed = newObservation.TimeElapsed(lastObservation);
                double decay = Math.Exp(-1 * DecayConstant * timeElapsed);
                EwmaVelocity.X = (1 - decay) * EwmaVelocity.X + decay * currentVelocity.X;
                EwmaVelocity.Y = (1 - decay) * EwmaVelocity.Y + decay * currentVelocity.Y;
                EwmaVelocity.Z = (1 - decay) * EwmaVelocity.Z + decay * currentVelocity.Z;
                EwmaVelocity.DateTime = currentVelocity.DateTime;
                lastObservation = newObservation;
            }
        }

    }
}
