using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using homestuff.entities;
using homestuff.interfaces;
using homestuff.popups;
using homestuff.repositories;
using homestuff.viewModels;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace homestuff.views
{
    public partial class FamilyTabPage : ContentPage
    {
        Family family;

        FamilyRepository familyRepository;
        UserRepository userRepository;

        List<User> toDisplay;

        FamilyTabPageViewModel familyTabPageViewModel;
        

        public FamilyTabPage()
        {
            InitializeComponent();
            familyTabPageViewModel = new FamilyTabPageViewModel();
            familyRepository = new FamilyRepository();
            userRepository = new UserRepository();
            toDisplay = new List<User>();
            Appearing += OnAppearing;
           
        }



        private async void OnAppearing(object sender, EventArgs e)
        {
            toDisplay.Clear();
            familyTabPageViewModel=(FamilyTabPageViewModel)BindingContext;
            if (!string.IsNullOrEmpty(familyTabPageViewModel.user.familyId))
            familyTabPageViewModel.family = familyTabPageViewModel.family = await familyRepository.GetOneAsync(familyTabPageViewModel.user.familyId);


            if (!string.IsNullOrEmpty(familyTabPageViewModel.user.familyId))
            {
                GoInEditMode();
                toDisplay = familyTabPageViewModel.members; 
            }
            else
            {
                GoInAddMode();
            }

            await Device.InvokeOnMainThreadAsync(async () => { lstUsers.ItemsSource = familyTabPageViewModel.members; });
            await Device.InvokeOnMainThreadAsync(async () => { familyNameEntry.Text = familyTabPageViewModel.family.familyName; });


        }




        public void GoInAddMode()
        {
            toDisplay.Clear();
            familyTabPageViewModel.members = new List<User> { familyTabPageViewModel.user };
            toDisplay.Add(familyTabPageViewModel.user);
            familyTabPageViewModel.family = new Family();
            familyTabPageViewModel.family.place = familyTabPageViewModel.user.place;
            familyTabPageViewModel.family.addMember(familyTabPageViewModel.user.uid);
            doneBtn.Text = "Crea famiglia";
            lblFamily.Text = "Il profilo non ha una famiglia associata";
            leaveBtn.IsVisible = false;

        }


        public void GoInEditMode()
        {
            doneBtn.Text = "Modifica famiglia";
            doneBtn.HeightRequest = 70;
            lblFamily.Text = "La tua famiglia";
            leaveBtn.IsVisible = true;
        }

        public async void LeaveClick (object sender,EventArgs e)
        {
            familyTabPageViewModel.family.removeMember(familyTabPageViewModel.user.uid);
            familyTabPageViewModel.user.familyId = null;
            await familyRepository.AddAsync(familyTabPageViewModel.family);
            await userRepository.AddAsync(familyTabPageViewModel.user);
            GoInAddMode();
            await Device.InvokeOnMainThreadAsync(async () => { lstUsers.ItemsSource = familyTabPageViewModel.members; });
            await Device.InvokeOnMainThreadAsync(async () => { familyNameEntry.Text = ""; });
            await DisplayAlert("La tua famiglia", "Sei uscito correttamente dalla famiglia", "OK");
            Parent.BindingContext = familyTabPageViewModel;

        }

        public async void DoneClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(familyNameEntry.Text))
            {
                familyTabPageViewModel.family.familyName = familyNameEntry.Text;
                familyTabPageViewModel.family.fid = utilities.Utilities.RandomString(20);
                familyTabPageViewModel.user.familyId = familyTabPageViewModel.family.fid;
                foreach (User u in familyTabPageViewModel.members)
                {
                    u.familyId = familyTabPageViewModel.family.fid;
                    await userRepository.AddAsync(u);

                }
                await familyRepository.AddAsync(familyTabPageViewModel.family);
                await DisplayAlert("La tua famiglia", "Operazione completata con successo", "OK");
                GoInEditMode();
            }else
            {
                await DisplayAlert("La tua famiglia", "Associare un nome alla famiglia", "OK");
            }
        }

        public async void AddMember(object sender, EventArgs e)
        {
           
            IEnumerable<User> members = await userRepository.GetAllAsync();
            List<User> possibleMembers = new List<User>();
            foreach (User u in members)
            {
                if (string.IsNullOrEmpty(u.familyId) && !u.uid.Equals(familyTabPageViewModel.user.uid) )
                    if (u.place!=null && !((List<string>)familyTabPageViewModel.family.membersID).Contains(u.uid))
                        if (u.place.address.Equals(familyTabPageViewModel.user.place.address))
                          possibleMembers.Add(u);
            }
            MemberAddPopup memberAddPopup = new MemberAddPopup(possibleMembers);
            MessagingCenter.Subscribe<User>(this, "userData", async (value) =>
            {
                if (value!=null && !toDisplay.Contains(value))
                {
                    familyTabPageViewModel.family.addMember(value.uid);
                    toDisplay.Add(value);
                    familyTabPageViewModel.members = toDisplay;
                    await Device.InvokeOnMainThreadAsync(async () => { lstUsers.ItemsSource = familyTabPageViewModel.members; });
                    MessagingCenter.Unsubscribe<User>(this, "userData");

                }
            });
            await PopupNavigation.Instance.PushAsync(memberAddPopup,true);

        }

        

    }
}
