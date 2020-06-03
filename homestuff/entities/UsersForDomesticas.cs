using System;
namespace homestuff.entities
{
    public class UsersForDomesticas
    {
        
            public Domesticas domesticas { get; set; }
        public User user { get; set; }

        public UsersForDomesticas(Domesticas domesticas,User user)
        {
            this.domesticas = domesticas;
            this.user = user;
        }

        public UsersForDomesticas() { }
    
    }
}

