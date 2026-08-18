CREATE TABLE [dbo].[LinkProjectPhases]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [RowVersion] ROWVERSION NOT NULL, 
    [ProjectId] BIGINT NOT NULL, 
    [PhaseId] BIGINT NOT NULL, 
    [Percentage] DECIMAL(5,2) NOT NULL DEFAULT 0, 
    [Order] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_LinkProjectPhase_ToProject] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects]([Id]), 
    CONSTRAINT [FK_LinkProjectPhase_ToPhase] FOREIGN KEY ([PhaseId]) REFERENCES [dbo].[Phases]([Id]), 
    CONSTRAINT [AK_LinkProjectPhase_Name] UNIQUE ([Name]),
    CONSTRAINT [AK_LinkProjectPhase_ProjectPhase] UNIQUE ([ProjectId], [PhaseId])
)

GO

CREATE INDEX [IX_LinkProjectPhase_ProjectId] ON [dbo].[LinkProjectPhases] ([ProjectId])

GO

CREATE INDEX [IX_LinkProjectPhase_Phase] ON [dbo].[LinkProjectPhases] ([PhaseId])

GO

CREATE INDEX [IX_LinkProjectPhase_Deleted] ON [dbo].[LinkProjectPhases] ([Deleted])
