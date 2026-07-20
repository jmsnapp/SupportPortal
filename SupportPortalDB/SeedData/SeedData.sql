-- Base tables
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('DEFAULT', 'Default');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('AVIATION', 'Aviation');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('CONGLOMERATE', 'Conglomerate');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('FINANCIAL', 'Financial');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('FOOD_SERVICE', 'Food Services');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('MANUFACTURING', 'Manufacturing');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('MEDIA', 'Media');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('TECHNOLOGY', 'Technology');

INSERT INTO [dbo].[IntegrationStatuses]([Name], [Description]) VALUES ('DEFAULT', 'Default');
INSERT INTO [dbo].[IntegrationStatuses]([Name], [Description]) VALUES ('OPERATIONAL', 'Operational');
INSERT INTO [dbo].[IntegrationStatuses]([Name], [Description]) VALUES ('OUT_OF_SERVICE', 'Out of Service');
INSERT INTO [dbo].[IntegrationStatuses]([Name], [Description]) VALUES ('STAGING', 'Staging');

INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('DEFAULT', 'Default');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('API', 'Application Programming Interface');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('APPLICATION', 'Application');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('FTP_FILE_DROP', 'FTP File Drop');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('NETWORK_FILE_DROP', 'Network File Drop');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('SCHEDULED_JOB', 'Scheduled Job');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('SQLJOB', 'SQL Job');

INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('DEFAULT', 'Default');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('BUS_REQ_DISCOVERY', 'Business Requirements Discovery');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('TECH_REQ_DISCOVERY', 'Technical Requirements Discovery');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('DEVELOPMENT', 'Development');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('TESTING', 'Testing');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('DEPLOYMENT', 'Deployment');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('MAINTENANCE', 'Maintenance');

INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('DEFAULT', 'Default');
INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('LOW', 'Low');
INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('MEDIUM', 'Medium');
INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('HIGH', 'High');
INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('CRITICAL', 'Critical');

INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('DEFAULT', 'Default');
INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('OPEN', 'Open');
INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('IN_PROGRESS', 'In Progress');
INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('RESOLVED', 'Resolved');
INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('CLOSED', 'Closed');

-- Secondary Tables
INSERT INTO [dbo].[Customers]([Name], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate], [Description])
	VALUES ('DEFAULT', 0, 'Default', 'default@noemail.com', 'Default', 'default@noemail.com', GETDATE(), 'Default');
INSERT INTO [dbo].[Customers]([Name], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate], [Description])
	VALUES ('LEXCORP', 
	(SELECT [Id] FROM [dbo].[Industries] WHERE [Name] = 'CONGLOMERATE'), 
	'Lex Luthor', 'Lex.Luthor@LexCorp.com', 'Mercy Graves', 'Mercy.Graves@LexCorp.com', GETDATE(), 'LexCorp');
INSERT INTO [dbo].[Customers]([Name], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate], [Description])
	VALUES ('QUEEN_INDUSTRIES', 
	(SELECT [Id] FROM [dbo].[Industries] WHERE [Name] = 'MANUFACTURING'), 
	'Oliver Queen', 'Oliver.Queen@QueenIndustries.com', 'John Diggle', 'John.Diggle@QueenIndustries.com', GETDATE(), 'Queen Industries');
INSERT INTO [dbo].[Customers]([Name], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate], [Description])
	VALUES ('STAR_LABS', 
	(SELECT [Id] FROM [dbo].[Industries] WHERE [Name] = 'TECHNOLOGY'), 
	'Garrison Slate', 'Garrison.Slate@StarLabs.com', 'Garrison Slate', 'Garrison.Slate@StarLabs.com', GETDATE(), 'Science and Technology Advanced Research Laboratories');
INSERT INTO [dbo].[Customers]([Name], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate], [Description])
	VALUES ('WAYNE', 
	(SELECT [Id] FROM [dbo].[Industries] WHERE [Name] = 'CONGLOMERATE'), 
	'Bruce Wayne', 'Bruce.Wayne@WayneEnterprise.com', 'Lucius Fox', 'Lucius.Fox@WayneEnterprise.com', GETDATE(), 'Wayne Enterprises');

-- Tertiary Tables
INSERT INTO [dbo].[Integrations]([CustomerId], [IntegrationTypeId], [IntegrationStatusId], [Name], [Description], [CreatedDate])
	VALUES ((SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[IntegrationTypes] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[IntegrationStatuses] WHERE [Name] = 'DEFAULT'), 
	'DEFAULT', 'Default', GETDATE());

INSERT INTO [dbo].[Projects]([Name], [CustomerId], [CurrentPhase], [TargetGoLiveDate])
	VALUES ('DEFAULT', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'DEFAULT'), 
	GETDATE());

INSERT INTO [dbo].[Tickets]([ProjectId], [IntegrationId], [SeverityId], [SupportStatusId], [Title], [Description], [CreatedDate])
	VALUES ((SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[Integrations] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[Severities] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[SupportStatuses] WHERE [Name] = 'DEFAULT'), 
	'DEFAULT', 'Default', GETDATE());

-- Quarternary Tables
INSERT INTO [dbo].[Escalations]([TicketId], [CreatedDate], [ProblemSummary], [CustomerImpact], [RootCause], [RecommendedActions])
	VALUES (0, GETDATE(), 'Default', 'Default', 'Default', 'Default');
