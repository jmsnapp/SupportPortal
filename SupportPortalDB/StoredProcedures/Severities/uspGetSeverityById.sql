CREATE PROCEDURE [dbo].[uspGetSeverityById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Severities] WHERE [Id] = @Id
