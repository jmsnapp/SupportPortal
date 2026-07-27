CREATE TABLE [dbo].[Industries]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_Industries_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_Industries_Deleted] ON [dbo].[Industries] ([Deleted])
