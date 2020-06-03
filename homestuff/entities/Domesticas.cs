using System;
using Plugin.CloudFirestore.Attributes;

namespace homestuff.entities
{
    public class Domesticas
    {
        [MapTo("name")]
        public string name { get; set; }
        [MapTo("user")]
        public string user { get; set; }
        [MapTo("familyID")]
        public string familyID { get; set; }
        [MapTo("domesticaID")]
        public string domesticaID { get; set; }

        public Domesticas(string name, string user)
        {
            this.name = name;
            this.user = user;
        }

        public Domesticas()
        {

        }
    }
}
