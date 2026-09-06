CREATE PROCEDURE [dbo].[uspGetAllCustomers]
AS
	SELECT [dbo].[Customers].[Id]
	, [dbo].[Customers].[Name]
	, [dbo].[Customers].[Description]
	, [dbo].[Customers].[Deleted]
	, [dbo].[Customers].[RowVersion]
	, [dbo].[Customers].[IndustryId]
	, [dbo].[Industries].[Description] AS 'IndustryDescription'
	, [dbo].[Customers].[PrimaryContactName]
	, [dbo].[Customers].[PrimaryContactEmail]
	, [dbo].[Customers].[TechnicalContactName]
	, [dbo].[Customers].[TechnicalContactEmail]
	, [dbo].[Customers].[CreatedDate]
	FROM [dbo].[Customers]
		LEFT JOIN [dbo].[Industries] ON [dbo].[Customers].[IndustryId] = [dbo].[Industries].[Id]

