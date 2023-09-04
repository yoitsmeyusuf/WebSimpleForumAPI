// <snippet_File>
using Takasbu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;

namespace Takasbu.Services;

public class SubjectService
{
    private readonly IMongoCollection<Subject> _SubjectsCollection;

    private readonly IGridFSBucket _gridFSBucket;

    // <snippet_ctor>
    public SubjectService(IOptions<SubjectStoreDatabaseSettings> SubjectStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(SubjectStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            SubjectStoreDatabaseSettings.Value.DatabaseName
        );

        _SubjectsCollection = mongoDatabase.GetCollection<Subject>(
            SubjectStoreDatabaseSettings.Value.SubjectsCollectionName
        );

        _gridFSBucket = new GridFSBucket(
            mongoDatabase,
            new GridFSBucketOptions
            {
                BucketName = SubjectStoreDatabaseSettings.Value.SubjectFilesBucketName
            }
        );
    }

    // </snippet_ctor>

    public async Task<List<Subject>> GetAsync() =>
        await _SubjectsCollection.Find(_ => true).ToListAsync();

    public async Task<Subject?> GetAsync(string id) =>
        await _SubjectsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Subject>> GetListAsync(List<string> ids)
    {
        var filter = Builders<Subject>.Filter.In(x => x.Id, ids);
        var result = await _SubjectsCollection.Find(filter).ToListAsync();
        return result;
    }

    public async Task<Subject?> GetAsyncu(string id) =>
        await _SubjectsCollection.Find(x => x.Name == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Subject newSubject)
    {
        newSubject.DateTime = DateTime.Now;
        await _SubjectsCollection.InsertOneAsync(newSubject);
    }

    public async Task UpdateAsync(string id, Subject updatedSubject) =>
        await _SubjectsCollection.ReplaceOneAsync(x => x.Id == id, updatedSubject);

    public async Task UpdateAsyncu(string id, Subject updatedSubject) =>
        await _SubjectsCollection.ReplaceOneAsync(x => x.Name == id, updatedSubject);

    public async Task RemoveAsync(string id) =>
        await _SubjectsCollection.DeleteOneAsync(x => x.Id == id);
}
// </snippet_File>
