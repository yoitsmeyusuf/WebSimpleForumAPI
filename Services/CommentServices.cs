// <snippet_File>
using ForumApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace ForumApi.Services;

public class CommentService
{
    private readonly IMongoCollection<Comment> _CommentsCollection;

    // <snippet_ctor>
    public CommentService(IOptions<CommentDatabaseSettings> CommentStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(CommentStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            CommentStoreDatabaseSettings.Value.DatabaseName
        );

        _CommentsCollection = mongoDatabase.GetCollection<Comment>(
            CommentStoreDatabaseSettings.Value.CommentsCollectionName
        );
    }

    // </snippet_ctor>

    public async Task<List<Comment>> GetAsync() =>
        await _CommentsCollection.Find(_ => true).ToListAsync();

    public async Task<Comment?> GetAsync(string id) =>
        await _CommentsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Comment>> GetListAsync(List<string> ids)
    {
        var filter = Builders<Comment>.Filter.In(x => x.Id, ids);
        var result = await _CommentsCollection.Find(filter).ToListAsync();
        return result;
    }

    public async Task CreateAsync(Comment newComment)
    {
        newComment.DateTime = DateTime.Now;
        await _CommentsCollection.InsertOneAsync(newComment);
    }

    public async Task UpdateAsync(string id, Comment updatedComment) =>
        await _CommentsCollection.ReplaceOneAsync(x => x.Id == id, updatedComment);

    public async Task RemoveAsync(string id) =>
        await _CommentsCollection.DeleteOneAsync(x => x.Id == id);
}
// </snippet_File>
