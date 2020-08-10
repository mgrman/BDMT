using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace BDMT.Shared
{
    [ServiceContract]
    public interface IModeInfoService
    {
       Task< IReadOnlyList<string>> GetAvailableModes();

        Task<string> GetModeWhichShouldBeActive();
    }
}