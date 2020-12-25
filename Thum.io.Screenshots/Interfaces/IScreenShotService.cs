using System.IO;
using System.Threading.Tasks;

namespace Thum.io.Screenshots.Interfaces
{
    public interface IScreenShotService
    {
        /// <summary>
        /// Takes a screenshot and returns it as a memory stream
        /// </summary>
        /// <param name="url">The URL to take a screenshot of</param>
        /// <param name="options">The image modifier options</param>
        /// <returns></returns>
        Task<MemoryStream> ToMemory(string url, ImageModifierOptions options = null);

        /// <summary>
        /// Takes a screenshot and saves it to disk
        /// </summary>
        /// <param name="url">The URL to take a screenshot of</param>
        /// <param name="path">The file system path where to save the screenshot</param>
        /// <param name="options">The image modifier options</param>
        /// <returns></returns>
        Task ToDisk(string url, string path, ImageModifierOptions parameters = null);
    }
}