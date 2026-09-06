CREATE PROCEDURE [dbo].[uspGetAllActiveSupportStatuses]
AS
	SELECT * FROM [dbo].[SupportStatuses] WHERE [Deleted] = 0
