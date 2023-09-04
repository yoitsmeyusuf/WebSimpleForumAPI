// <snippet_File>
namespace Takasbu.Services
{
    public class CommentDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string CommentsCollectionName { get; set; } = null!;
    }
}
