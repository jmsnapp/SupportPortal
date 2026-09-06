CREATE PROCEDURE [dbo].[uspGetProjectPhasesByProjectId]
	@ProjectId BIGINT
AS
	SELECT [dbo].[LinkProjectPhases].[Id]
	, [dbo].[LinkProjectPhases].[Description]
	, [dbo].[LinkProjectPhases].[Deleted]
	, [dbo].[LinkProjectPhases].[RowVersion]
	, [dbo].[LinkProjectPhases].[ProjectId]
	, [dbo].[LinkProjectPhases].[PhaseId]
	, [dbo].[LinkProjectPhases].[Percentage]
	, [dbo].[LinkProjectPhases].[Order] 
	, [dbo].[Projects].[Description] AS 'ProjectDescription'
	, [dbo].[Phases].[Description] AS 'PhaseDescription'
	FROM [dbo].[LinkProjectPhases]
		LEFT JOIN [dbo].[Projects] ON [dbo].[LinkProjectPhases].[ProjectId] = [dbo].[Projects].[Id]
		LEFT JOIN [dbo].[Phases] ON [dbo].[LinkProjectPhases].[PhaseId] = [dbo].[Phases].[Id]
	WHERE [dbo].[LinkProjectPhases].[ProjectId] = @ProjectId
