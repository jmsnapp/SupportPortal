CREATE TABLE [dbo].[Escalations]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Description] NVARCHAR(255) NOT NULL  , 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [RowVersion] ROWVERSION NOT NULL, 
    [CreatedDate] DATETIME NOT NULL DEFAULT 0, 
    [ProblemSummary] NVARCHAR(MAX) NOT NULL, 
    [CustomerImpact] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [RootCause] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [RecommendedActions] NVARCHAR(MAX) NOT NULL DEFAULT ''
)

GO

CREATE INDEX [IX_Escalations_Deleted] ON [dbo].[Escalations] ([Deleted])

GO
