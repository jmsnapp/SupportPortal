CREATE TABLE [dbo].[TicketNote]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(253) NULL, 
    [TicketId] BIGINT NOT NULL, 
    [Note] NVARCHAR(MAX) NOT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_TicketNote_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_TicketNote_Ticket] ON [dbo].[TicketNote] ([TicketId])

GO

CREATE INDEX [IX_TicketNote_Deleted] ON [dbo].[TicketNote] ([Deleted])
