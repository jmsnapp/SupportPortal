CREATE TABLE [dbo].[Integrations]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(127) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [CustomerId] BIGINT NOT NULL, 
    [IntegrationTypeId] BIGINT NOT NULL, 
    [CurrentStatusId] BIGINT NOT NULL, 
    [LastSuccessfulSync] DATETIME NULL, 
    [LastFailedSync] DATETIME NULL, 
    [RetryCount] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Integrations_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]), 
    CONSTRAINT [FK_Integrations_ToStatus] FOREIGN KEY ([CurrentStatusId]) REFERENCES [dbo].[IntegrationStatuses]([Id]), 
    CONSTRAINT [FK_Integrations_ToType] FOREIGN KEY ([IntegrationTypeId]) REFERENCES [dbo].[IntegrationTypes]([Id]), 
    CONSTRAINT [AK_Integrations_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_Integrations_Customer] ON [dbo].[Integrations] ([CustomerId])

GO

CREATE INDEX [IX_Integrations_IntegrationType] ON [dbo].[Integrations] ([IntegrationTypeId])

GO

CREATE INDEX [IX_Integrations_Status] ON [dbo].[Integrations] ([CurrentStatusId])
