using ForumApi.Models;
using ForumApi.Models.DTO;
using ForumApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

using System.Text.RegularExpressions;

//TODO services eklicez UNUTMA YUSUF
namespace ForumApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AuthService _AuthService;
        private readonly UsersService _UsersService;
        private readonly MailServices _MailServices;

        private readonly SubjectService _SubjectService;

        public static User user = new User();
        public static Subject subject = new Subject();
        public static Comment comment = new Comment();
        private readonly CommentService _CommentService;

        public AuthController(
            IConfiguration configuration,
            UsersService UsersService,
            AuthService AuthService,
            SubjectService SubjectService,
            CommentService CommentService,
            MailServices mailServices
        )
        {
            _SubjectService = SubjectService;
            _AuthService = AuthService;
            _CommentService = CommentService;
            _UsersService = UsersService;
            _MailServices = mailServices;
        }

        public async Task<List<User>> Get() => await _UsersService.GetAsync();

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Registration")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Registration([FromForm]UserDto request)
        {
            user.Id = string.Empty;
            if (
                request.Password == null
                || request.Password == ""
                || request.Username == null
                || request.Username == ""
            )
            {
                return BadRequest("pass yada us ");
            }
            var users = await _UsersService.GetAsync();
            var userg = users.FirstOrDefault(u => u.Username == request.Username);

            if (userg == null)
            {
                _AuthService.CreatePasswordHash(
                    request.Password,
                    out byte[] passwordHash,
                    out byte[] passwordSalt
                );
                user.Username = request.Username;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                await _UsersService.CreateAsync(user);

                Random rnd = new Random();
                int Random = rnd.Next(1000, 9999);
                _MailServices.SendSimpleMessage(user.Username, Random.ToString());
                user.mailpass = Random.ToString();
                await _UsersService.UpdateAsync(user.Id, user);

                return Ok(user);
            }

            return BadRequest("There is a same Username with a user");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> LoginAsync([FromForm]UserDto request,[FromForm] string mail)
        {
            var users = await _UsersService.GetAsync();

            var user = users.FirstOrDefault(u => u.Username == request.Username);

            if (user == null)
                return BadRequest("User not Found");

            if (user.mailpass != mail || user.mailpass == " ")
            {
                return BadRequest("Yanlış yada Mail (Tekrar) alınmamis");
            }
            user.mailpass = " ";
            user.verified = true;
            await _UsersService.UpdateAsync(user.Id, user);

            if (
                !_AuthService.VerifyPasswordHash(
                    request.Password,
                    user.PasswordHash!,
                    user.PasswordSalt!
                )
            )
                return BadRequest("Incorrect Username or Password!");

            var token = _AuthService.GenerateToken(user);

            return Ok(token);
        }

        [HttpPut("Updatebio")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<string>> biochange(string username, string newbio)
        {
            if (aut(username))
            {
                var users = await _UsersService.GetAsync();

                var user = users.FirstOrDefault(u => u.Username == username);
                user!.Bio = newbio;
                await _UsersService.UpdateAsync(user.Username, user, true);

                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("DeleteSubject")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Subject>> DeleteSubject(string subjectid)
        {
            subject.Id = string.Empty;

            var _bearer_token = Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);
            if (token == null)
            {
                return BadRequest("Token  cant found");
            }

            var user = await _UsersService.GetAsync(token, true);

            if (user == null)
            {
                return BadRequest("User cant found");
            }

            if (!user.SubjectIds.Contains(subjectid))
            {
                return BadRequest("The subject is not yours");
            }

            await _SubjectService.RemoveAsync(subjectid);

            return Ok("Deleted");
        }

        [HttpDelete("DeleteComment")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Subject>> DeleteComment(string commentid)
        {
            subject.Id = string.Empty;

            var _bearer_token = Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);
            if (token == null)
            {
                return BadRequest("Token  cant found");
            }

            var user = await _UsersService.GetAsync(token, true);

            if (user == null)
            {
                return BadRequest("User cant found");
            }

            if (!user.CommentIds.Contains(commentid))
            {
                return BadRequest("The subject is not yours");
            }

            await _CommentService.RemoveAsync(commentid);

            return Ok("Deleted");
        }

        [HttpPost("CreateSubject")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Subject>> CreateSubject(SubjectDTO request)
        {
            subject.Id = string.Empty;

            var _bearer_token = Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);
            if (token == null)
            {
                return BadRequest("Token  cant found");
            }

            var user = await _UsersService.GetAsync(token, true);

            if (user == null)
            {
                return BadRequest("User cant found");
            }

            subject.Name = request.Name;

            subject.Description = request.Description;

            subject.Owner = token;

            await _SubjectService.CreateAsync(subject);
            user.SubjectIds.Add(subject.Id);
            await _UsersService.UpdateAsync(user.Id, user);

            return Ok(subject);
        }

        [HttpPost("CreateComment")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Comment>> CreateComment(CommentDTO request)
        {
            comment.Id = string.Empty;
            var _bearer_token = Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);
            if (token == null)
            {
                return BadRequest("Token  cant found");
            }

            var user = await _UsersService.GetAsync(token, true);

            var SSubject = await _SubjectService.GetAsync(request.Subject);

            if (user == null || SSubject == null || request.Context == null)
            {
                return BadRequest("User cant found");
            }

            comment.context = request.Context;

            comment.SubjectId = request.Subject;

            comment.Owner = token;

            await _CommentService.CreateAsync(comment);
            user.CommentIds.Add(comment.Id);
            SSubject.Comments.Add(comment.Id);
            var kelimeler = etiketkontrol(request.Context);
            foreach (string kelime in kelimeler)
            {
                var ussser = await _UsersService.GetAsync(kelime, true);
                if (ussser != null)
                {
                    ussser.notifcomids.Add(comment.Id);
                    Console.WriteLine(ussser.Id);
                    await _UsersService.UpdateAsync(ussser.Id, ussser);
                }
            }
            await _UsersService.UpdateAsync(user.Id, user);
            await _SubjectService.UpdateAsync(request.Subject, SSubject);

            return Ok(comment);
        }

        [HttpGet("GetSubjects")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<string>> GeTSubjects()
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);
            if (token == null)
            {
                return BadRequest("Token cant found");
            }
            var user = await _UsersService.GetAsync(token, true);

            if (user == null)
            {
                return BadRequest("User cant found");
            }

            var result = await _SubjectService.GetListAsync(user.SubjectIds);

            return Ok(result);
        }

        #region client
        [HttpGet("GetAllSubjects")]
        public async Task<ActionResult<Subject>> GetAllSubjects()
        {
            var result = await _SubjectService.GetAsync();
            return Ok(result);
        }

        [HttpGet("GetSubjectsComments")]
        public async Task<ActionResult<string>> GetSubjectsComments(string id)
        {
            var result = await _SubjectService.GetAsync(id);
            if (result == null)
            {
                return BadRequest("no comments");
            }
            return Ok(result.Comments);
        }

        #endregion

        [HttpPut("UpdateSelf")]
        [Authorize(Roles = "User")]
        //TODO bu biraz tehlikeli fakat çözmüş olabilirim rolesin setterını kaldırarak

        public async Task<IActionResult> Update(string UserName, User updatedUser)
        {
            if (aut(UserName))
            {
                var User = await _UsersService.GetAsync(UserName, true);

                if (User is null)
                {
                    return NotFound();
                }

                updatedUser.Id = User.Id;

                await _UsersService.UpdateAsync(UserName, updatedUser, true);

                return NoContent();
            }
            return BadRequest("Token or User cant found");
        }

        //private methods
        private bool aut(string UserName)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");
            var token = _AuthService.GetUserIdFromToken(_bearer_token!);

            if (UserName == token)
            {
                return true;
            }

            return false;
        }

        static string[] etiketkontrol(string metin)
        {
            // Metindeki "@" ile başlayan kelimeleri bulmak için regex deseni
            string desen = @"@\w+";

            // Regex eşleşmelerini bulma
            MatchCollection eslesmeler = Regex.Matches(metin, desen);

            // Eşleşen kelimeleri bir dizi olarak almak
            string[] kelimeler = eslesmeler.Cast<Match>().Select(m => m.Value).ToArray();
            string[] yeniDizi = new string[kelimeler.Length];
            for (int i = 0; i < kelimeler.Length; i++)
            {
                yeniDizi[i] = kelimeler[i].Replace("@", ""); // 'a' harfini çıkar
            }
            Console.WriteLine(yeniDizi[0]);
            return yeniDizi;
        }
    }
}
