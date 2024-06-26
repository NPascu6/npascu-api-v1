﻿using Microsoft.AspNetCore.Mvc;
using npascu_api_v1.Models.DTOs.Auth;
using npascu_api_v1.Services.Interface;

namespace npascu_api_v1.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost, Route("login")]
        public IActionResult Login(LoginModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }
            if (ModelState.IsValid)
            {
                if (model.UserName == null || model.Password == null)
                {
                    return BadRequest("Invalid client request");
                }

                try
                {
                    var token = _authService.Login(model.UserName, model.Password);
                    if (token == null || token == "")
                    {
                        return Unauthorized();
                    }
                    else
                    {
                        return Ok(new { token });

                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            }
            else
            {
                return BadRequest("Invalid client request");
            }
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.UserName == null || model.Password == null || model.Email == null)
                {
                    return BadRequest("Invalid client request");
                }

                var result = _authService.Register(model.UserName, model.Email, model.Password);

                if (result == "Email is taken.")
                {
                    return BadRequest("Email is taken.");
                }

                if (result == "User already exists.")
                {
                    return BadRequest("User already exists.");
                }

                var response = new
                {
                    token = result,
                    emailValidationSent = true
                };

                if (result == null)
                {
                    return BadRequest("Invalid client request");
                }
                else
                {
                    return Ok(response);
                }
            }
            else
            {
                return BadRequest("Invalid client request");
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid client request");
            }

            var result = _authService.DeleteUser(email);

            if (result == false)
            {
                return BadRequest("Invalid client request");
            }
            else
            {
                return Ok(result);
            }
        }


        [HttpGet]
        [Route("validate-email")]
        public IActionResult ValidateEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid client request");
            }

            var result = _authService.ValidateEmail(token);

            if (result == false)
            {
                return BadRequest("Invalid client request");
            }
            else
            {
                return Ok(result);
            }
        }
    }
}
