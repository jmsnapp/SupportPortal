CREATE PROCEDURE [dbo].[uspGetAllProjects]
AS
	SELECT [dbo].[Projects].[Id]
		, [dbo].[Projects].[Name]
		, [dbo].[Projects].[Description]
		, [dbo].[Projects].[Deleted]
		, [dbo].[Projects].[RowVersion]
		, [dbo].[Projects].[CustomerId]
		, [dbo].[Projects].[CurrentPhase]
		, [dbo].[Projects].[TargetGoLiveDate]
		, [dbo].[Projects].[ActualGoLiveDate] 
		, [dbo].[Customers].[Description] AS 'CustomerDescription'
		, [dbo].[Phases].[Description] AS 'CurrentPhaseDescription'
	FROM [dbo].[Projects]
		LEFT JOIN [dbo].[Customers] ON [dbo].[Projects].[CustomerId] = [dbo].[Customers].[Id]
		LEFT JOIN [dbo].[Phases] ON [dbo].[Projects].[CurrentPhase] = [dbo].[Phases].[Id]
