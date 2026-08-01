# Design Notes

I have been using Artificial Intelligence (specifically Microsoft CoPilot in Visual Studio) to scaffold the projects in this solution. This has been very useful in setting up the basic project structures and providing a 'first pass' of the basic objects (such as the database table definitions, the Infrastructure entities and models, the API controllers, and the Unit Tests).  I have still had to go in to modify these bases to fit my style and approaches, but the use of AI has significantly cut down on the time needed to architect all of this out.

I have also been impressed with the value provided by Microsoft CoPilot in Visual Studio, as I am using the Visual Studio community edition, and am not paying for any extra tokens or resources.  In the process of scaffolding the database project, the infrastructure project, and the API, I did use up all my Inline suggestions, but less than 20% of my Monthly Limit for my copilot context.  Compare that to Anthropic's Claude, where I was not able to revise my resume using another version of the document and my CV without having to purchase their lowest tier service.  I suspect that the difference in value is due to Microsoft using their own MAI models for much of this work, and (very strangely for a technology company in this day and age) passing those savings on to me.

# Database

## SQL Version

The SQL that I chose for this project is Microsoft SQL. There are a couple of reasons for my choice. 
 
1. The developer editions of MSSQL are free, and give you a full MSSQL server instance (albeit in a local-only mode). You get all the crunchy goodness of a real production grade SQL instance limited only by the specifications of your development machine. The networking aspect of the MSSQL should not be affected by the application, so a local instance will be just fine.

2. I am most familiar with MSSQL, having been using it since 1997 (with SQL 6.0) all the way through modern versions (at the time of this writing, SQL2025 and AzureSQL). I have some experience with Oracle PL/SQL back in the mists of time, and some GuptaSQL back when we regularly had to chase the velociraptors out of the server room. If you want to put this on PL/SQL, it should work fine except you will need to check the date formats and the date functions used in the SeedData.sql file. I am not doing anything particularly sophisticated in the database tables, so the SQL code should translate to other SQL versions (like PostGresSQL) with minimal changes.

## Table Structure

I have my tables structured in a very consistent way, which is very deliberate.

First off, every table has an 'Id' column which is auto incrementing (with the seed as '0'- more on that in a bit) and acting as the primary key for the table. In MSSQL, this also forces the server to put the table's clustered index on that column. This way, I can uniquely identify every row in that table with a minimum of trouble. If I know the row's 'Id' field, I can find that row.

Second, each table has a 'Name' column.  This column has a unique constraint on it.  On most tables, this acts as a human readable Candidate Key that can be referenced in code to find that table row.  This allows me to avoid having tightly bound enumerations in the application code.  If I know the 'Name', I can find that unique row, and any developer following along behind trying to discern my alleged brilliance will know what I am referring to without having to just to some enum defined somewhere else- usually in another project.  

Third, each table has a 'Description' field. This column is what will usually be shown in the User Interface for tables that supply dropdown controls. For objects like Escalations, Integrations, Projects, and Tickets, it is also used as the summary or heading field.

Finally, each table has a 'Deleted' field. This is used to cause a soft delete of objects, instead of a hard delete. If, at some later date, the tables need to be cleared out of old data, then an archiving process can be built to take care of this. In the meantime, data integrity is always preserved. It does mean that the application does have to filter out deleted items, but this is a simple conditional in a SQL query or a LINQ query. This also means that there is no function difference between an Update operation and a Delete operation- technically, the Delete operation does not exist. This is put in as an administrative survival mechanism. If I had a dollar (USD) for every stakeholder who said 'I need that <whatever> back!' aftet deleting something, I wouldn't need to have a portfolio project.

Also, notice that I do not allow Nulls in the database. As a result, I have default data defined for fields that are not required. I try to do this whenever I can, as Null values in the data store propogate through the application code and cause issues. Therefore, I should not have to do any null checks on the data coming from my datastore. I will have to do some value checks on objects to see if they are 'real', but those checks can only be where logically necessary. I should not have to do any null checks of data being sent to the User Interface, because I know that an acceptable default value will be there, at least.

This also means that I have had to enter default data into my datastore, specifically a default entry in every table, set to default data for all fields. These rows will have the 'Id' of 0 (due to my seed value above), so any object with an 'Id' of '0' is not 'real' and can be discarded. However, this also means that foreign keys are always defined, even for empty objects, further reducing the need for Null checks.

# Infrastructure

## Use of Entity Framework

I am using Entity Framework Core as my ORM.  I did this primarily to prove that I know how to use LINQ an ORM in general and Entity Framework in particular. In most of my roles, I have had sufficient access to the datastores that I could use stored procedures for CRUD operations on the database.  I do understand the attractions of ORMs, and I have used them on those rare occasions where my user account only had Read access to the datastores, and any changes had to be done through the application.  In these situations, an ORM comes in handy, as it will provide an Object interface for CRUD operations.

Usually what I would do for a project like this is create a series of stored procedures for each table that handle the basic CRUD operations. This would consist of:  
1. uspGetAllWhatever - returns all the objects in the table. Usually used for admin interfaces to maintain lookup tables. Effectively `SELECT * FROM [Whatevers]`.
2. uspGetAllActiveWhatever - returns all the objects in the table that are not deleted. Usually used for dropdowns populated by lookup tables.  Effectively `SELECT * FROM [Whatevers] WHERE [Deleted] = 0`.
3. upsGetWhateverById - returns a single row from the table that has that Id. Effectively `SELECT * FROM [Whatevers] WHERE [Id] = @value`
4. upsGetWhateverByName - returns a single row from the table that has that Name. Effectively `SELECT * FROM [Whatevers] WHERE [Name] = @value`
5. upsCreateWhatever - Inserts a new row in the `[Whatevers]` table.
6. uspUpdateWhatever - Updates the row in the `[Whatevers]` table. Because I use soft deleted by default, this also functionally acts as the Delete operation, just by setting the `[Deleted]` field to '1'.

For complex objects that have foreign keys, I use these basic procedures for the links to the child tables.  That way, if there is a problem in a given stored procedure, I only have to fix it in one location.

# Domain

The Domain project is sparse, as this system is currently a very simple system. I have the domain models defined in there (and those are also very simple due to the straightforward requirements of the system).

I also have the DBMapper class in the Domain project instead of the Infrastructure project. I did this because the DB Mapper takes the Infrastructure entities and maps them to Domain models.  If I decide to add another data source (like an API, for example), I can put that access and it's appropriate entities into the Infrastructure project, then just add an appropriate Mapper class to the Domain project.  That Infrastructure/Domain boundary remains defined across data sources, and layers above the domain (the SupportPortalAPI and eventually the Web UI) are not majorly affected by the new data source, other than their Models can now show data from that new source.

# API

The Application Programming Interface (API) project is a very simple project.  It is a .NET 10 Web API project, and it is the only project that has any external dependencies.  It has a dependency on the Domain project, and it has a dependency on the Infrastructure project.  The Controller classes are just wrappers around the Domain models and provide endpoints that map to the repository methods in the Infrastructure project.

# Application

I have not built out the Application project yet, but it will be a .NET 10 Blazor WebAssembly project.  It will have a dependency on the Domain project, and it will have a dependency on the API project.  The Models in the Application project will be mapped to the Domain models, and the UI will be built using Blazor components that call the API endpoints to get data from the Infrastructure project.
