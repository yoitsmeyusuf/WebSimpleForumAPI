// <snippet_File>
using Takasbu.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Takasbu.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _UsersCollection;

    // <snippet_ctor>
    public UsersService(IOptions<UserStoreDatabaseSettings> UserStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(UserStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(UserStoreDatabaseSettings.Value.DatabaseName);

        _UsersCollection = mongoDatabase.GetCollection<User>(
            UserStoreDatabaseSettings.Value.UsersCollectionName
        );
    }

    // </snippet_ctor>

    public async Task<List<User>> GetAsync() =>
        await _UsersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string id) =>
        await _UsersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetAsync(string Username, bool Usernameusing) =>
        await _UsersCollection.Find(x => x.Username == Username).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) => await _UsersCollection.InsertOneAsync(newUser);

    public async Task UpdateAsync(string id, User updatedUser) =>
        await _UsersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task UpdateAsync(string id, User updatedUser, bool Usernameusing) =>
        await _UsersCollection.ReplaceOneAsync(x => x.Username == id, updatedUser);

    public async Task RemoveAsync(string id) =>
        await _UsersCollection.DeleteOneAsync(x => x.Id == id);
}
// </snippet_File>
