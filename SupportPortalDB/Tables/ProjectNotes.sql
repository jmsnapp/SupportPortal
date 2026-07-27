CREATE TABLE [dbo].[ProjectNotes]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [ProjectId] BIGINT NOT NULL, 
    [Note] NVARCHAR(MAX) NULL, 
    [CreateTime] DATETIME NOT NULL, 
    CONSTRAINT [AK_ProjectNotes_Name] UNIQUE ([Name]), 
    CONSTRAINT [FK_ProjectNotes_ToProject] FOREIGN KEY ([ProjectId]) REFERENCES [Projects]([Id]), 
)

GO

CREATE INDEX [IX_ProjectNotes_Project] ON [dbo].[ProjectNotes] ([ProjectId])

GO

CREATE INDEX [IX_ProjectNotes_Deleted] ON [dbo].[ProjectNotes] ([Deleted])
