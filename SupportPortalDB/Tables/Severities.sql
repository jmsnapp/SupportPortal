CREATE TABLE [dbo].[Severities]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [RowVersion] ROWVERSION NOT NULL, 
    CONSTRAINT [AK_Severities_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_Severities_Deleted] ON [dbo].[Severities] ([Deleted])
