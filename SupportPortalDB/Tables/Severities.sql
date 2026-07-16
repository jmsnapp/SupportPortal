CREATE TABLE [dbo].[Severities]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(31) NOT NULL, 
    [Description] NVARCHAR(63) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_Severities_Name] UNIQUE ([Name])
)
