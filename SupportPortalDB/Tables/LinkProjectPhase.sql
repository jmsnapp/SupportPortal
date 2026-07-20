CREATE TABLE [dbo].[LinkProjectPhase]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [ProjectId] BIGINT NOT NULL, 
    [PhaseId] BIGINT NOT NULL, 
    [Percentage] DECIMAL NULL, 
    [Order] INT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_LinkProjectPhase_ToProject] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects]([Id]), 
    CONSTRAINT [FK_LinkProjectPhase_ToPhase] FOREIGN KEY ([PhaseId]) REFERENCES [dbo].[Phases]([Id]), 
    CONSTRAINT [AK_LinkProjectPhase_ProjectPhase] UNIQUE ([ProjectId], [PhaseId])
)

GO

CREATE INDEX [IX_LinkProjectPhase_ProjectId] ON [dbo].[LinkProjectPhase] ([ProjectId])

GO

CREATE INDEX [IX_LinkProjectPhase_Phase] ON [dbo].[LinkProjectPhase] ([PhaseId])
