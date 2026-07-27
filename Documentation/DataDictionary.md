# Data Dictionary

## Tables

### Customers

This table represents the customers of the company. Each customer has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted. The table has a foreign key to the Industries table.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.  

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

##### IndustryId

Data Type: bigint  
Allow Nulls: No  
Default Value: 0  
This is a foreign key to the Industries table. This column has an index.

##### PrimaryContactName

Data Type: nvarchar(63)  
Allow Nulls: Yes  
This field is meant for the name of the primary contact for the customer.

##### PrimaryContactEmail

Data Type: nvarchar(63)  
Allow Nulls: Yes  
This field is meant for the email address of the primary contact for the customer.

##### TechnicalContactName

Data Type: nvarchar(63)  
Allow Nulls: Yes  
This field is meant for the name of the technical contact for the customer. 

##### TechnicalContactEmail

Data Type: nvarchar(63)  
Allow Nulls: Yes  
This field is meant for the email address of the technical contact for the customer.

##### CreatedDate

Data Type: datetime  
Allow Nulls: Yes  
This field is meant for the date and time when the customer entry was created.

### Escalations

This table represents the escalations that have been created for the customers. Each escalation has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the escalation has been deleted.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

##### CreatedDate

Data Type: datetime  
Allow Nulls: Yes  
This field is meant for the date and time when the escalation was created.

##### ProblemSummary

Data Type: nvarchar(max)  
Allow Nulls: Yes  
This field is meant for a summary of the problem that led to the escalation.

##### CustomerImpact

Data Type: nvarchar(max)  
Allow Nulls: Yes  
This field is meant for a description of the impact that the problem had on the customer.

##### RootCause

Data Type: nvarchar(max)  
Allow Nulls: Yes  
This field is meant for a description of the root cause of the problem that led to the escalation.

##### RecommendedActions

Data Type: nvarchar(max)  
Allow Nulls: Yes  
This field is meant for a description of the recommended actions to be taken to resolve the problem that led to the escalation.

### Industries

This table represents the industries that the customers belong to. Each industry has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the industry has been deleted.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

### IntegrationErrors

This table acts as an error log for the integrations being tracked. Each Integration Error has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted. It also has a foreign key to the Integrations table.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

##### IntegrationId

Data Type: bigint  
Allow Nulls: Yes  
This is a foreign key to the Integrations table. This column has an index.

##### ErrorMessage

Data Type: nvarchar(1027)  
Allow Nulls: No  
This field is meant to hold the exception message thrown by the integration.

##### Stacktrace

Data Type: nvarchar(max)  
Allow Nulls: No  
This field is meant to hold the stack trace from the exception thrown by the integration.

##### ErrorTime

Data Type: datetime  
Allow Nulls: No  
This field contains the date and time that the integration error occured.

### Integrations

This table holds the basic data for integrations monitored by the system. Each Integration has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted. The table also has foreign keys to the Customer table, the IntegrationTypes table, and the IntegrationStatuses table.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

##### CustomerId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Customers table. This column has an index.

##### IntegrationTypeId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the IntegrationTypes table. This column has an index.

##### CurrentStatusId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the IntegrationStatuses table. This column has an index.

##### LastSuccessfulSync

Data Type: datetime  
Allow Nulls: Yes  
This is the date and time of the last time that the integration operated successfully.

##### LastFailedSync

Data Type: datetime  
Allow Nulls: Yes  
This is the date and time of the last time that the integration failed.

##### RetryCount

Data Type: int  
Allow Nulls: No  
Default Value: 0  
This is a field to contain the number of retry attempts for the integration to use if the integration fails.

### IntegrationStatuses

This table lists the possible statuses of integrations. Each status has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface.  This column has an index.

### Integration Types

This table lists the types of possible integrations. Each customer has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

### LinkProjectPhase

This table provides the link to the phases for a given project entry. Each link also has it's own properties, which is why this is a link table, and is wider than the usual many-to-many relationship tables. Each link has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted.

This table has a UNIQUE constraint on the ProjectId and PhaseId columns- there can only be one combination of Project and Phase.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

##### Project Id

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Projects table. This column has an index.

##### PhaseId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Phases table. This column has an index.

##### Percentage

Data Type: decimal(18,0)
Allow Nulls: Yes
This represents the percentage of the project that the phase represents.

##### Order

Data Type: int  
Allow Nulls: Yes
This represents the relative order of the phase for the project.

### Phases

This table holds the possible phases used by projects in the system. Each phase has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

### ProjectNotes

This table holds the notes for each project. Each Project Note has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted. The table also has a foreign key to the Projects table.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

##### ProjectId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Projects table. This column has an index.

##### Note

Data Type: nvarchar(max)  
Allow Nulls: Yes  
This is the actual note for the project.

##### CreateTime

Data Type: datetime  
Allow Nulls: No  
This represents the time the note was created.

### Projects

This table represents the projects being managed. Each project has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted. The project also has foreign keys to the Customers and Phases tables.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

##### CustomerId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Customers table. It has an index.

##### CurrentPhase

Data Type: bigint  
Allow Nulls: No
This represents the current active phase of the project. It is a foreign key to the Phases table. It has an index.

##### TargetGoLiveDate

Data Type:  datetime  
Allow Nulls: No  
This represents the target date for the project's go-live event.

##### ActualGoLiveDate

Data Type: datetime  
Allow Nulls: Yes
This represents the actual date for the project's go-live event.

### Severities

This table holds the possible ticket severities used by the system.  Each severity has a unique Id, a Name, and a Description.  The table also has a Deleted flag to indicate if the customer has been deleted.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

### SupportStatuses

This table holds the possible statuses that are used by support tickets.  Each status has a unique Id, a Name, and a Description.  The table also has a Deleted flag to indicate if the customer has been deleted.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

### TicketNote

This table holds the notes for each Ticket. Each Ticket Note has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted. The table also has a foreign key to the Tickets table.

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)   
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(255)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an index.

##### TicketId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Tickets table. This column has an index.

##### Note

Data Type: nvarchar(max)  
Allow Nulls: Yes  
This is the actual note for the ticket.

##### CreateTime

Data Type: datetime  
Allow Nulls: No  
This represents the time the note was created.

### Tickets

This table holds the data for support Tickets filed in the system. Each Ticket has a unique Id, a Name, and a Description. The table also has a Deleted flag to indicate if the customer has been deleted. The table has foreign keys to the Customers, Integrations, Severities, SupportStatuses and the Escalations tables. 

#### Columns

##### Id

Data Type: bigint  
Allow Nulls: No  
Primary Key for the table.

##### Name

Data Type: nvarchar(63)  
Allow Nulls: No  
Human Readable Candidate key for the table. The column has a UNIQUE constraint. This is used by application developers in the code to reference a specific row, instead of a tightly coupled enumeration in the application.

##### Description

Data Type: nvarchar(1023)  
Allow Nulls: Yes  
This field is meant for a description that would appear in the user interface for this item.

##### Deleted

Data Type: bit  
Allow Nulls: No  
Default Value: 0  
This is a flag to indicate that the item has been deleted. This is used to implement a soft delete, where the item is not actually removed from the database, but is marked as deleted and will not be shown in the user interface. This column has an Index.

##### CustomerId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Customers table. This column has an index.

##### IntegrationId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Integrations table. This column has an index.

##### SeverityId

Data Type: bigint  
Allow Nulls: No  
This is a foreign key to the Severities table. This column has an index.

##### StatusId

Data Type: bigint  
Allow Nulls: No
This is a foreign key to the SupportStatuses table. This column has an index.

##### EscalationId

Data Type: bigint  
Allow Nulls: Yes  
This is a foreign key to the Escalations table. This column has an index.

##### Reproduce

Data Type: nvarchar(max)  
Allow Nulls: Yes  
This field is to hold the instructions to reproduce the error for the ticket.

##### ReportedBy

Data Type: nvarchar(63)  
Allow Nulls: No  
This field is for the name of the person reporting the issue.

##### AssignedTo

Data Type: nvarchar(63)  
Allow Nulls: Yes  
This field is for the name of the technician assigned to work the issue.

##### CreationDate

Data Type: datetime  
Allow Nulls: No  
This field holds the date that the issue was filed.

##### ResolutionDate

Data Type:  datetime  
Allow Nulls:  Yes  
This field holds the date that the issue was resolved.

##### Resolution

Data Type:  nvarchar(max)  
Allow Nulls: Yes  
This field is meant to hold the details of the resolution of the issue.
