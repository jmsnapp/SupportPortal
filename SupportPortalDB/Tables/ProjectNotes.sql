CREATE TABLE [dbo].[ProjectNotes]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [ProjectId] BIGINT NOT NULL, 
    [Note] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [CreateTime] DATETIME NOT NULL DEFAULT '01/01/1900 00:00:00.000', 
    CONSTRAINT [AK_ProjectNotes_Name] UNIQUE ([Name]), 
    CONSTRAINT [FK_ProjectNotes_ToProject] FOREIGN KEY ([ProjectId]) REFERENCES [Projects]([Id]), 
)

GO

CREATE INDEX [IX_ProjectNotes_Project] ON [dbo].[ProjectNotes] ([ProjectId])

GO

CREATE INDEX [IX_ProjectNotes_Deleted] ON [dbo].[ProjectNotes] ([Deleted])
