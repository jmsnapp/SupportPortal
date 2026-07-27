CREATE TABLE [dbo].[IntegrationErrors]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [IntegrationId] BIGINT NOT NULL , 
    [ErrorMessage] NVARCHAR(1027) NOT NULL DEFAULT '', 
    [StackTrace] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [ErrorTime] DATETIME NOT NULL, 
    CONSTRAINT [FK_IntegrationErrors_ToIntegrations] FOREIGN KEY ([IntegrationId]) REFERENCES [dbo].[Integrations]([Id]), 
    CONSTRAINT [AK_IntegrationErrors_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_IntegrationErrors_Integration] ON [dbo].[IntegrationErrors] ([IntegrationId])

GO

CREATE INDEX [IX_IntegrationErrors_Deleted] ON [dbo].[IntegrationErrors] ([Deleted])
