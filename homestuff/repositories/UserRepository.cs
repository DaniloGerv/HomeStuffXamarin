using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using homestuff.entities;
using homestuff.interfaces;
using Plugin.CloudFirestore;

namespace homestuff.repositories
{
    public class UserRepository: IRepository<User>
    {
        private string collection = "users";
        public UserRepository()
        {
        }


        public async Task AddAsync(User obj)
        {
            await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection(collection)
                         .GetDocument(obj.uid)
                         .SetDataAsync(obj);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var group = await CrossCloudFirestore.Current
                                      .Instance
                                      .GetCollectionGroup(collection)
                                      .GetDocumentsAsync();

            return group.ToObjects<User>();
           
        }   


        public async Task<IEnumerable<User>> GetAllAsyncFiltered(string filter)
        {
            var group = await CrossCloudFirestore.Current
                                      .Instance
                                      .GetCollectionGroup(collection)
                                      .WhereEqualsTo("place.address",filter)
                                      .WhereEqualsTo("familyId",null)
                                      .GetDocumentsAsync();

            return group.ToObjects<User>();

        }

        public async Task<User> GetOneAsync(string id)
        {
            var document = await CrossCloudFirestore.Current
                                            .Instance
                                            .GetCollection(collection)
                                            .GetDocument(id)
                                            .GetDocumentAsync();

            var result=document.GetData(new ServerTimestampBehavior()).Values;
            return document.ToObject<User>();
            
        }


    }
}
