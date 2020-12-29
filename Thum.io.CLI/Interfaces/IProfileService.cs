using System.Threading.Tasks;

using Thum.io.CLI.Models;

namespace Thum.io.CLI.Interfaces
{
    public interface IProfileService
    {
        /// <summary>
        /// Gets the profile from the INI configuration
        /// </summary>
        /// <returns></returns>
        Profile Get();

        /// <summary>
        /// Creates a new profile with the given API key
        /// </summary>
        /// <param name="apiKey">The API key</param>
        /// <param name="profile">The name of the profile</param>
        /// <returns></returns>
        Task Create(string apiKey, string profile = "default");

        /// <summary>
        /// Updates an existing profile
        /// </summary>
        /// <param name="profile">The profile to update</param>
        void Update(Profile profile);
    }
}