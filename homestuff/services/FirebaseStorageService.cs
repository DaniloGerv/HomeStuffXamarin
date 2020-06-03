using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Storage;

namespace homestuff.services
{
    public class FirebaseStorageService
    {
        FirebaseStorage firebaseStorage = new FirebaseStorage("homestuff-a8614.appspot.com");

        public async Task<string> UploadFile(Stream fileStream, string fileName)
        {
            var imageUrl = await firebaseStorage
                .Child("profileImages")
                .Child(fileName)
                .PutAsync(fileStream);
            return imageUrl;
        }

        public async Task<string> GetFile(string fileName)
        {
            
            return await firebaseStorage
                .Child("profileImages")
                .Child(fileName)
                .GetDownloadUrlAsync();
        }
        public async Task DeleteFile(string fileName)
        {
            await firebaseStorage
                 .Child("profileImages")
                 .Child(fileName)
                 .DeleteAsync();

        }
    }
}
