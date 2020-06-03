using System;
using System.Collections.Generic;
using homestuff.entities;
using homestuff.viewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace homestuff.popups
{
    public partial class DomesticasPopup : PopupPage
    {
        UsersForDomesticas domesticas;
        FamilyTabPageViewModel familyTabPageViewModel;
        bool edit;

        public DomesticasPopup(FamilyTabPageViewModel familyTabPageViewModel,bool edit,UsersForDomesticas domesticas)
        {
            InitializeComponent();
            this.edit = edit;
            this.familyTabPageViewModel = familyTabPageViewModel;
            BindingContext = familyTabPageViewModel.members;

            if (!edit)
            {
                lblDomesticas.Text = "Aggiungi compito famigliare";
                this.domesticas = new UsersForDomesticas(new Domesticas(), familyTabPageViewModel.user);
            }
            else
            {
                lblDomesticas.Text = "Modifica compito famigliare";
                this.domesticas = domesticas;
                domesticasNameEntry.Text = this.domesticas.domesticas.name;
                lstUsers.SelectedItem = ((List<User>)BindingContext).Find(use => use.uid.Equals(this.domesticas.user.uid));
            }
        }


        private async void LeaveClick(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();

        }

        private async void DoneClick(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem != null && lstUsers.SelectedItem.GetType().Equals(typeof(User)) && !string.IsNullOrEmpty(domesticasNameEntry.Text))
            {
                domesticas.domesticas.name = domesticasNameEntry.Text;
                domesticas.user = (User)lstUsers.SelectedItem;
                domesticas.domesticas.user = domesticas.user.uid;

                try
                {
                    await PopupNavigation.Instance.PopAsync();
                    if (edit)
                    MessagingCenter.Send(domesticas, "domesticasDataEdit");
                    else
                    MessagingCenter.Send(domesticas, "domesticasDataAdd");

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }
            }
            else
                await DisplayAlert("Compito famigliare", "Inserire tutte le informazioni necessarie", "OK");

        }

    }
}
