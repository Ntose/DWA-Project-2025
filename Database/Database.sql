-- =========================================
-- Tables
-- =========================================

-- 1. NationalMinority
CREATE TABLE NationalMinority
(
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100)    NOT NULL,
    CONSTRAINT UQ_NationalMinority_Name UNIQUE(Name)
);

-- 2. Topic
CREATE TABLE Topic
(
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100)    NOT NULL,
    CONSTRAINT UQ_Topic_Name UNIQUE(Name)
);

-- 3. CulturalHeritage
CREATE TABLE CulturalHeritage
(
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    Name                NVARCHAR(200)    NOT NULL,
    Description         NVARCHAR(MAX)    NULL,
    ImageUrl            NVARCHAR(500)    NULL,
    DateAdded           DATETIME         NOT NULL DEFAULT GETDATE(),
    NationalMinorityId  INT              NOT NULL,
    CONSTRAINT UQ_CulturalHeritage_Name UNIQUE(Name),
    CONSTRAINT FK_CH_NationalMinority
        FOREIGN KEY(NationalMinorityId) REFERENCES NationalMinority(Id)
);

-- 4. CulturalHeritageTopic (bridge M–N)
CREATE TABLE CulturalHeritageTopic
(
    CulturalHeritageId  INT NOT NULL,
    TopicId             INT NOT NULL,
    PRIMARY KEY(CulturalHeritageId, TopicId),
    CONSTRAINT FK_CHT_CulturalHeritage
        FOREIGN KEY(CulturalHeritageId) REFERENCES CulturalHeritage(Id),
    CONSTRAINT FK_CHT_Topic
        FOREIGN KEY(TopicId) REFERENCES Topic(Id)
);

-- 5. ApplicationUser
CREATE TABLE ApplicationUser
(
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Username        NVARCHAR(100)    NOT NULL,
    Email           NVARCHAR(200)    NOT NULL,
    PasswordHash    NVARCHAR(500)    NOT NULL,
    FirstName       NVARCHAR(100)    NULL,
    LastName        NVARCHAR(100)    NULL,
    Phone           NVARCHAR(50)     NULL,
    DateRegistered  DATETIME         NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_User_Username UNIQUE(Username),
    CONSTRAINT UQ_User_Email    UNIQUE(Email)
);

-- 6. Comment
CREATE TABLE Comment
(
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    Text                NVARCHAR(MAX)    NOT NULL,
    Timestamp           DATETIME         NOT NULL DEFAULT GETDATE(),
    CulturalHeritageId  INT              NOT NULL,
    UserId              INT              NOT NULL,
    Approved            BIT              NOT NULL DEFAULT 0,
    CONSTRAINT FK_Comment_CH
        FOREIGN KEY(CulturalHeritageId) REFERENCES CulturalHeritage(Id),
    CONSTRAINT FK_Comment_User
        FOREIGN KEY(UserId) REFERENCES ApplicationUser(Id)
);


-- =========================================
-- Seed Data
-- =========================================

-- NationalMinorities
INSERT INTO NationalMinority (Name) VALUES
('Hungarian'),
('Serbian'),
('Italian');

-- Topics
INSERT INTO Topic (Name) VALUES
('Art'),
('Architecture'),
('Gastronomy'),
('Music');

-- CulturalHeritage items
INSERT INTO CulturalHeritage (Name, Description, ImageUrl, NationalMinorityId) VALUES
('Folk Dance of Vojvodina',
 'Traditional dance from the Vojvodina region, showcasing vibrant costumes and choreography.',
 'https://i.scdn.co/image/ab67616d0000b27311bf9e2169296b850fa0b6ab',
 1),
('?erdap Gorge Architecture',
 'Historic fortifications and structures along the ?erdap Gorge.',
 'https://example.com/images/?erdap_arch.jpg',
 2),
('Trieste Seafood Festival',
 'Annual gastronomic event celebrating traditional seafood dishes in Trieste.',
 'https://example.com/images/trieste_festival.jpg',
 3);

-- CulturalHeritage–Topic relationships
INSERT INTO CulturalHeritageTopic (CulturalHeritageId, TopicId) VALUES
(1, 1),  -- Folk Dance ? Art
(1, 4),  -- Folk Dance ? Music
(2, 2),  -- ?erdap Gorge ? Architecture
(3, 3);  -- Trieste Festival ? Gastronomy

-- Application users (passwords stored as hashes/placeholders)
INSERT INTO ApplicationUser (Username, Email, PasswordHash, FirstName, LastName, Phone) VALUES
('admin01', 'admin@heritage.local', 'AQAAAAEAACcQAAAAEIa...hashed...', 'Platform', 'Admin', '+385912345678'),
('user01',  'user@heritage.local',  'AQAAAAEAACcQAAAAERx...hashed...', 'John',     'Doe',   '+385987654321');

-- Sample comments
INSERT INTO Comment (Text, CulturalHeritageId, UserId, Approved) VALUES
('Amazing dance tradition! Would love more video samples.', 1, 2, 1),
('Can we include historical maps of the ?erdap fort?', 2, 2, 0);
