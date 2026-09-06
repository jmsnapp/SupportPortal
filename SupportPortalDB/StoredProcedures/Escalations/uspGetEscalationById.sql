CREATE PROCEDURE [dbo].[uspGetEscalationById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Escalations] WHERE [Id] = @Id
