CREATE PROCEDURE [dbo].[uspGetIndustryById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Industries] WHERE [Id] = @Id
