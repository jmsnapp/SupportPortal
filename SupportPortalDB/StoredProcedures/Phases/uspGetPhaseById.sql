CREATE PROCEDURE [dbo].[uspGetPhaseById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Phases] WHERE [Id] = @Id
