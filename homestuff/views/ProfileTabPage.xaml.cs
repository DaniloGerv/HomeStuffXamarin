using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Firebase.Storage;
using homestuff.entities;
using homestuff.interfaces;
using homestuff.repositories;
using homestuff.services;
using Plugin.Geolocator;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace homestuff.views
{
    public partial class ProfileTabPage : ContentPage
    {
        User user;
        UserRepository userRepository;
        FirebaseStorageService firebaseStorageService;
        IFirebaseAuthentication auth;


        public ProfileTabPage()
        {
            InitializeComponent();
            userRepository = new UserRepository();
            firebaseStorageService = new FirebaseStorageService();
            auth = DependencyService.Get<IFirebaseAuthentication>();
            Appearing += OnAppearing;
        }


        private async void OnAppearing(object sender, EventArgs e)
        {
            user = (User)BindingContext;
            if (user.place != null)
                locationImg.Source = "baseline_thumb_up_alt_black_24dp.png";
            else
                locationImg.Source = "baseline_thumb_down_alt_black_24dp.png";
        }


        public async void DoneClick(object sender, EventArgs e)
        {

            user.email = emailEntry.Text;
            user.firstName = firstNameEntry.Text;
            user.lastName = lastNameEntry.Text;
            await userRepository.AddAsync(user);
            await DisplayAlert("Profilo utente", "Profilo aggiornato correttamente", "OK");

        }

        public async void LeaveClick(object sender, EventArgs e)
        {

            auth.SignOut();

           App.Current.MainPage= new LoginPage();

        }

        public async void LocationClick(object sender, EventArgs e)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    if (await
                     CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Localizzazione", "Si necessita l'utilizzo della geolocalizzazione", "OK");
                    }
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }
           if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var results = await CrossGeolocator.Current.GetPositionAsync();
                    try
                    {

                        var placemarks = await Geocoding.GetPlacemarksAsync(results.Latitude, results.Longitude);

                        var placemark = placemarks?.FirstOrDefault();
                        if (placemark != null)
                        {
                            entities.Location place = new entities.Location();
                            place.latitude = results.Latitude;
                            place.longitude = results.Longitude;
                            place.address = placemark.Thoroughfare + " " + placemark.FeatureName;
                            place.city = placemark.Locality;
                            place.state = placemark.AdminArea;
                            place.country = placemark.CountryName;
                            place.postalCode = placemark.PostalCode;
                           
                            user.place = place;
                            await userRepository.AddAsync(user);
                            await DisplayAlert("Profilo utente", "Geolocalizzazione effettuata correttamente", "OK");
                            locationImg.Source = "baseline_thumb_up_alt_black_24dp.png";
                        }
                    }
                    catch (FeatureNotSupportedException fnsEx)
                    {
                        await DisplayAlert("Errore", "Non è stato possibile accedere alla geolocalizzazione, riprovare", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Errore", "Non è stato possibile accedere alla geolocalizzazione, riprovare", "OK");
                    }
                }
            else if (status != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
            {
                await DisplayAlert("Errore", "Non è stato possibile accedere alla geolocalizzazione, riprovare", "OK");
            }
        }
        catch (Exception ex){
                await DisplayAlert("Errore", "Si è verificato un errore durante la geolocalizzazione", "OK");
            }

        }

        public async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null)
                    return;
                proPic.Source = ImageSource.FromStream(() =>
                {
                    var imageStram = file.GetStream();
                    return imageStram;
                });
                user.photoURL = file.Path;
                await firebaseStorageService.UploadFile(file.GetStream(), user.uid);
                await userRepository.AddAsync(user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        
    }
}
