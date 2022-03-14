namespace UXF
{
    /// <summary>
    /// Represents a unit of an experiment.
    /// </summary>
    public interface IExperimentUnit : ISettingsContainer
    {
        /// <summary>
        /// Sets wether data should be saved for this experiment unit.
        /// </summary>
        bool saveData { get; }
    }
}