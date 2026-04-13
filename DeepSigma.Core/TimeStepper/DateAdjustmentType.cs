using System;
using System.Collections.Generic;
using System.Text;

namespace DeepSigma.Core.TimeStepper;

/// <summary>
/// Defines how to adjust dates that do not fall on valid time steps.
/// </summary>
public enum DateAdjustmentType
{
    /// <summary>
    /// Move the date forward in time to the next valid date.
    /// </summary>
    MoveForward,
    /// <summary>
    /// Move the date backward in time to the previous valid date.
    /// </summary>
    MoveBackward,
    /// <summary>
    /// Move the date in the direction of the time step (forward or backward).
    /// For example, if we are stepping forward in time, the date will be moved forward to the next valid date.
    /// If we are stepping backward in time, the date will be moved backward to the previous valid date.
    /// </summary>
    MoveInDirectionOfTimeStep
}
