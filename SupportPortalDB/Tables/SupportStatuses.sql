CREATE TABLE [dbo].[SupportStatuses]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(127) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_SupportStatuses_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_SupportStatuses_Deleted] ON [dbo].[SupportStatuses] ([Deleted])
