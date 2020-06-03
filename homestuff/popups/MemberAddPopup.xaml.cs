using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using homestuff.entities;
using homestuff.views;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace homestuff.popups
{
    public partial class MemberAddPopup : PopupPage
    {
        IEnumerable<User> possibleMembers;
        User selectedUser;
        public MemberAddPopup(IEnumerable<User> users)
        {
            possibleMembers = users;
            BindingContext = possibleMembers;
            InitializeComponent();
      
        }


        private async void LeaveClick(object sender,EventArgs e)
        {
            MessagingCenter.Send((App)Application.Current, "userData",selectedUser);
            await PopupNavigation.Instance.PopAsync();

        }

        private async void DoneClick(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem != null && lstUsers.SelectedItem.GetType().Equals(typeof(User)))
            {
                selectedUser = (User)lstUsers.SelectedItem;
                try
                {
                    await PopupNavigation.Instance.PopAsync();
                    MessagingCenter.Send(selectedUser, "userData");
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }
            }else
                await PopupNavigation.Instance.PopAsync();

        }






    }
}
