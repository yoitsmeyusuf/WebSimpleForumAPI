namespace ForumApi.Services;

public class BackgroundServices : BackgroundService
{
    private readonly ILogger<BackgroundServices> _logger;
    private readonly UsersService _usersService;
    private readonly MailServices _mailServices;

    public BackgroundServices(ILogger<BackgroundServices> logger, UsersService usersService, MailServices mailServices)
    {
        _mailServices = mailServices;
        _logger = logger;
        _usersService = usersService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("BackgroundServices running at: {time}", DateTimeOffset.Now);

            // <snippet_GetUsers>
            var users = await _usersService.GetAsync();

            // </snippet_GetUsers>

            // <snippet_Foreach>
            foreach (var user in users)
            {
                // <snippet_If>
                if (user.verified == false)
                {
                    _mailServices.SendSimpleMessage(user.Username, "<h1>your account is deleted for not verifying</h1>");
                    await _usersService.RemoveAsync(user.Id);
                }
                // </snippet_If>
            }
            // </snippet_Foreach>

            await Task.Delay(3600000, stoppingToken);
        }
    }
}
