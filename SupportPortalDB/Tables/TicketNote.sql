CREATE TABLE [dbo].[TicketNote]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [TicketId] BIGINT NOT NULL, 
    [Note] NVARCHAR(MAX) NOT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0
)

GO

CREATE INDEX [IX_TicketNote_Ticket] ON [dbo].[TicketNote] ([TicketId])
