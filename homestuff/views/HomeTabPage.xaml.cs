using System;
using System.Collections.Generic;
using homestuff.entities;
using homestuff.popups;
using homestuff.repositories;
using homestuff.viewModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace homestuff.views
{
    public partial class HomeTabPage : ContentPage
    {
        FamilyRepository familyRepository;
        UserRepository userRepository;
        bool firstTime = true;
        FamilyTabPageViewModel familyTabPageViewModel;

        public HomeTabPage()
        {
            InitializeComponent();
            familyTabPageViewModel = new FamilyTabPageViewModel();
            familyRepository = new FamilyRepository();
            userRepository = new UserRepository();
            Appearing += OnAppearing;
            BindingContextChanged += OnBindingContextChanged;
        
        }

        private async void OnBindingContextChanged(object sender, EventArgs e)
        {
          

                familyTabPageViewModel = (FamilyTabPageViewModel)Parent.BindingContext;
                if (!string.IsNullOrEmpty(familyTabPageViewModel.user.familyId))
                    familyTabPageViewModel.family = await familyRepository.GetOneAsync(familyTabPageViewModel.user.familyId);

                if (familyTabPageViewModel.family != null)
                {
                   

                    if (familyTabPageViewModel.family.toDoList != null && familyTabPageViewModel.family.toDoList.Count > 0 && ((List<UsersForDomesticas>)familyTabPageViewModel.usersForDomesticas).Count != familyTabPageViewModel.family.toDoList.Count)
                    {
                        lblNoDomestica.IsVisible = false;
                        lstDomesticas.IsVisible = true;
                        familyTabPageViewModel.usersForDomesticas = new List<UsersForDomesticas>();
                        foreach (Domesticas d in familyTabPageViewModel.family.toDoList)
                        {
                            familyTabPageViewModel.addUsersForDomesticas(new UsersForDomesticas(d, ((List<User>)familyTabPageViewModel.members).Find(use => use.uid.Equals(d.user))));
                        }
                    }
                    else if (familyTabPageViewModel.family.toDoList.Count == 0)
                    {
                        lblNoDomestica.IsVisible = true;
                        lstDomesticas.IsVisible = false;
                    }
                    await Device.InvokeOnMainThreadAsync(async () => { lstDomesticas.ItemsSource = familyTabPageViewModel.usersForDomesticas; });
                    firstTime = false;
                }
            
        }

            private async void OnAppearing(object sender, EventArgs e)
        {
            if (BindingContext != null && !firstTime)
            {
                if (!string.IsNullOrEmpty(familyTabPageViewModel.user.familyId))
                    familyTabPageViewModel.family = await familyRepository.GetOneAsync(familyTabPageViewModel.user.familyId);

                List<User> members = new List<User>();
                foreach (string id in familyTabPageViewModel.family.membersID)
                    members.Add(await userRepository.GetOneAsync(id));
                familyTabPageViewModel.members = members;

                if (familyTabPageViewModel.family.toDoList != null && familyTabPageViewModel.family.toDoList.Count > 0)
                {
                    lblNoDomestica.IsVisible = false;
                    lstDomesticas.IsVisible = true;
                    familyTabPageViewModel.usersForDomesticas = new List<UsersForDomesticas>();
                    foreach (Domesticas d in familyTabPageViewModel.family.toDoList)
                    {
                        familyTabPageViewModel.addUsersForDomesticas(new UsersForDomesticas(d, await userRepository.GetOneAsync(d.user)));
                    }
                }
                else
                {
                    lblNoDomestica.IsVisible = true;
                    lstDomesticas.IsVisible = false;
                    familyTabPageViewModel.usersForDomesticas = new List<UsersForDomesticas>();
                }
                await Device.InvokeOnMainThreadAsync(async() => {lstDomesticas.ItemsSource = familyTabPageViewModel.usersForDomesticas; });
            }
        }



        private async void AddClick(object sender, EventArgs e)
        {
            var item = new UsersForDomesticas();

            MessagingCenter.Subscribe<UsersForDomesticas>(this, "domesticasDataAdd", async (value) =>
            {
                if (value != null)
                {

                    value.domesticas.domesticaID = utilities.Utilities.RandomString(20);
                    value.domesticas.familyID = familyTabPageViewModel.family.fid;
                    familyTabPageViewModel.family.addDomesticas(value.domesticas);
                    familyTabPageViewModel.addUsersForDomesticas(value);
                    await familyRepository.AddAsync(familyTabPageViewModel.family);
                    await DisplayAlert("Compito famigliare", "Compito famigliare aggiunto correttamente", "OK");
                    lblNoDomestica.IsVisible = false;
                    lstDomesticas.IsVisible = true;
                    await Device.InvokeOnMainThreadAsync(async () => { lstDomesticas.ItemsSource = familyTabPageViewModel.usersForDomesticas; });
                    MessagingCenter.Unsubscribe<UsersForDomesticas>(this, "domesticasDataAdd");

                }
            });
            if (((ImageButton)sender).ClassId.Equals("btnAdd"))
                await PopupNavigation.Instance.PushAsync(new DomesticasPopup(familyTabPageViewModel, false,null), true);  
            else
                await DisplayAlert("Compito famigliare", "Il compito famigliare selezionato è associato ad un altro utente. Operazione non completabile", "OK");
        }

        private async void EditClick(object sender,EventArgs e)
        {
            var item=new UsersForDomesticas();
            if (((ImageButton)sender).ClassId.Equals("btnEdit"))
                 item = (UsersForDomesticas)((ImageButton)sender).BindingContext;

            MessagingCenter.Subscribe<UsersForDomesticas>(this, "domesticasDataEdit", async (value) =>
            {
                if (value != null)
                {

                      familyTabPageViewModel.family.toDoList.Remove(item.domesticas);
                      familyTabPageViewModel.removeUsersForDomesticas(((UsersForDomesticas)lstDomesticas.SelectedItem));

                    value.domesticas.familyID = familyTabPageViewModel.family.fid;
                    value.domesticas.domesticaID = utilities.Utilities.RandomString(20);
                    familyTabPageViewModel.family.addDomesticas(value.domesticas);
                    familyTabPageViewModel.addUsersForDomesticas(value);
                    await familyRepository.AddAsync(familyTabPageViewModel.family);
                     await DisplayAlert("Compito famigliare", "Compito famigliare modificato correttamente", "OK");
                    lblNoDomestica.IsVisible = false;
                    lstDomesticas.IsVisible = true;
                    await Device.InvokeOnMainThreadAsync(async () => { lstDomesticas.ItemsSource = familyTabPageViewModel.usersForDomesticas; });
                    MessagingCenter.Unsubscribe<UsersForDomesticas>(this, "domesticasDataEdit");

                }
            });
            if (item != null && item.user.uid.Equals(familyTabPageViewModel.user.uid))
                await PopupNavigation.Instance.PushAsync(new DomesticasPopup(familyTabPageViewModel, true, item), true);
            else
                await DisplayAlert("Compito famigliare", "Il compito famigliare selezionato è associato ad un altro utente. Operazione non completabile", "OK");
        }

        private async void CompleteClick(object sender, EventArgs e)
        {
            var item = (UsersForDomesticas)((ImageButton)sender).BindingContext;
            if (item!=null && item.user.uid.Equals(familyTabPageViewModel.user.uid))
            {
                familyTabPageViewModel.family.toDoList.Remove(item.domesticas);
                familyTabPageViewModel.removeUsersForDomesticas(item);
                await familyRepository.AddAsync(familyTabPageViewModel.family);
                await DisplayAlert("Compito famigliare", "Compito famigliare correttamente completato", "OK", "Cancel");
                if (familyTabPageViewModel.family.toDoList != null && familyTabPageViewModel.family.toDoList.Count > 0)
                {
                    lblNoDomestica.IsVisible = false;
                    lstDomesticas.IsVisible = true;
                }else
                {
                    lblNoDomestica.IsVisible = true;
                    lstDomesticas.IsVisible = false;
                }
                await Device.InvokeOnMainThreadAsync(async () => { lstDomesticas.ItemsSource = familyTabPageViewModel.usersForDomesticas; });

            }
            else
            {
                await DisplayAlert("Compito famigliare", "Il compito famigliare selezionato è associato ad un altro utente. Operazione non completabile", "OK");
            }

         }


     

        private bool IsForUser(Domesticas d)
        {
            if (d.user.Equals(familyTabPageViewModel.user.uid))
                return true;
            else
                return false;
        }




    }
}
