CREATE TABLE [dbo].[Projects]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [CustomerId] BIGINT NOT NULL, 
    [Name] NVARCHAR(127) NOT NULL, 
    [CurrentPhase] BIGINT NOT NULL, 
    [TargetGoLiveDate] DATETIME NOT NULL, 
    [ActualGoLiveDate] DATETIME NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Projects_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([Id]), 
    CONSTRAINT [FK_Projects_ToPhase] FOREIGN KEY ([CurrentPhase]) REFERENCES [dbo].[Phases]([Id]), 
    CONSTRAINT [AK_Projects_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_Projects_Customer] ON [dbo].[Projects] ([CustomerId])

GO

CREATE INDEX [IX_Projects_Phase] ON [dbo].[Projects] ([CurrentPhase])
