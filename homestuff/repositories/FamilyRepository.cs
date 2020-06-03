using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using homestuff.entities;
using homestuff.interfaces;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Extensions;

namespace homestuff.repositories
{
    public class FamilyRepository: IRepository<Family>
    {
        private string collection = "families";
        public FamilyRepository()
        {
        }


        public async Task AddAsync(Family obj)
        {
            await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection(collection)
                         .GetDocument(obj.fid)
                         .SetDataAsync(obj);
        }

        public async Task<IEnumerable<Family>> GetAllAsync()
        {
            var group = await CrossCloudFirestore.Current
                                      .Instance
                                      .GetCollectionGroup(collection)
                                      .GetDocumentsAsync();

            return group.ToObjects<Family>();
           
        }


        public async Task<Family> GetOneAsync(string id)
        {
            var document = await CrossCloudFirestore.Current
                                        .Instance
                                        .GetCollection(collection)
                                        .GetDocument(id)
                                        .GetDocumentAsync();

            return document.ToObject<Family>();
        }



       

    }
}
