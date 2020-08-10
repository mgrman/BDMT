using System.Threading.Tasks;

namespace BDMT.Client
{
    public interface IModeManager
    {
        Task SwitchModeAsync(string mode);
    }
}