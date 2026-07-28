CREATE TABLE [dbo].[Projects]
(
	[Id] BIGINT NOT NULL IDENTITY(0,1) PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NOT NULL DEFAULT '', 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [CustomerId] BIGINT NOT NULL, 
    [CurrentPhase] BIGINT NOT NULL, 
    [TargetGoLiveDate] DATETIME NOT NULL DEFAULT '01/01/1900 00:00:00.000', 
    [ActualGoLiveDate] DATETIME NOT NULL DEFAULT '01/01/1900 00:00:00.000', 
    CONSTRAINT [FK_Projects_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]), 
    CONSTRAINT [FK_Projects_ToPhase] FOREIGN KEY ([CurrentPhase]) REFERENCES [dbo].[Phases]([Id]), 
    CONSTRAINT [AK_Projects_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_Projects_Customer] ON [dbo].[Projects] ([CustomerId])

GO

CREATE INDEX [IX_Projects_Phase] ON [dbo].[Projects] ([CurrentPhase])

GO

CREATE INDEX [IX_Projects_Deleted] ON [dbo].[Projects] ([Deleted])
