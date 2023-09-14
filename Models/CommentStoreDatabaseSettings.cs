// <snippet_File>
namespace ForumApi.Services
{
    public class CommentDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string CommentsCollectionName { get; set; } = null!;
    }
}
