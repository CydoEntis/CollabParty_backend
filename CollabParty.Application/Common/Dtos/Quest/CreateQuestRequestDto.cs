using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Quest;

public class CreateQuestRequestDto
{
    public int PartyId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public PriorityLevelOption PriorityLevel { get; set; }
    public string[] Steps { get; set; }
    public string[] PartyMembers { get; set; }
}