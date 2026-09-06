CREATE PROCEDURE [dbo].[uspGetSupportStatusById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[SupportStatuses] WHERE [Id] = @Id
