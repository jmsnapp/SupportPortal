-- Base tables
INSERT INTO [dbo].[Industries]([Name], [Description], [Deleted]) VALUES ('DEFAULT', 'Default', 1);
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('AVIATION', 'Aviation');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('CONGLOMERATE', 'Conglomerate');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('FINANCIAL', 'Financial');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('FOOD_SERVICE', 'Food Services');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('MANUFACTURING', 'Manufacturing');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('MEDIA', 'Media');
INSERT INTO [dbo].[Industries]([Name], [Description]) VALUES ('TECHNOLOGY', 'Technology');

INSERT INTO [dbo].[IntegrationStatuses]([Name], [Description], [Deleted]) VALUES ('DEFAULT', 'Default', 1);
INSERT INTO [dbo].[IntegrationStatuses]([Name], [Description]) VALUES ('OPERATIONAL', 'Operational');
INSERT INTO [dbo].[IntegrationStatuses]([Name], [Description]) VALUES ('OUT_OF_SERVICE', 'Out of Service');
INSERT INTO [dbo].[IntegrationStatuses]([Name], [Description]) VALUES ('STAGING', 'Staging');

INSERT INTO [dbo].[IntegrationTypes]([Name], [Description], [Deleted]) VALUES ('DEFAULT', 'Default', 1);
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('API', 'Application Programming Interface');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('APPLICATION', 'Application');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('FTP_FILE_DROP', 'FTP File Drop');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('NETWORK_FILE_DROP', 'Network File Drop');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('SCHEDULED_JOB', 'Scheduled Job');
INSERT INTO [dbo].[IntegrationTypes]([Name], [Description]) VALUES ('SQLJOB', 'SQL Job');

INSERT INTO [dbo].[Phases]([Name], [Description], [Deleted]) VALUES ('DEFAULT', 'Default', 1);
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('BUS_REQ_DISCOVERY', 'Business Requirements Discovery');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('TECH_REQ_DISCOVERY', 'Technical Requirements Discovery');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('DEVELOPMENT', 'Development');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('TESTING', 'Testing');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('DEPLOYMENT', 'Deployment');
INSERT INTO [dbo].[Phases]([Name], [Description]) VALUES ('MAINTENANCE', 'Maintenance');

INSERT INTO [dbo].[Severities]([Name], [Description], [Deleted]) VALUES ('DEFAULT', 'Default', 1);
INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('LOW', 'Low');
INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('MEDIUM', 'Medium');
INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('HIGH', 'High');
INSERT INTO [dbo].[Severities]([Name], [Description]) VALUES ('CRITICAL', 'Critical');

INSERT INTO [dbo].[SupportStatuses]([Name], [Description], [Deleted]) VALUES ('DEFAULT', 'Default', 1);
INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('OPEN', 'Open');
INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('IN_PROGRESS', 'In Progress');
INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('RESOLVED', 'Resolved');
INSERT INTO [dbo].[SupportStatuses]([Name], [Description]) VALUES ('CLOSED', 'Closed');

-- Secondary Tables
INSERT INTO [dbo].[Customers]([Name], [Description], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate], [Deleted])
	VALUES ('DEFAULT', 'Default Customer', 0, 'Default', 'default@noemail.com', 'Default', 'default@noemail.com', GETDATE(), 1);
INSERT INTO [dbo].[Customers]([Name], [Description], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate])
	VALUES ('LEXCORP', 'LexCorp', 
	(SELECT [Id] FROM [dbo].[Industries] WHERE [Name] = 'CONGLOMERATE'), 
	'Lex Luthor', 'Lex.Luthor@LexCorp.com', 'Mercy Graves', 'Mercy.Graves@LexCorp.com', GETDATE());
INSERT INTO [dbo].[Customers]([Name], [Description], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate])
	VALUES ('QUEEN_INDUSTRIES', 'Queen Industries', 
	(SELECT [Id] FROM [dbo].[Industries] WHERE [Name] = 'MANUFACTURING'), 
	'Oliver Queen', 'Oliver.Queen@QueenIndustries.com', 'John Diggle', 'John.Diggle@QueenIndustries.com', GETDATE());
INSERT INTO [dbo].[Customers]([Name], [Description], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate])
	VALUES ('STAR_LABS', 'Science and Technology Advanced Laboratories', 
	(SELECT [Id] FROM [dbo].[Industries] WHERE [Name] = 'TECHNOLOGY'), 
	'Garrison Slate', 'Garrison.Slate@StarLabs.com', 'Garrison Slate', 'Garrison.Slate@StarLabs.com', GETDATE());
INSERT INTO [dbo].[Customers]([Name], [Description], [IndustryId], [PrimaryContactName], [PrimaryContactEmail], [TechnicalContactName], [TechnicalContactEmail], [CreatedDate])
	VALUES ('WAYNE', 'Wayne Industries', 
	(SELECT [Id] FROM [dbo].[Industries] WHERE [Name] = 'CONGLOMERATE'), 
	'Bruce Wayne', 'Bruce.Wayne@WayneEnterprise.com', 'Lucius Fox', 'Lucius.Fox@WayneEnterprise.com', GETDATE());

-- Tertiary Tables
INSERT INTO [dbo].[Integrations]([Name], [Description], [CustomerId], [IntegrationTypeId], [CurrentStatusId], [Deleted])
	VALUES ('DEFAULT', 'Default Integration', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[IntegrationTypes] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[IntegrationStatuses] WHERE [Name] = 'DEFAULT'), 1);
INSERT INTO [dbo].[Integrations]([Name], [Description], [CustomerId], [IntegrationTypeId], [CurrentStatusId], [LastSuccessfulSync], [LastFailedSync], [RetryCount])
	VALUES('QUEEN2JLZ', 'Queen Industries to the Justice League of America', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'QUEEN_INDUSTRIES'), 
	(SELECT [Id] FROM [dbo].[IntegrationTypes] WHERE [Name] = 'API'), 
	(SELECT [Id] FROM [dbo].[IntegrationStatuses] WHERE [Name] = 'OPERATIONAL'), 
	GETDATE(), DATEADD(MONTH, -6, GETDATE()), 5);
INSERT INTO [dbo].[Integrations]([Name], [Description], [CustomerId], [IntegrationTypeId], [CurrentStatusId], [LastSuccessfulSync], [LastFailedSync], [RetryCount])
	VALUES('WAYNE2JLZ', 'Wayne Industries to the Justice League of America', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'WAYNE'), 
	(SELECT [Id] FROM [dbo].[IntegrationTypes] WHERE [Name] = 'API'), 
	(SELECT [Id] FROM [dbo].[IntegrationStatuses] WHERE [Name] = 'OPERATIONAL'), 
	GETDATE(), DATEADD(MONTH, -6, GETDATE()), 5);
INSERT INTO [dbo].[Integrations]([Name], [Description], [CustomerId], [IntegrationTypeId], [CurrentStatusId], [LastSuccessfulSync], [LastFailedSync], [RetryCount])
	VALUES('LEX2DOOM', 'LexCorp to the Legion of Doom', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'LEXCORP'), 
	(SELECT [Id] FROM [dbo].[IntegrationTypes] WHERE [Name] = 'API'), 
	(SELECT [Id] FROM [dbo].[IntegrationStatuses] WHERE [Name] = 'OUT_OF_SERVICE'), 
	DATEADD(MONTH, -6, GETDATE()), GETDATE(), 10);
INSERT INTO [dbo].[Integrations]([Name], [Description], [CustomerId], [IntegrationTypeId], [CurrentStatusId], [LastSuccessfulSync], [LastFailedSync], [RetryCount])
	VALUES('FEED12STAR', 'Feed 1 to STAR Labs', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'STAR_LABS'), 
	(SELECT [Id] FROM [dbo].[IntegrationTypes] WHERE [Name] = 'NETWORK_FILE_DROP'), 
	(SELECT [Id] FROM [dbo].[IntegrationStatuses] WHERE [Name] = 'OUT_OF_SERVICE'), 
	DATEADD(MONTH, -6, GETDATE()), GETDATE(), 10);
INSERT INTO [dbo].[Integrations]([Name], [Description], [CustomerId], [IntegrationTypeId], [CurrentStatusId], [LastSuccessfulSync], [LastFailedSync], [RetryCount])
	VALUES('FEED22STAR', 'Feed 2 to STAR Labs', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'STAR_LABS'), 
	(SELECT [Id] FROM [dbo].[IntegrationTypes] WHERE [Name] = 'NETWORK_FILE_DROP'), 
	(SELECT [Id] FROM [dbo].[IntegrationStatuses] WHERE [Name] = 'OPERATIONAL'), 
	GETDATE(), DATEADD(MONTH, -6, GETDATE()), 5);

INSERT INTO [dbo].[Projects]([Name], [Description], [CustomerId], [CurrentPhase], [Deleted])
	VALUES ('DEFAULT', 'Default Project', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'DEFAULT'), 1);
INSERT INTO [dbo].[Projects]([Name], [Description], [CustomerId], [CurrentPhase], [TargetGoLiveDate])
	VALUES ('STAR2DOW', 'STAR Labs to Department of War feed', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'STAR_LABS'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'DEVELOPMENT'), 
	DATEADD(YEAR, 1, GETDATE()));

INSERT INTO [dbo].[LinkProjectPhase]([Name], [Description], [ProjectId], [PhaseId], [Percentage], [Order], [Deleted])
	VALUES('DEFAULT', 'Default', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'DEFAULT'), 
	0, 0, 1);
INSERT INTO [dbo].[LinkProjectPhase]([Name], [Description], [ProjectId], [PhaseId], [Percentage], [Order])
	VALUES('DEFAULT', 'Default', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'STAR2DOW'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'BUS_REQ_DISCOVERY'), 
	20, 1);
INSERT INTO [dbo].[LinkProjectPhase]([Name], [Description], [ProjectId], [PhaseId], [Percentage], [Order])
	VALUES('DEFAULT', 'Default', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'STAR2DOW'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'TECH_REQ_DISCOVERY'), 
	20, 2);
INSERT INTO [dbo].[LinkProjectPhase]([Name], [Description], [ProjectId], [PhaseId], [Percentage], [Order])
	VALUES('DEFAULT', 'Default', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'STAR2DOW'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'DEVELOPMENT'), 
	20, 3);
INSERT INTO [dbo].[LinkProjectPhase]([Name], [Description], [ProjectId], [PhaseId], [Percentage], [Order])
	VALUES('DEFAULT', 'Default', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'STAR2DOW'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'TESTING'), 
	20, 4);
INSERT INTO [dbo].[LinkProjectPhase]([Name], [Description], [ProjectId], [PhaseId], [Percentage], [Order])
	VALUES('DEFAULT', 'Default', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'STAR2DOW'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'DEPLOYMENT'), 
	20, 5);
INSERT INTO [dbo].[LinkProjectPhase]([Name], [Description], [ProjectId], [PhaseId], [Percentage], [Order])
	VALUES('DEFAULT', 'Default', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'STAR2DOW'), 
	(SELECT [Id] FROM [dbo].[Phases] WHERE [Name] = 'MAINTENANCE'), 
	0, 6);

INSERT INTO [dbo].[ProjectNotes]([Name], [Description], [ProjectId], [Deleted])
	VALUES ('DEFAULT', 'Default Note', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'DEFAULT'), 1);
INSERT INTO [dbo].[ProjectNotes]([Name], [Description], [ProjectId], [Note], [CreateTime])
	VALUES('1', 'Business Requirements status', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'STAR2DOW'), 
	'Business Requirements determined and documented.', 
	DATEADD(MONTH, -5, GETDATE()));
INSERT INTO [dbo].[ProjectNotes]([Name], [Description], [ProjectId], [Note], [CreateTime])
	VALUES('2', 'Technical Requirements status', 
	(SELECT [Id] FROM [dbo].[Projects] WHERE [Name] = 'STAR2DOW'), 
	'Technical Requirements determined and documented.', 
	DATEADD(MONTH, -4, GETDATE()));

INSERT INTO [dbo].[Escalations]([Name], [Description], [ProblemSummary], [CustomerImpact], [RootCause], [RecommendedActions], [Deleted])
	VALUES('DEFAULT', 'Default', 'Default', 'Default', 'Default', 'Default', 1);
INSERT INTO [dbo].[Escalations]([Name], [Description], [CreatedDate], [ProblemSummary], [CustomerImpact], [RootCause], [RecommendedActions])
	VALUES('1', 'Lexcorp to Doom integration failure', DATEADD(MONTH, -5, GETDATE()), 
	'Legion of Doom API not responding.', 'Customer is evasive on impact.  Due to language used by reporting contact, set impact at critical.', 
	'JLA Feed has a log entry referencing a JLA operation at the time that the customer''s API went down.  Reasonably suspect that the other server is no longer operational.', 
	'Recommend customer investigate other datacenter for remains.');

-- Quarternary Tables
INSERT INTO [dbo].[Tickets]([Name], [Description], [CustomerId], [IntegrationId], [Reproduce], [SeverityId], [StatusId], [ReportedBy], [AssignedTo], [CreatedDate], [ResolutionDate], [Resolution], [EscalationId], [Deleted])
	VALUES ('DEFAULT', 'Default Ticket', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'DEFAULT'),
	(SELECT [Id] FROM [dbo].[Integrations] WHERE [Name] = 'DEFAULT'), 
	'', 
	(SELECT [Id] FROM [dbo].[Severities] WHERE [Name] = 'DEFAULT'), 
	(SELECT [Id] FROM [dbo].[SupportStatuses] WHERE [Name] = 'DEFAULT'), 
	'Default', 'Default', GETDATE(), GETDATE(), '', 
	(SELECT [Id] FROM [dbo].[Escalations] WHERE [Name] = 'DEFAULT'), 1);
INSERT INTO [dbo].[Tickets]([Name], [Description], [CustomerId], [IntegrationId], [Reproduce], [SeverityId], [StatusId], [ReportedBy], [AssignedTo], [CreatedDate], [ResolutionDate], [Resolution], [EscalationId])
	VALUES('1', 'Lex2Doom integration down', 
	(SELECT [Id] FROM [dbo].[Customers] WHERE [Name] = 'LEXCORP'), 
	(SELECT [Id] FROM [dbo].[Integrations] WHERE [Name] = 'LEX2DOOM'), 
	'Using Postman, access https://api.LDoom.Net, get a http 404 error.  Customer states that datacenter should be on-line and there is no issue with the LexCorp network', 
	(SELECT [Id] FROM [dbo].[Severities] WHERE [Name] = 'CRITICAL'), 
	(SELECT [Id] FROM [dbo].[SupportStatuses] WHERE [Name] = 'RESOLVED'), 
	'Lex Luthor', 'Matt Snapp', DATEADD(MONTH, -6, GETDATE()), DATEADD(MONTH, -5, GETDATE()), 
	'JLA Feed has a log entry referencing a JLA operation at the time that the customer''s API went down.  Reasonably suspect that the other server is no longer operational.', 1);

INSERT INTO [dbo].[TicketNote]([Name], [Description], [TicketId], [Deleted])
	VALUES(0, 'Default Note', 0, 1);
INSERT INTO [dbo].[TicketNote]([Name], [Description], [TicketId], [Note], [CreateTime])
	VALUES('1', 'Customer Contact Warning', 1, 'This guy is a jerk.  Redirect to technical contact ASAP.', DATEADD(MONTH, -5, GETDATE()));
