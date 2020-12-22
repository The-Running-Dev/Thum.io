using System.IO;
using System.Threading.Tasks;

namespace Thum.io.Screenshots
{
    public interface IScreenshotService
    {
        Task<MemoryStream> ToMemory(string url);
        
        Task ToDisk(string url, string path);
    }
}