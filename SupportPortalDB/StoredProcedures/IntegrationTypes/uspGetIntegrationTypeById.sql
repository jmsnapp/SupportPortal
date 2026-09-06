CREATE PROCEDURE [dbo].[uspGetIntegrationTypeById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[IntegrationTypes] WHERE [Id] = @Id
