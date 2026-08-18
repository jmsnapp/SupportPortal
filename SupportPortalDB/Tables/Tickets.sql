CREATE TABLE [dbo].[Tickets]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(1023) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [RowVersion] ROWVERSION NOT NULL, 
    [CustomerId] BIGINT NOT NULL, 
    [IntegrationId] BIGINT NOT NULL, 
    [Reproduce] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [SeverityId] BIGINT NOT NULL, 
    [StatusId] BIGINT NOT NULL,
    [ReportedBy] NVARCHAR(63) NOT NULL DEFAULT '', 
    [AssignedTo] NVARCHAR(63) NOT NULL DEFAULT '', 
    [CreatedDate] DATETIME NOT NULL DEFAULT 0, 
    [ResolutionDate] DATETIME NOT NULL DEFAULT 0, 
    [Resolution] NVARCHAR(MAX) NOT NULL DEFAULT '', 
    [EscalationId] BIGINT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Tickets_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]), 
    CONSTRAINT [FK_Tickets_ToIntegration] FOREIGN KEY ([IntegrationId]) REFERENCES [dbo].[Integrations]([Id]), 
    CONSTRAINT [FK_Tickets_ToSeverity] FOREIGN KEY ([SeverityId]) REFERENCES [dbo].[Severities]([Id]), 
    CONSTRAINT [FK_Tickets_ToEscalation] FOREIGN KEY ([EscalationId]) REFERENCES [dbo].[Escalations]([Id]), 
    CONSTRAINT [AK_Tickets_Name] UNIQUE ([Name]), 
    CONSTRAINT [FK_Tickets_ToStatus] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[SupportStatuses]([Id])
)

GO

CREATE INDEX [IX_Tickets_Deleted] ON [dbo].[Tickets] ([Deleted])

GO

CREATE INDEX [IX_Tickets_Integration] ON [dbo].[Tickets] ([IntegrationId])

GO

CREATE INDEX [IX_Tickets_Customer] ON [dbo].[Tickets] ([CustomerId])

GO

CREATE INDEX [IX_Tickets_Severity] ON [dbo].[Tickets] ([SeverityId])

GO

CREATE INDEX [IX_Tickets_Escalation] ON [dbo].[Tickets] ([EscalationId])

GO

CREATE INDEX [IX_Tickets_Status] ON [dbo].[Tickets] ([StatusId])
