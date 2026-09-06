CREATE PROCEDURE [dbo].[uspGetIntegrationById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Integrations] WHERE [Id] = @Id

	SELECT * FROM [dbo].[IntegrationTypes] WHERE [Id] = 
		(SELECT [IntegrationTypeId] FROM [dbo].[Integrations]WHERE [Id] = @Id)

	SELECT * FROM [dbo].[IntegrationStatuses] WHERE [Id] = 
		(SELECT [CurrentStatusId] FROM [dbo].[Integrations] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[Customers] WHERE [Id] = 
		(SELECT [CustomerId] FROM [dbo].[Integrations] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[Industries] WHERE [Id] = 
		(SELECT [IndustryId] FROM [dbo].[Customers] WHERE [Id] = 
			(SELECT [CustomerId] FROM [dbo].[Integrations] WHERE [Id] = @Id))
