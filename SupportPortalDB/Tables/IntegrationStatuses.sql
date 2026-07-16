CREATE TABLE [dbo].[IntegrationStatuses]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(63) NOT NULL, 
    [Description] NVARCHAR(127) NOT NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_IntegrationStatuses_Name] UNIQUE ([Name])
)
