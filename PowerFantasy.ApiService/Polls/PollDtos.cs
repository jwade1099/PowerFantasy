namespace PowerFantasy.ApiService.Polls;

public record TeamDto( Guid Id, string TeamName, string ManagerName, string? AvatarUrl, int Wins, int Losses, int Ties );

public record PollDto( Guid Id, string Season, int Week, DateTimeOffset CreatedAt, List<TeamDto> Teams, int VotesSubmitted, int TotalVoters );

public record SubmitVoteRequest( Guid VoterTeamId, List<Guid> RankedTeamIds );

public record PollResultDto( Guid TeamId, string TeamName, string ManagerName, string? AvatarUrl, int Wins, int Losses, int Ties, double AverageRank, int VoteCount );

public record PollResultsDto( Guid PollId, string Season, int Week, int VotesSubmitted, int TotalVoters, List<PollResultDto> Rankings );

public record PollSummaryDto( Guid Id, string Season, int Week, DateTimeOffset CreatedAt, int VotesSubmitted, int TotalVoters );

public enum SubmitVoteResult { Success, PollNotFound, InvalidVoter, InvalidBallot }
