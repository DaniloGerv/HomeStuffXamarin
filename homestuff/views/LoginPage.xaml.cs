using System;
using System.Collections.Generic;
using System.Diagnostics;
using homestuff.entities;
using homestuff.interfaces;
using homestuff.repositories;
using Xamarin.Forms;

namespace homestuff
{
    public partial class LoginPage : ContentPage
    {
        UserRepository userRepository;
        IFirebaseAuthentication auth;
        public LoginPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<IFirebaseAuthentication>();
            BindingContext = new User();
            userRepository = new UserRepository();

        }

        public async void LoginButton_Clicked(object sender, EventArgs e)
        {

            try
            {
                string token = await auth.LoginWithEmailAndPassword(emailEntry.Text, passwordEntry.Text);
            if (token != string.Empty)
            {
                User u = new User();
                u.email = emailEntry.Text;
                u.password = passwordEntry.Text;
                u.uid = auth.GetID();
                Application.Current.MainPage = new MainPage(u);
            }
            else
            {
                ShowError();
            }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", "L'email inserita è già utilizzata da un altro account", "OK");
            }
        }

        public async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string uid = await auth.RegisterWithEmailAndPassword(emailEntry.Text, passwordEntry.Text);
                if (!string.IsNullOrEmpty(uid))
                {
                    User u = new User();
                    u.email = emailEntry.Text;
                    u.password = passwordEntry.Text;
                    u.uid = uid;
                    await userRepository.AddAsync(u);
                    Application.Current.MainPage = new MainPage(u);
                }
                else
                {
                    ShowError();
                }
            }catch(Exception ex)
            {
                await DisplayAlert("Errore", "L'email inserita è già utilizzata da un altro account", "OK");
            }
        }



        private async void ShowError()
        {
            await DisplayAlert("Errore", "Le credenziali di accesso inserite sono errate, riprovare!", "OK");
        }
    }
}
