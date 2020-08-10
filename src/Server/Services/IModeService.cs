using BDMT.Shared;
using System.Threading.Tasks;

namespace BDMT.Server
{
    public interface IModeService
    {
        Task<string> GetViewAsync();

        Task SwitchModeAsync(string mode);
    }
}