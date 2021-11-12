using System;

namespace TweetSpot.Exceptions
{
    /// <summary>
    /// Raised when a value essential to proper operation of an application has not been configured.
    /// </summary>
    [Serializable]
    public class EnvironmentConfigurationException : ApplicationException
    {

        public EnvironmentConfigurationException(string environmentVariableName, string description)
            : base($"Environment variable '{environmentVariableName}' not set.  {description}.")
        {
            EnvironmentVariableName = environmentVariableName;
        }

        /// <summary>
        /// The name of the environment variable that was unavailable
        /// </summary>
        public string EnvironmentVariableName { get; }

    }
}