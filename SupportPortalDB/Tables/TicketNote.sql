CREATE TABLE [dbo].[TicketNote]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [TicketId] BIGINT NOT NULL, 
    [Note] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [CreateTime] DATETIME NOT NULL DEFAULT '01/01/1900 00:00:00.000', 
    CONSTRAINT [AK_TicketNote_Name] UNIQUE ([Name]), 
    CONSTRAINT [FK_TicketNote_ToTicket] FOREIGN KEY ([TicketId]) REFERENCES [Tickets]([Id])
)

GO

CREATE INDEX [IX_TicketNote_Ticket] ON [dbo].[TicketNote] ([TicketId])

GO

CREATE INDEX [IX_TicketNote_Deleted] ON [dbo].[TicketNote] ([Deleted])
