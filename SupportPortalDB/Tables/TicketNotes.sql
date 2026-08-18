CREATE TABLE [dbo].[TicketNotes]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [TicketId] BIGINT NOT NULL, 
    [Note] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [RowVersion] ROWVERSION NOT NULL, 
    [CreateTime] DATETIME NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_TicketNote_Name] UNIQUE ([Name]), 
    CONSTRAINT [FK_TicketNote_ToTicket] FOREIGN KEY ([TicketId]) REFERENCES [Tickets]([Id])
)

GO

CREATE INDEX [IX_TicketNote_Ticket] ON [dbo].[TicketNotes] ([TicketId])

GO

CREATE INDEX [IX_TicketNote_Deleted] ON [dbo].[TicketNotes] ([Deleted])
