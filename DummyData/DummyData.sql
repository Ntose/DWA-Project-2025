-- 1) Insert Topics
INSERT INTO Topic (Name)
VALUES
  ('Folklore'),
  ('Music'),
  ('Art'),
  ('Architecture'),
  ('Dance'),
  ('Ceramics'),
  ('Weaving'),
  ('Storytelling');
GO

-- 2) Insert National Minorities
INSERT INTO NationalMinority(Name)
VALUES
  ('Roma'),
  ('Serb'),
  ('Italian'),
  ('Hungarian'),
  ('Czech'),
  ('Slovak');
GO

-- 3) Insert Users
INSERT INTO ApplicationUser
  (Username, PasswordHash, Email, FirstName, LastName, Phone, DateRegistered, Role)
VALUES
  ('john.doe', 'test',    'john.doe@example.com',   'John',    'Doe',        '123-456-7890', '2024-02-15', 'User'),
  ('jane.smith', 'test', 'jane.smith@example.com', 'Jane',    'Smith',      '098-765-4321', '2024-03-10', 'User'),
  ('maria.popov', 'test', 'maria.popov@example.com','Maria',   'Popov',      '+385 91 234 5678','2024-04-20','User');
GO

-- 4) Insert Cultural Heritages
INSERT INTO CulturalHeritage
  (Name, Description, ImageUrl, NationalMinorityId)
VALUES
  ('Traditional Embroidery',
   'Intricate hand-stitched patterns passed down through generations of local artisans.',
   'https://picsum.photos/seed/embroidery/600/400',
   1),

  ('Folk Music Festival',
   'Annual gathering of musicians and dancers celebrating traditional tunes and steps.',
   'https://picsum.photos/seed/musicfest/600/400',
   2),

  ('Historic Fortress',
   'A medieval fortress perched on a rocky outcrop, guarding the valley for over 800 years.',
   'https://picsum.photos/seed/fortress/600/400',
   3),

  ('Clay Pottery Workshop',
   'Hands‐on workshops teaching ancient pottery techniques using local clay.',
   'https://picsum.photos/seed/pottery/600/400',
   1),

  ('Storytelling Evenings',
   'Oral storytelling sessions highlighting myths, legends, and family histories.',
   'https://picsum.photos/seed/storytelling/600/400',
   4);
GO

-- 5) Link Heritages to Topics (many-to-many)
INSERT INTO CulturalHeritageTopic(CulturalHeritageId, TopicId)
VALUES
  (1, 1), (1, 7),       -- Embroidery: Folklore, Weaving
  (2, 2), (2, 4),       -- Music Festival: Music, Architecture
  (3, 4), (3, 3),       -- Fortress: Architecture, Art
  (4, 6), (4, 7),       -- Pottery: Ceramics, Weaving
  (5, 8), (5, 1);       -- Storytelling: Storytelling, Folklore
GO


-- 6) Insert Logs linked to actions
INSERT INTO Log (Timestamp, Level, Message)
VALUES
  ('2024-02-15 08:00:00', 'Info',    'User john.doe registered account.'),
  ('2024-02-15 08:05:00', 'Info',    'User john.doe logged in.'),
  ('2024-03-10 09:30:00', 'Info',    'User jane.smith registered account.'),
  ('2024-03-10 09:35:00', 'Info',    'User jane.smith logged in.'),
  ('2024-04-20 10:00:00', 'Info',    'User maria.popov registered account.'),
  ('2024-04-20 10:05:00', 'Info',    'User maria.popov logged in.'),
  ('2024-05-01 10:15:00', 'Info',    'User john.doe commented on heritage #1.'),
  ('2024-05-02 14:30:00', 'Info',    'User jane.smith commented on heritage #1.'),
  ('2024-06-10 18:45:00', 'Info',    'User john.doe commented on heritage #2.'),
  ('2024-07-05 09:20:00', 'Info',    'User jane.smith commented on heritage #3.'),
  ('2024-07-12 16:10:00', 'Warning', 'User maria.popov submitted a comment pending approval.'),
  ('2024-07-15 19:05:00', 'Info',    'User john.doe commented on heritage #5.'),
  ('2024-07-16 11:00:00', 'Info',    'User john.doe updated profile information.'),
  ('2024-07-16 11:02:00', 'Error',   'Unauthorized access attempt to AdminTerminal by user john.doe.');
GO
-- 7) Insert Comments
INSERT INTO Comments
  (CulturalHeritageId, UserId, Text, Timestamp, Approved)
VALUES
  (1, 2,
   'Amazing craftsmanship! The colors and stitches tell a story of heritage and pride.',
   '2024-05-01 10:15:00', 1),

  (1, 3,
   'I love the geometric patterns—it''s like each stitch holds a secret.',
   '2024-05-02 14:30:00', 1),

  (2, 2,
   'What a lively festival—dancers and musicians truly know how to celebrate!',
   '2024-06-10 18:45:00', 1),

  (3, 3,
   'The fortress is breathtaking at sunrise. You can almost hear the history echo.',
   '2024-07-05 09:20:00', 1),

  (4, 4,
   'Made my first clay pot today—it was messy but so rewarding!',
   '2024-07-12 16:10:00', 0),  -- pending approval

  (5, 2,
   'These storytelling evenings bring our ancestors back to life. Incredible!',
   '2024-07-15 19:05:00', 1);
GO

