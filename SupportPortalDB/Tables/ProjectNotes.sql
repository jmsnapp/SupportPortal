CREATE TABLE [dbo].[ProjectNotes]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(253) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [ProjectId] BIGINT NULL, 
    [Note] NVARCHAR(MAX) NULL, 
    [CreateTime] DATETIME NOT NULL, 
    CONSTRAINT [AK_ProjectNotes_Name] UNIQUE ([Name]), 
)

GO

CREATE INDEX [IX_ProjectNotes_Project] ON [dbo].[ProjectNotes] ([ProjectId])

GO

CREATE INDEX [IX_ProjectNotes_Deleted] ON [dbo].[ProjectNotes] ([Deleted])
