CREATE TABLE [dbo].[IntegrationTypes]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [AK_IntegrationTypes_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_IntegrationTypes_Deleted] ON [dbo].[IntegrationTypes] ([Deleted])
