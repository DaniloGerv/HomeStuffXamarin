using System;
using System.Collections.Generic;
using System.ComponentModel;
using homestuff.entities;

namespace homestuff.viewModels
{
    public class FamilyTabPageViewModel
    {

        public User user { get; set; }
        public List<UsersForDomesticas> usersForDomesticas { get; set; }
        public Family family  { get; set; }
        public List<User> members { get; set; }

        public FamilyTabPageViewModel()
        {
            this.user = new User();
            this.family = new Family();
            this.usersForDomesticas = new List<UsersForDomesticas>();
            this.members = new List<User>();
        }

        public void addUsersForDomesticas(UsersForDomesticas ud)
        {
            if (this.usersForDomesticas == null)
                this.usersForDomesticas = new List<UsersForDomesticas>();
            ((List<UsersForDomesticas>)this.usersForDomesticas).Add(ud);
        }

        public void removeUsersForDomesticas(UsersForDomesticas ud)
        {
            if (this.usersForDomesticas != null)
                ((List<UsersForDomesticas>)this.usersForDomesticas).Remove(ud);
        }
    }

}
