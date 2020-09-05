using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamoDb.Contracts;
using DynamoDb.Contracts.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamoDb.WritersApp.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WritersController : ControllerBase
    {
        private IWritersRepository _repository;

        public WritersController(IWritersRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IEnumerable<Writer>> Get(string userName = "")
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var readers = await _repository.Find(new InputModel { Username = userName });
                return readers;
            }
            else
            {
                var readers = await _repository.All();
                return readers.Writers;
            }
        }

        [HttpGet]
        [Route("{readerId}")]
        public async Task<IActionResult> Single(Guid readerId)
        {
            try
            {
                var reader = await _repository.Single(readerId);

                if (reader != null)
                    return Ok(reader);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // POST: WritersController/Create
        [HttpPost]
        public async Task<ActionResult> Create(InputModel model)
        {
            try
            {
                await _repository.Add(new Writer(model.EmailAddress, model.Username, model.Name));
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // POST: WritersController/Edit/5
        [HttpPatch]
        [Route("{readerId}")]
        public async Task<ActionResult> Edit(Guid readerId, InputModel model)
        {
            try
            {
                await _repository.Update(readerId, new Writer(model.EmailAddress, model.Username, model.Name));
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("{readerId}")]
        public async Task<ActionResult> Delete(Guid readerId)
        {
            try
            {
                await _repository.Remove(readerId);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
