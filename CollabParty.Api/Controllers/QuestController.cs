using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/quests")]
[ApiController]
[Authorize]
public class QuestController : ControllerBase
{
    private readonly IQuestService _questService;

    public QuestController(IQuestService questService)
    {
        _questService = questService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateQuest([FromBody] CreateQuestRequestDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questService.CreateQuest(userId, dto);

            if (result.IsSuccess)
                return Ok(ApiResponse.Success(result.Message));

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return
                ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
        }
    }
}