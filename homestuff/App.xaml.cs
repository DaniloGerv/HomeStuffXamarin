using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using homestuff.entities;
using homestuff.interfaces;
using homestuff.repositories;
using homestuff.views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace homestuff
{
    public partial class App : Application
    {
        IFirebaseAuthentication auth;
        UserRepository userRepository;
        public App()
        {
            InitializeComponent();

            userRepository = new UserRepository();
            auth = DependencyService.Get<IFirebaseAuthentication>();
            MainPage = new LoadingPage();

        }

       
        protected override async void OnStart()
        {
          
            if (auth.IsSignIn())
            {
                User u = await userRepository.GetOneAsync(auth.GetID());
                MainPage = new MainPage(u);
            }
            else
            {
                MainPage = new LoginPage();
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
