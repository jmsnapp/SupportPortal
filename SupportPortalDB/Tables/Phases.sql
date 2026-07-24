CREATE TABLE [dbo].[Phases]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(127) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_Phase_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_Phases_Deleted] ON [dbo].[Phases] ([Deleted])
