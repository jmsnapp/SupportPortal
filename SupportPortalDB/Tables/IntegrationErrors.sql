CREATE TABLE [dbo].[IntegrationErrors]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [IntegrationId] BIGINT NOT NULL, 
    [ErrorMessage] NVARCHAR(1027) NULL, 
    [StackTrace] NVARCHAR(MAX) NULL, 
    [ErrorTime] DATETIME NOT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0
)

GO

CREATE INDEX [IX_IntegrationErrors_Integration] ON [dbo].[IntegrationErrors] ([IntegrationId])
