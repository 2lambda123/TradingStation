﻿using DTO;
using Microsoft.AspNetCore.Mvc;
using UserService.Commands;
using UserService.Interfaces;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserServiceController : Controller
    {
        
        [Route("create")]
        [HttpPost] 
        /// <summary>
        /// This method will be implemented in communication with other services
        /// </summary>
        public string CreateUser([FromServices] ICreateUserCommand command, [FromBody] UserCredential userCredential)
        {
            return command.Execute(userCredential);
        }
    }
}
