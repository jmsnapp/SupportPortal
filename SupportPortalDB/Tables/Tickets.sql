CREATE TABLE [dbo].[Tickets]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [CustomerId] BIGINT NOT NULL, 
    [IntegrationId] BIGINT NOT NULL, 
    [Description] NVARCHAR(1023) NOT NULL, 
    [Reproduce] NVARCHAR(MAX) NULL, 
    [SeverityId] BIGINT NOT NULL, 
    [ReportedBy] NVARCHAR(63) NOT NULL, 
    [AssignedTo] NVARCHAR(63) NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [ResolutionDate] DATETIME NULL, 
    [Resolution] NVARCHAR(MAX) NULL, 
    [EsclationId] BIGINT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Tickets_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]), 
    CONSTRAINT [FK_Tickets_ToIntegration] FOREIGN KEY ([IntegrationId]) REFERENCES [dbo].[Integrations]([Id]), 
    CONSTRAINT [FK_Tickets_ToSeverity] FOREIGN KEY ([SeverityId]) REFERENCES [dbo].[Severities]([Id]), 
    CONSTRAINT [FK_Tickets_ToEscalation] FOREIGN KEY ([EsclationId]) REFERENCES [dbo].[Escalations]([Id])
)
