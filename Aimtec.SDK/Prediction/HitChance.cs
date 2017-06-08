﻿namespace Aimtec.SDK.Prediction
{
    /// <summary>
    ///     The chance that the prediction will hit the target.
    /// </summary>
    public enum HitChance
    {
        /// <summary>
        ///     There is a collision.
        /// </summary>
        Collision = -1,

        /// <summary>
        ///     The target is not moving, hindering prediction.
        /// </summary>
        Low = 0,

        /// <summary>
        ///     The target has a medium chance of being hit.
        /// </summary>
        Medium = 1,

        /// <summary>
        ///     The target has a high chance of being hit.
        /// </summary>
        High = 2,

        /// <summary>
        ///     The target is slowed, and/or too close, allowing for an easy hit.
        /// </summary>
        Slowed = 3,

        /// <summary>
        ///     The target is immobile, guaranteeing a hit.
        /// </summary>
        Immobile = 4,

        /// <summary>
        ///     The target is dashing in a straight line, guaranteeing a hit.
        /// </summary>
        Dash = 5
    }
}