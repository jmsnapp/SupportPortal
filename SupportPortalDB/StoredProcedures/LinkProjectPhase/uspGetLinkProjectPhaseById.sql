CREATE PROCEDURE [dbo].[uspGetLinkProjectPhaseById]
	@Id BIGINT
AS
	SELECT [dbo].[LinkProjectPhases].[Id]
		, [dbo].[LinkProjectPhases].[Description]
		, [dbo].[LinkProjectPhases].[Deleted]
		, [dbo].[LinkProjectPhases].[RowVersion]
		, [dbo].[LinkProjectPhases].[ProjectId]
		, [dbo].[LinkProjectPhases].[PhaseId]
		, [dbo].[LinkProjectPhases].[Percentage]
		, [dbo].[LinkProjectPhases].[Order]
		, [dbo].[Phases].[Id] AS 'ChildPhaseId'
		, [dbo].[Phases].[Name] AS 'ChildPhaseName'
		, [dbo].[Phases].[Description] AS 'ChildDescription'
		, [dbo].[Phases].[Deleted] AS 'ChildPhaseDeleted'
	FROM [dbo].[LinkProjectPhases] 
		LEFT JOIN [dbo].[Phases] ON [dbo].[LinkProjectPhases].[PhaseId] = [dbo].[Phases].[Id]
	WHERE [dbo].[LinkProjectPhases].[Id] = @Id
