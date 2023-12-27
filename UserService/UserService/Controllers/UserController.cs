using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using UserService.Data;
using UserService.Model;
using UserService.Repository;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly AppSettings _appSettings;

        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
        };

        public UserController(IUserRepository repository, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _repository = repository;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var data = _repository.getAll();
                if (data == null)
                {
                    return NotFound();
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                return Ok(_repository.getById(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, UserVM user)
        {
            try
            {
                if (id != user.Id)
                {
                    return BadRequest();
                }
                _repository.Update(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(string id)
        {
            try
            {
                _repository.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult AddUser(UserVM user)
        {
            try
            {
                return Ok(_repository.Add(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Validate(UserLogin user)
        {
            var us = _repository.Validate(user);
            if (us == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Invalid Email or Password"
                });
            }
            var token = await _repository.GenerateToken(us, _appSettings.SecretKey);
            return Ok(new
            {
                Success = true,
                Message = "Authentication success",
                Data = token
            });
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> CreateToken(TokenModel model)
        {
            var token = await _repository.GenerateRefreshToken(model, _appSettings.SecretKey);
            return Ok(token);
        }

        [HttpPost("Register")]
        public IActionResult Register(RegisterUser registerUser)
        {
            var re = _repository.Register(registerUser);
            if (re == null)
            {
                return Unauthorized(new Respone
                {
                    Success = false,
                    Message = "Registration failed"
                });
            }
            return Ok(re);
        }
    }
}
