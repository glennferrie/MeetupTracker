
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/11/2015 11:54:52
-- Generated from EDMX file: c:\users\glenn\documents\visual studio 2015\Projects\MeetupTracker\MeetupTracker\Models\MeetupModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MeetupTracker];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MeetupEventAttendee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Attendees] DROP CONSTRAINT [FK_MeetupEventAttendee];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[MeetupEvents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MeetupEvents];
GO
IF OBJECT_ID(N'[dbo].[Attendees]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Attendees];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'MeetupEvents'
CREATE TABLE [dbo].[MeetupEvents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Location] nvarchar(max)  NOT NULL,
    [EventDate] datetime  NOT NULL
);
GO

-- Creating table 'Attendees'
CREATE TABLE [dbo].[Attendees] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FullName] nvarchar(max)  NOT NULL,
    [Company] nvarchar(max)  NULL,
    [EmailAddress] nvarchar(max)  NOT NULL,
    [InviteDate] datetime  NOT NULL,
    [Rsvp] bit  NULL,
    [Attended] bit  NULL,
    [InvitationCode] nvarchar(max)  NOT NULL,
    [MeetupEvent_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'MeetupEvents'
ALTER TABLE [dbo].[MeetupEvents]
ADD CONSTRAINT [PK_MeetupEvents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Attendees'
ALTER TABLE [dbo].[Attendees]
ADD CONSTRAINT [PK_Attendees]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MeetupEvent_Id] in table 'Attendees'
ALTER TABLE [dbo].[Attendees]
ADD CONSTRAINT [FK_MeetupEventAttendee]
    FOREIGN KEY ([MeetupEvent_Id])
    REFERENCES [dbo].[MeetupEvents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MeetupEventAttendee'
CREATE INDEX [IX_FK_MeetupEventAttendee]
ON [dbo].[Attendees]
    ([MeetupEvent_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------