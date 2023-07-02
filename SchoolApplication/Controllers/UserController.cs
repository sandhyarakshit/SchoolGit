using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApplication.Models;

namespace SchoolApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("angularApplication")]
    public class UserController : Controller
    {
        private readonly IConfiguration _config;
        private readonly StudentDbContext _context;

        public UserController(IConfiguration config, StudentDbContext context)
        {
            _config = config;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public IActionResult Create(User user)
        {
            if (_context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null)
            {
                return Ok("Already Exists");
            }
            user.MemberSince = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Success");
        }
        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public IActionResult Login(Login user)
        {
            var userAvailable = _context.Users.Where(u => u.Email == user.Email && u.Pwd == user.Password).FirstOrDefault();
           if( userAvailable!=null ) {

                return Ok(new JwtService(_config).GenerateToken(
                    userAvailable.UserId.ToString(),
                    userAvailable.FirstName,
                    userAvailable.LastName,
                    userAvailable.Email,
                    userAvailable.Mobile,
                    userAvailable.Gender
                    ));
            }
            return Ok("Failure");
        }
       
    }
}
