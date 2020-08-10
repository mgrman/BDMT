using System.ServiceModel;
using System.Threading.Tasks;

namespace BDMT.Client.Clientside.Hosting
{
    [ServiceContract]
    public interface IUserInfoService
    {
        Task<UserInfo> GetAsync();
    }
}