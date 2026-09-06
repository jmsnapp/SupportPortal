CREATE PROCEDURE [dbo].[uspGetIntegrationByName]
	@Name NVARCHAR(63)
AS
	SELECT * FROM [dbo].[Integrations] WHERE [Name] = @Name

	SELECT * FROM [dbo].[IntegrationTypes] WHERE [Id] = 
		(SELECT [IntegrationTypeId] FROM [dbo].[Integrations]WHERE [Name] = @Name)

	SELECT * FROM [dbo].[IntegrationStatuses] WHERE [Id] = 
		(SELECT [CurrentStatusId] FROM [dbo].[Integrations] WHERE [Name] = @Name)

	SELECT * FROM [dbo].[Customers] WHERE [Id] = 
		(SELECT [CustomerId] FROM [dbo].[Integrations] WHERE [Name] = @Name)

	SELECT * FROM [dbo].[Industries] WHERE [Id] = 
		(SELECT [IndustryId] FROM [dbo].[Customers] WHERE [Id] = 
			(SELECT [CustomerId] FROM [dbo].[Integrations] WHERE [Name] = @Name))
