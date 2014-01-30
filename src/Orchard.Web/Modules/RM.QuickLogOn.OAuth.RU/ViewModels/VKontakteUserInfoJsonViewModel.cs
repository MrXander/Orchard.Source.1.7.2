using System.Runtime.Serialization;

namespace RM.QuickLogOn.OAuth.RU.ViewModels
{
    [DataContract]
    public class VKontakteUserInfoJsonViewModel
    {
        public class UserInfo
        {
            [DataMember]
            public string uid { get; set; }

            [DataMember]
            public string first_name { get; set; }
            
            [DataMember]
            public string last_name { get; set; }
            
            [DataMember]
            public string photo_max_orig { get; set; }
        }

        [DataMember]
        public UserInfo[] response { get; set; }
    }
}
