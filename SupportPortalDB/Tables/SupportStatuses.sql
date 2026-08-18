CREATE TABLE [dbo].[SupportStatuses]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_SupportStatuses_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_SupportStatuses_Deleted] ON [dbo].[SupportStatuses] ([Deleted])
