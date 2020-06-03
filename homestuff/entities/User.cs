using System;
using Plugin.CloudFirestore.Attributes;

namespace homestuff.entities
{

    public class User
    {

        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string uid { get; set; }
        public Location place { get; set; }
        public string photoURL { get; set; }
        public string familyId { get; set; }
        public string DisplayName { get { return this.firstName + " " + this.lastName; } set { } }

        public User() {

        }
    }
}
