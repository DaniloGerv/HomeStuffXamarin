using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using homestuff.entities;
using homestuff.repositories;
using homestuff.services;
using homestuff.viewModels;
using homestuff.views;
using Xamarin.Forms;

namespace homestuff
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : TabbedPage
    {
        User user;
        UserRepository userRepository;
        FamilyRepository familyRepository;
        FamilyTabPageViewModel familyTabPageViewModel;
        FirebaseStorageService firebaseStorageService;

        public MainPage(User u)
        {
            user = u;
            userRepository = new UserRepository();
            familyRepository = new FamilyRepository();
            firebaseStorageService = new FirebaseStorageService();
            InitializeComponent();
            CurrentPageChanged += CurrentPageHasChanged;
            Appearing += OnAppearing;

        }

        private async void OnAppearing(object sender,EventArgs e)
        {
            familyTabPageViewModel = new FamilyTabPageViewModel();
          
            if (string.IsNullOrEmpty(user.photoURL))
                user.photoURL = "placeholder_profile.png";
            else
            {
                if (!File.Exists(user.photoURL))
                {
                    await LoadImmage();
                }
            }

            familyTabPageViewModel.user = user;

            if (!string.IsNullOrEmpty(familyTabPageViewModel.user.familyId))
                familyTabPageViewModel.family = await familyRepository.GetOneAsync(familyTabPageViewModel.user.familyId);

            List<User> members = new List<User>();
            foreach (string id in familyTabPageViewModel.family.membersID)
                members.Add(await userRepository.GetOneAsync(id));
            familyTabPageViewModel.members = members;

            BindingContext = familyTabPageViewModel;

        }
    

        public async Task LoadImmage()
        {
            try
            {
                string path = DownloadImage(new Uri(await firebaseStorageService.GetFile(user.uid), UriKind.Absolute));
                user.photoURL = path;
                await userRepository.AddAsync(user);
            }
            catch (Exception e)
            {
                user.photoURL = "placeholder_profile.png";
            }

        }

        public String DownloadImage(Uri URL)
        {
            WebClient webClient = new WebClient();

            string folderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Pictures", "temp");
            string fileName = URL.ToString().Split('/').Last();
            string filePath = System.IO.Path.Combine(folderPath, fileName);

            webClient.DownloadDataCompleted += (s, e) =>
            {
                Directory.CreateDirectory(folderPath);

                File.WriteAllBytes(filePath, e.Result);
            };

            webClient.DownloadDataAsync(URL);

            return filePath;
        }

        private async void CurrentPageHasChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(user.firstName) || string.IsNullOrEmpty(user.lastName) || user.place == null)
            {
                if (CurrentPage == Children[0] || CurrentPage == Children[1])
                {
                    await DisplayAlert("Errore", "Completare il profilo utente per utilizzare questa funzionalità", "OK");
                    CurrentPage = Children[2]; //Going to the profile page
                }
            }
            else {
                if (string.IsNullOrEmpty(user.familyId) && CurrentPage==Children[0])
                {
                    await DisplayAlert("Errore", "Associare il profilo ad una famiglia per utilizzare questa funzionalità", "OK");
                    CurrentPage = Children[1]; //Going to the family page

                }

            }

            if (CurrentPage!=Children[0])
            {
                MessagingCenter.Unsubscribe<User>(this,"userData");
            }

        }


       


    }
}
