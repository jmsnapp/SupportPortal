CREATE TABLE [dbo].[Tickets]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(1023) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [CustomerId] BIGINT NOT NULL, 
    [IntegrationId] BIGINT NOT NULL, 
    [Reproduce] NVARCHAR(MAX) NULL, 
    [SeverityId] BIGINT NOT NULL, 
    [ReportedBy] NVARCHAR(63) NOT NULL, 
    [AssignedTo] NVARCHAR(63) NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [ResolutionDate] DATETIME NULL, 
    [Resolution] NVARCHAR(MAX) NULL, 
    [EscalationId] BIGINT NULL, 
    CONSTRAINT [FK_Tickets_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]), 
    CONSTRAINT [FK_Tickets_ToIntegration] FOREIGN KEY ([IntegrationId]) REFERENCES [dbo].[Integrations]([Id]), 
    CONSTRAINT [FK_Tickets_ToSeverity] FOREIGN KEY ([SeverityId]) REFERENCES [dbo].[Severities]([Id]), 
    CONSTRAINT [FK_Tickets_ToEscalation] FOREIGN KEY ([EscalationId]) REFERENCES [dbo].[Escalations]([Id]), 
    CONSTRAINT [AK_Tickets_Name] UNIQUE ([Name])
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
