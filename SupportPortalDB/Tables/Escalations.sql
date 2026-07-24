CREATE TABLE [dbo].[Escalations]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(253) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [CreatedDate] DATETIME NOT NULL, 
    [ProblemSummary] NVARCHAR(MAX) NOT NULL, 
    [CustomerImpact] NVARCHAR(MAX) NULL, 
    [RootCause] NVARCHAR(MAX) NULL, 
    [RecommendedActions] NVARCHAR(MAX) NULL, 
    CONSTRAINT [AK_Escalations_Name] UNIQUE ([Name]) 
)

GO

CREATE INDEX [IX_Escalations_Deleted] ON [dbo].[Escalations] ([Deleted])

GO
