﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker, includeSpeakers: true);
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, talkId: id, true);
                if (talk == null)
                    return NotFound();

                return _mapper.Map<TalkModel>(talk);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }


        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            var camp = await _repository.GetCampAsync(moniker);
            if (camp == null)
                return BadRequest("Camp does not exists");

            var talk = _mapper.Map<Talk>(model);
            talk.Camp = camp;

            if (model.Speaker == null)
                return BadRequest("Dpeaker Id is required");

            var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
            if (speaker == null)
                return BadRequest("Speaker cound not be found");

            talk.Speaker = speaker;

            var location = _linkGenerator.GetPathByAction(this.HttpContext,
                "Get",
                values: new
                {
                    moniker,
                    id = talk.TalkId
                });

            try
            {
                _repository.Add(talk);
                if (await _repository.SaveChangesAsync())
                {
                    return Created(location, _mapper.Map<TalkModel>(talk));
                }
                else
                    return BadRequest("Failed to save new Talk");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel model)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null)
                    return NotFound("Couldn'find talk");

                _mapper.Map(model, talk);

                if (model.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null)
                        talk.Speaker = speaker;
                }

                if (await _repository.SaveChangesAsync())
                    return _mapper.Map<TalkModel>(talk);

                return BadRequest("Failed to update Database.");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get Talk");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null)
                    return NotFound("Couldn'find talk");

                _repository.Delete(talk);
                if (await _repository.SaveChangesAsync())
                    return this.Ok();

                return BadRequest("Failed to delete talk");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete Talk");
            }
        }
    }
}