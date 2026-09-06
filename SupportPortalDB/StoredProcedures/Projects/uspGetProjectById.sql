CREATE PROCEDURE [dbo].[uspGetProjectById]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Projects] WHERE [Id] = @Id

	SELECT * FROM [dbo].[Phases] WHERE [Id] = 
		(SELECT [CurrentPhase] FROM [dbo].[Projects] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[Customers] WHERE [Id] = 
		(SELECT [CustomerId] FROM [dbo].[Projects] WHERE [Id] = @Id)

	SELECT * FROM [dbo].[Industries] WHERE [Id] = 
		(SELECT [IndustryId] FROM [dbo].[Customers] WHERE [Id] = 
			(SELECT[CustomerId] FROM [dbo].[Projects] WHERE [Id] = @Id))

	SELECT * FROM [dbo].[ProjectNotes] WHERE [ProjectId] = @Id AND [Deleted] = 0
