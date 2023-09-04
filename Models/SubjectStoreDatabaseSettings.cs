// <snippet_File>
namespace Takasbu.Services
{
    public class SubjectStoreDatabaseSettings
    {
        public string SubjectFilesBucketName { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string SubjectsCollectionName { get; set; } = null!;
    }
}
