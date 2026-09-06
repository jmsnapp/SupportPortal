CREATE PROCEDURE [dbo].[uspGetTicketById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Tickets] WHERE [Id] = @Id

	SELECT * FROM [dbo].[Severities] WHERE [Id] = 
		(SELECT [SeverityId] FROM [dbo].[Tickets] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[SupportStatuses] WHERE [Id] = 
		(SELECT [StatusId] FROM [dbo].[Tickets] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[Escalations] WHERE [Id] = 
		(SELECT [EscalationId] FROM [dbo].[Tickets] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[Customers] WHERE [Id] = 
		(SELECT [CustomerId] FROM [dbo].[Tickets] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[Industries] WHERE [Id] = 
		(SELECT [IndustryId] FROM [dbo].[Customers] WHERE [Id] = 
			(SELECT [CustomerId] FROM [dbo].[Tickets] WHERE [Id] = @Id))

	SELECT * FROM [dbo].[Integrations] WHERE [Id] = 
		(SELECT [IntegrationId] FROM [dbo].[Tickets] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[IntegrationStatuses] WHERE [Id] = 
		(SELECT [CurrentStatusId] FROM [dbo].[Integrations] WHERE [Id] = 
			(SELECT [IntegrationId] FROM [dbo].[Tickets] WHERE [Id] = @Id))

	SELECT * FROM [dbo].[IntegrationTypes] WHERE [Id] = 
		(SELECT [IntegrationTypeId] FROM [dbo].[Integrations] WHERE [Id] = 
			(SELECT [IntegrationId] FROM [dbo].[Tickets] WHERE [Id] = @Id))

	SELECT * FROM [dbo].[Customers] WHERE [Id] = 
		(SELECT [CustomerId] FROM [dbo].[Integrations] WHERE [Id] = 
			(SELECT [IntegrationId] FROM [dbo].[Tickets] WHERE [Id] = @Id))

	SELECT * FROM [dbo].[Industries] WHERE [Id] = 
		(SELECT [IndustryId] FROM [dbo].[Customers] WHERE [Id] = 
			(SELECT [CustomerId] FROM [dbo].[Integrations] WHERE [Id] = 
				(SELECT [IntegrationId] FROM [dbo].[Tickets] WHERE [Id] = @Id)))

	SELECT * FROM [dbo].[TicketNotes] WHERE [Deleted] = 0 AND [TicketId] = @Id
