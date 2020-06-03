using System;
using System.Threading.Tasks;

namespace homestuff.interfaces
{
    public interface IFirebaseAuthentication
    {
        Task<string> LoginWithEmailAndPassword(string email, string password);
        Task<string> RegisterWithEmailAndPassword(string email, string password);
        string GetID();
        bool SignOut();
        bool IsSignIn();
    }
}
