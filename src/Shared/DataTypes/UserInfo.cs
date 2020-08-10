using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BDMT.Client.Clientside
{
    [DataContract]
    public class UserInfo
    {
        [DataMember(Order = 1)]
        public bool IsAuthenticated { get; set; }

        [DataMember(Order = 2)]
        public IReadOnlyList<UserClaim> Claims { get; set; }
    }

    [DataContract]
    public class UserClaim
    {
        [DataMember(Order = 1)]
        public string Type { get; set; }

        [DataMember(Order = 2)]
        public string Value { get; set; }
    }
}