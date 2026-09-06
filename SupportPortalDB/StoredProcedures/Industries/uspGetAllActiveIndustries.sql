CREATE PROCEDURE [dbo].[uspGetAllActiveIndustries]
AS
	SELECT * FROM [dbo].[Industries] WHERE [Deleted] = 0
