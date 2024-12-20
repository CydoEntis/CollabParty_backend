using AutoMapper;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class PartyMemberService : IPartyMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PartyMember> _logger;
    private readonly IMapper _mapper;

    public PartyMemberService(IUnitOfWork unitOfWork, ILogger<PartyMember> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result> AddPartyMember(string userId, int partyId)
    {
        if (string.IsNullOrEmpty(userId))
            return Result.Failure("User Id is required.");

        if (partyId <= 0)
            return Result.Failure("Party Id is required.");

        try
        {
            PartyMember newUserParty = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                Role = UserRole.Member
            };

            await _unitOfWork.PartyMember.CreateAsync(newUserParty);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed add member to party");
            return Result.Failure("An error occurred while add member to party");
        }
    }

    public async Task<Result> AddPartyLeader(string userId, int partyId)
    {
        if (string.IsNullOrEmpty(userId))
            return Result.Failure("User Id is required.");

        if (partyId <= 0)
            return Result.Failure("Party Id is required.");

        try
        {
            PartyMember newUserParty = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                Role = UserRole.Leader
            };

            await _unitOfWork.PartyMember.CreateAsync(newUserParty);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party leader.");
            return Result.Failure("An error occurred while adding user as party leader.");
        }
    }

    public async Task<Result<List<PartyMemberResponseDto>>> GetPartyMembers(string userId, int partyId)
    {
        try
        {
            // Fetch the user’s membership in the given party (without ExistsAsync)
            var userPartyMember = await _unitOfWork.PartyMember.GetAsync(
                pm => pm.PartyId == partyId && pm.UserId == userId,
                includeProperties: "User,User.UnlockedAvatars,User.UnlockedAvatars.Avatar");

            // If the user is not part of the party, return an error
            if (userPartyMember == null)
            {
                return Result<List<PartyMemberResponseDto>>.Failure(
                    "You are not a member of this party or the party does not exist.");
            }

            // Fetch all party members for the given party
            var partyMembers = await _unitOfWork.PartyMember.GetAllAsync(
                pm => pm.PartyId == partyId,
                includeProperties: "User,User.UnlockedAvatars,User.UnlockedAvatars.Avatar");

            // If no members are found, return an error
            if (!partyMembers.Any())
            {
                return Result<List<PartyMemberResponseDto>>.Failure(
                    $"No members found for party with ID {partyId}.");
            }

            // Map the party members to DTOs
            var partyMemberDtos = _mapper.Map<List<PartyMemberResponseDto>>(partyMembers);
            return Result<List<PartyMemberResponseDto>>.Success(partyMemberDtos);
        }
        catch (Exception ex)
        {
            // Log the exception and return a failure result
            _logger.LogError(ex, "Failed to get party members for party {PartyId}.", partyId);
            return Result<List<PartyMemberResponseDto>>.Failure("An error occurred while fetching party members.");
        }
    }



    public async Task<Result<List<PartyMemberResponseDto>>> RemovePartyMembers(string userId, int partyId,
        RemoverUserFromPartyDto dto)
    {
        try
        {
            var foundParty = await _unitOfWork.PartyMember.GetAsync(
                up => up.PartyId == partyId && up.UserId == userId,
                includeProperties: "PartyMembers");

            if (foundParty == null)
                return Result<List<PartyMemberResponseDto>>.Failure($"No party with the {dto.PartyId} exists");

            if (foundParty.Role == UserRole.Member)
                return Result<List<PartyMemberResponseDto>>.Failure(
                    "User must have a role of Leader or Captain to remove members");

            var usersToRemove = await _unitOfWork.PartyMember.GetAllAsync(
                up => up.PartyId == dto.PartyId && dto.UserIds.Contains(up.UserId));

            var usersToRemoveList = usersToRemove.ToList();

            if (!usersToRemoveList.Any())
                return Result<List<PartyMemberResponseDto>>.Failure("Users to remove could not be found");

            await _unitOfWork.PartyMember.RemoveUsersAsync(usersToRemoveList);

            var memberDtos = _mapper.Map<List<PartyMemberResponseDto>>(usersToRemoveList);
            return Result<List<PartyMemberResponseDto>>.Success(memberDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party members");
            return Result<List<PartyMemberResponseDto>>.Failure("An error occurred while fetching party members.");
        }
    }

    public async Task<Result<List<PartyMemberResponseDto>>> UpdatePartyMemberRoles(string userId, int partyId,
        UpdatePartyMembersRoleDto dto)
    {
        try
        {
            var foundParty = await _unitOfWork.PartyMember.GetAsync(
                up => up.PartyId == partyId && up.UserId == userId);


            if (foundParty == null)
                return Result<List<PartyMemberResponseDto>>.Failure($"No party with ID {partyId} exists");

            if (foundParty.Role == UserRole.Member)
                return Result<List<PartyMemberResponseDto>>.Failure(
                    "User must have a role of Leader or Captain to update member roles");

            var allPartyMembers = await _unitOfWork.PartyMember
                .GetAllAsync(up => up.PartyId == partyId);

            var usersToUpdate = allPartyMembers
                .Where(up => dto.NewRoles.Any(nr => nr.UserId == up.UserId))
                .ToList();

            if (!usersToUpdate.Any())
                return Result<List<PartyMemberResponseDto>>.Failure("No matching users found to update roles");

            foreach (var user in usersToUpdate)
            {
                var newRole = dto.NewRoles.First(nr => nr.UserId == user.UserId).Role;
                user.Role = newRole;
            }

            await _unitOfWork.PartyMember.UpdateUsersAsync(usersToUpdate);

            var PartyMemberResponseDtos = _mapper.Map<List<PartyMemberResponseDto>>(usersToUpdate);
            return Result<List<PartyMemberResponseDto>>.Success(PartyMemberResponseDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update party member roles");
            return Result<List<PartyMemberResponseDto>>.Failure("An error occurred while updating member roles.");
        }
    }

    public async Task<Result> LeaveParty(string userId, int partyId)
    {
        try
        {
            var foundParty = await _unitOfWork.PartyMember.GetAsync(
                up => up.PartyId == partyId && up.UserId == userId);

            if (foundParty == null)
                return Result.Failure($"No party with ID {partyId} exists");


            await _unitOfWork.PartyMember.RemoveAsync(foundParty);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update party member roles");
            return Result.Failure("An error occurred while updating member roles.");
        }
    }
}