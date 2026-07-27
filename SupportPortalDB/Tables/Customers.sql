CREATE TABLE [dbo].[Customers]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(255) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [IndustryId] BIGINT NOT NULL DEFAULT 0, 
    [PrimaryContactName] NVARCHAR(63) NOT NULL, 
    [PrimaryContactEmail] NVARCHAR(63) NOT NULL, 
    [TechnicalContactName] NVARCHAR(63) NOT NULL, 
    [TechnicalContactEmail] NVARCHAR(63) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    CONSTRAINT [FK_Customers_Industries] FOREIGN KEY ([IndustryId]) REFERENCES [dbo].[Industries]([Id]), 
    CONSTRAINT [AK_Customers_Name] UNIQUE ([Name])
)

GO

CREATE INDEX [IX_Customers_Deleted] ON [dbo].[Customers] ([Deleted])

GO

CREATE INDEX [IX_Customers_Industry] ON [dbo].[Customers] ([IndustryId])
