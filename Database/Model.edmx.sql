
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/15/2014 18:20:21
-- Generated from EDMX file: C:\Users\wakco_000\Desktop\worldmachine\Database\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [WorldMachine];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_FeedFeedItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FeedItems] DROP CONSTRAINT [FK_FeedFeedItem];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Feeds]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Feeds];
GO
IF OBJECT_ID(N'[dbo].[FeedItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FeedItems];
GO
IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Feeds'
CREATE TABLE [dbo].[Feeds] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [RssUrl] nvarchar(max)  NOT NULL,
    [UpdateInterval] real  NOT NULL,
    [LastUpdate] datetime  NULL
);
GO

-- Creating table 'FeedItems'
CREATE TABLE [dbo].[FeedItems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FeedId] int  NOT NULL,
    [Published] datetime  NOT NULL,
    [Url] nvarchar(max)  NOT NULL,
    [Guid] nvarchar(max)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [Tags] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [ItemCount] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Feeds'
ALTER TABLE [dbo].[Feeds]
ADD CONSTRAINT [PK_Feeds]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FeedItems'
ALTER TABLE [dbo].[FeedItems]
ADD CONSTRAINT [PK_FeedItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [FeedId] in table 'FeedItems'
ALTER TABLE [dbo].[FeedItems]
ADD CONSTRAINT [FK_FeedFeedItem]
    FOREIGN KEY ([FeedId])
    REFERENCES [dbo].[Feeds]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FeedFeedItem'
CREATE INDEX [IX_FK_FeedFeedItem]
ON [dbo].[FeedItems]
    ([FeedId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------