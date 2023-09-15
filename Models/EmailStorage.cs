namespace ForumApi.Models;

public class EmailStorage
{
    public string SmtpServer { get; set; } = null!;

    public string Port { get; set; } = null!;

    public string SenderEmail { get; set; } = null!;

    public string Pass {get; set;} =null!;
}
