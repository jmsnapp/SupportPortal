CREATE TABLE [dbo].[ProjectNotes]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ProjectId] NCHAR(10) NULL, 
    [Note] NVARCHAR(MAX) NULL, 
    [CreateTime] DATETIME NOT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0
)

GO

CREATE INDEX [IX_ProjectNotes_Project] ON [dbo].[ProjectNotes] ([ProjectId])
