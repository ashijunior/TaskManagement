using Microsoft.AspNetCore.Mvc;
using TaskManager_Simplex_.Models;
using TaskManager_Simplex_.Repositories.Interface;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using TaskManager_Simplex_.Context;
using TaskManager_Simplex_.Hasher;

namespace TaskManager_Simplex_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly AppDbContext _context;

        public UserController(IUserRepo userRepo, AppDbContext context)
        {
            _userRepo = userRepo;
            _context = context;
        }

        //To Create User
        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user, [FromQuery] string role = "User")
        {
            if (user == null)
                return BadRequest();

            // Check if the provided data is valid
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            // Check if the username already exists
            if (await CheckUserNameExistAsync(user.UserName))
                return BadRequest(new { Message = "Username already exists" });

            // Check if the email already exists
            if (await CheckEmailExistAsync(user.Email))
                return BadRequest(new { Message = "Email already exists" });

            // Check the password strength
            var passwordStrengthMessage = CheckPasswordStrength(user.Password);
            if (!string.IsNullOrEmpty(passwordStrengthMessage))
                return BadRequest(new { Message = passwordStrengthMessage });

            // Hash the password
            user.Password = PasswordHasher.HashPassword(user.Password);

            // Validate the role
            var validRoles = new[] { "User", "Admin", "Client" };
            if (!validRoles.Contains(role))
            {
                return BadRequest(new { Message = "Invalid role specified" });
            }

            user.Role = role;
            user.Token = string.Empty;

            // Add user to repository
            var addedUser = await _userRepo.CreateUserAsync(user);

            return Ok(new
            {
                Message = "User registered",
                User = new
                {
                    UserName = addedUser.UserName,
                    Email = addedUser.Email,
                    Role = addedUser.Role,
                    Token = addedUser.Token
                }
            });
        }

        private async Task<bool> CheckUserNameExistAsync(string userName) =>
            await _context.Users.AnyAsync(s => s.UserName == userName);

        private async Task<bool> CheckEmailExistAsync(string email) =>
            await _context.Users.AnyAsync(s => s.Email == email);

        private string CheckPasswordStrength(string password)
        {
            var sb = new StringBuilder();
            if (password.Length < 7)
                sb.AppendLine("Minimum password length is 7");
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.AppendLine("Password should contain a capital letter and be alphanumeric");
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.AppendLine("Password should contain special characters");
            return sb.ToString();
        }

        //To Get All Users
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userRepo.GetAllUsersAsync();
            return Ok(users);
        }


        //To Update Users
        [HttpPut("UpdateUser/{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null || id != user.Id)
                return BadRequest("Invalid user data.");

            var existingUser = await _userRepo.GetUserByIdAsync(id);
            if (existingUser == null)
                return NotFound("User not found.");

            // Update user details
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;

            // If password is being updated, hash the new password
            if (!string.IsNullOrEmpty(user.Password))
            {
                var passwordStrengthMessage = CheckPasswordStrength(user.Password);
                if (!string.IsNullOrEmpty(passwordStrengthMessage))
                    return BadRequest(new { Message = passwordStrengthMessage });

                existingUser.Password = PasswordHasher.HashPassword(user.Password);
            }

            var updatedUser = await _userRepo.UpdateUserAsync(existingUser);

            return Ok(new
            {
                Message = "User updated",
                User = new
                {
                    UserName = updatedUser.UserName,
                    Email = updatedUser.Email,
                    Role = updatedUser.Role,
                    Token = updatedUser.Token
                }
            });
        }

        //To Delete User
        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            await _userRepo.DeleteUserAsync(id);

            return Ok(new { Message = "User deleted" });
        }

    }
}
