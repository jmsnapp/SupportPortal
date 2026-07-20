CREATE TABLE [dbo].[Customers]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [IndustryId] BIGINT NOT NULL DEFAULT 0, 
    [PrimaryContactName] NVARCHAR(63) NOT NULL, 
    [PrimaryContactEmail] NVARCHAR(63) NOT NULL, 
    [TechnicalContactName] NVARCHAR(63) NOT NULL, 
    [TechnicalContactEmail] NVARCHAR(63) NOT NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    [Description] NVARCHAR(127) NOT NULL, 
    CONSTRAINT [FK_Customers_Industries] FOREIGN KEY ([IndustryId]) REFERENCES [dbo].[Industries]([Id]), 
    CONSTRAINT [AK_Customers_Name] UNIQUE ([Name])
)
