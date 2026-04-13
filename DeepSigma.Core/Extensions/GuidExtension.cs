namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extension methods for the <see cref="Guid"/> struct.
/// </summary>
public static class GuidExtension
{
    extension(Guid guid)
    {
        /// <summary>
        /// Generates a new time-ordered GUID (Version 7 UUID).
        /// </summary>
        /// <remarks>
        /// This method should be used when you need a GUID that reflects the time of its creation, which can be beneficial for database indexing and sorting.
        /// </remarks>
        /// <returns></returns>
        public static Guid NewGuidTimeOrdered() => Guid.CreateVersion7();

        /// <inheritdoc cref="NewGuidTimeOrdered()"/>
        public static Guid NewGuidTimeOrdered(DateTimeOffset date_time_offset) 
            => Guid.CreateVersion7(date_time_offset);
    }
}
