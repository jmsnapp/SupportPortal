CREATE PROCEDURE [dbo].[uspGetIntegrationStatusById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[IntegrationStatuses] WHERE [Id] = @Id
