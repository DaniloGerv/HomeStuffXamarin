using System;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Firebase.Auth;
using homestuff.interfaces;

namespace homestuff.Droid.services
{
    public class FirebaseAuthentication : IFirebaseAuthentication
    {
        public string GetID()
        {
            if (IsSignIn())
                return Firebase.Auth.FirebaseAuth.Instance.Uid;
            else
                return string.Empty;
        }

        public bool IsSignIn()
        {
            var user = Firebase.Auth.FirebaseAuth.Instance.CurrentUser;
            return user != null;
        }

        public async Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return string.Empty;
            try
            {
                var user = await Firebase.Auth.FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync(false);
                return token.Token;
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch (FirebaseAuthInvalidCredentialsException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
        }

        public async Task<string> RegisterWithEmailAndPassword(string email,string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return string.Empty;
            try
            {
               await Firebase.Auth.FirebaseAuth.Instance.CreateUserWithEmailAndPassword(email, password);
                var user = Firebase.Auth.FirebaseAuth.Instance.Uid;
                return user;
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch (FirebaseAuthInvalidCredentialsException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
        }

        public bool SignOut()
        {
            try
            {
                Firebase.Auth.FirebaseAuth.Instance.SignOut();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
