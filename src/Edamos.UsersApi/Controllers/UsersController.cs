using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edamos.UsersApi.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(string id)
        {
            return this.Ok(new
            {
                Identity = new
                {
                    this.User.Identity.Name,
                    this.User.Identity.AuthenticationType,
                    this.User.Identity.IsAuthenticated
                },
                Claims = this.User.Claims.Select(c => $"{c.Issuer}-{c.Type}|{c.ValueType.Split('#').Last()} : {c.Value}").ToArray(),
                IsAdmin = this.User.IsInRole("admin")
            });
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
