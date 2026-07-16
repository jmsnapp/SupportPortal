CREATE TABLE [dbo].[Escalations]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [TicketId] BIGINT NOT NULL DEFAULT 0, 
    [CreatedDate] DATETIME NOT NULL, 
    [ProblemSummary] NVARCHAR(MAX) NOT NULL, 
    [CustomerImpact] NVARCHAR(MAX) NULL, 
    [RootCause] NVARCHAR(MAX) NULL, 
    [RecommendedActions] NVARCHAR(MAX) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0
)
