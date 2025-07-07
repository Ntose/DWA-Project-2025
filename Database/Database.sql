/*
Do not use database modifying (ALTER DATABASE), creating (CREATE DATABASE) or switching (USE) statements 
in this file.
*/

-- ========================================
-- 1. Users
-- ========================================
CREATE TABLE [dbo].[ApplicationUser](
    [Id]             INT IDENTITY(1,1)    NOT NULL,
    [Username]       NVARCHAR(100)        NOT NULL,
    [Email]          NVARCHAR(200)        NOT NULL,
    [PasswordHash]   NVARCHAR(500)        NOT NULL,
    [FirstName]      NVARCHAR(100)        NULL,
    [LastName]       NVARCHAR(100)        NULL,
    [Phone]          NVARCHAR(50)         NULL,
    [DateRegistered] DATETIME             NOT NULL,
    [Role]           NVARCHAR(50)         NOT NULL,
    CONSTRAINT [PK_ApplicationUser] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- ========================================
-- 2. NationalMinority (1-to-N)
-- ========================================
CREATE TABLE [dbo].[NationalMinority](
    [Id]   INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100)     NOT NULL,
    CONSTRAINT [PK_NationalMinority] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- ========================================
-- 3. Topic (1-to-N)
-- ========================================
CREATE TABLE [dbo].[Topic](
    [Id]   INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100)     NOT NULL,
    CONSTRAINT [PK_Topic] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- ========================================
-- 4. CulturalHeritage (1-to-N)
-- ========================================
CREATE TABLE [dbo].[CulturalHeritage](
    [Id]                 INT IDENTITY(1,1) NOT NULL,
    [Name]               NVARCHAR(200)     NOT NULL,
    [Description]        NVARCHAR(MAX)     NULL,
    [ImageUrl]           NVARCHAR(500)     NULL,
    [DateAdded]          DATETIME         NOT NULL,
    [NationalMinorityId] INT              NOT NULL,
    CONSTRAINT [PK_CulturalHeritage] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- ========================================
-- 5. CulturalHeritageTopic (M-to-N)
-- ========================================
CREATE TABLE [dbo].[CulturalHeritageTopic](
    [CulturalHeritageId] INT NOT NULL,
    [TopicId]            INT NOT NULL,
    CONSTRAINT [PK_CulturalHeritageTopic] PRIMARY KEY CLUSTERED ([CulturalHeritageId] ASC, [TopicId] ASC)
)
GO

-- ========================================
-- 6. Comments (1-to-N)
-- ========================================
CREATE TABLE [dbo].[Comments](
    [Id]                 INT IDENTITY(1,1) NOT NULL,
    [Text]               NVARCHAR(MAX)     NOT NULL,
    [Timestamp]          DATETIME         NOT NULL,
    [Approved]           BIT              NOT NULL,
    [CulturalHeritageId] INT              NOT NULL,
    [UserId]             INT              NOT NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- ========================================
-- 7. Log
-- ========================================
CREATE TABLE [dbo].[Log](
    [Id]        INT IDENTITY(1,1) NOT NULL,
    [Timestamp] DATETIME         NOT NULL,
    [Level]     NVARCHAR(20)     NOT NULL,
    [Message]   NVARCHAR(MAX)    NOT NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- ========================================
-- Indexes
-- ========================================
CREATE UNIQUE INDEX [UQ_ApplicationUser_Username] 
    ON [dbo].[ApplicationUser]([Username]);
GO
CREATE UNIQUE INDEX [UQ_ApplicationUser_Email] 
    ON [dbo].[ApplicationUser]([Email]);
GO
CREATE UNIQUE INDEX [UQ_NationalMinority_Name] 
    ON [dbo].[NationalMinority]([Name]);
GO
CREATE UNIQUE INDEX [UQ_Topic_Name] 
    ON [dbo].[Topic]([Name]);
GO
CREATE UNIQUE INDEX [UQ_CulturalHeritage_Name] 
    ON [dbo].[CulturalHeritage]([Name]);
GO

-- ========================================
-- Default constraints
-- ========================================
ALTER TABLE [dbo].[ApplicationUser] 
    ADD CONSTRAINT [DF_ApplicationUser_DateRegistered] 
    DEFAULT (GETDATE()) FOR [DateRegistered];
GO
ALTER TABLE [dbo].[ApplicationUser] 
    ADD CONSTRAINT [DF_ApplicationUser_Role] 
    DEFAULT ('User') FOR [Role];
GO
ALTER TABLE [dbo].[CulturalHeritage] 
    ADD CONSTRAINT [DF_CulturalHeritage_DateAdded] 
    DEFAULT (GETDATE()) FOR [DateAdded];
GO
ALTER TABLE [dbo].[Comments] 
    ADD CONSTRAINT [DF_Comments_Timestamp] 
    DEFAULT (GETDATE()) FOR [Timestamp];
GO
ALTER TABLE [dbo].[Comments] 
    ADD CONSTRAINT [DF_Comments_Approved] 
    DEFAULT ((0)) FOR [Approved];
GO
ALTER TABLE [dbo].[Log] 
    ADD CONSTRAINT [DF_Log_Timestamp] 
    DEFAULT (GETDATE()) FOR [Timestamp];
GO

-- ========================================
-- Foreign key constraints
-- ========================================
ALTER TABLE [dbo].[CulturalHeritage] 
    WITH CHECK ADD CONSTRAINT [FK_CH_NationalMinority] 
    FOREIGN KEY([NationalMinorityId])
    REFERENCES [dbo].[NationalMinority]([Id]);
GO
ALTER TABLE [dbo].[CulturalHeritageTopic] 
    WITH CHECK ADD CONSTRAINT [FK_CHT_CH] 
    FOREIGN KEY([CulturalHeritageId])
    REFERENCES [dbo].[CulturalHeritage]([Id])
    ON DELETE CASCADE;
GO
ALTER TABLE [dbo].[CulturalHeritageTopic] 
    WITH CHECK ADD CONSTRAINT [FK_CHT_Topic] 
    FOREIGN KEY([TopicId])
    REFERENCES [dbo].[Topic]([Id])
    ON DELETE CASCADE;
GO
ALTER TABLE [dbo].[Comments] 
    WITH CHECK ADD CONSTRAINT [FK_Comments_CH] 
    FOREIGN KEY([CulturalHeritageId])
    REFERENCES [dbo].[CulturalHeritage]([Id])
    ON DELETE CASCADE;
GO
ALTER TABLE [dbo].[Comments] 
    WITH CHECK ADD CONSTRAINT [FK_Comments_User] 
    FOREIGN KEY([UserId])
    REFERENCES [dbo].[ApplicationUser]([Id])
    ON DELETE CASCADE;
GO

-- ========================================
-- Seed: Admin user
-- ========================================
INSERT INTO [dbo].[ApplicationUser]
    ([Username],[Email],[PasswordHash],[FirstName],[LastName],[Phone],[Role])
VALUES
    ('admin','admin@example.com','admin','System','Administrator','000-000-0000','Admin');
GO
