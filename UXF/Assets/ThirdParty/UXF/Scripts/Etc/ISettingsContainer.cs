namespace UXF
{
    /// <summary>
    /// Interface representing an object that can contain settings.
    /// </summary>
    public interface ISettingsContainer
    {
        /// <summary>
        /// A settings object.
        /// </summary>
        /// <value></value>
        Settings settings { get; }
    }
}