/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
update Questions set Bounty=0 where Bounty is NULL;
update Comments set Score=0 where Score is NULL;
update Answers set Score=0 where Score is NULL;
update Questions set Score=0 where Score is NULL;
update UserLevel set XP=0 where XP is NULL;
GO
ALTER TABLE dbo.Badges
	DROP CONSTRAINT DF_Badges_ID
GO
ALTER TABLE dbo.Badges
	DROP CONSTRAINT DF_Badges_Reward
GO
ALTER TABLE dbo.Badges
	DROP CONSTRAINT DF_Badges_Rarity
GO
CREATE TABLE dbo.Tmp_Badges
	(
	ID uniqueidentifier NOT NULL,
	Name nvarchar(50) NOT NULL,
	Description nvarchar(MAX) NULL,
	Image varbinary(MAX) NULL,
	Reward int NOT NULL,
	ImageLoc nvarchar(MAX) NULL,
	IdentifierName nvarchar(MAX) NULL,
	Rarity int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Badges SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Badges ADD CONSTRAINT
	DF_Badges_ID DEFAULT (newid()) FOR ID
GO
ALTER TABLE dbo.Tmp_Badges ADD CONSTRAINT
	DF_Badges_Reward DEFAULT ((0)) FOR Reward
GO
ALTER TABLE dbo.Tmp_Badges ADD CONSTRAINT
	DF_Badges_Rarity DEFAULT ((0)) FOR Rarity
GO
IF EXISTS(SELECT * FROM dbo.Badges)
	 EXEC('INSERT INTO dbo.Tmp_Badges (ID, Name, Description, Image, Reward, ImageLoc, IdentifierName, Rarity)
		SELECT ID, Name, Description, Image, CONVERT(int, Reward), ImageLoc, IdentifierName, Rarity FROM dbo.Badges WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.UserBadges
	DROP CONSTRAINT FK_UserBadges_Badges
GO
DROP TABLE dbo.Badges
GO
EXECUTE sp_rename N'dbo.Tmp_Badges', N'Badges', 'OBJECT' 
GO
ALTER TABLE dbo.Badges ADD CONSTRAINT
	PK_Badges PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT FK_UserLevel_Category
GO
ALTER TABLE dbo.Questions
	DROP CONSTRAINT FK_Questions_Category
GO
ALTER TABLE dbo.Category SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ScoreItems
	DROP CONSTRAINT FK_ScoreItems_AspNetUsers
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT FK_UserLevel_AspNetUsers
GO
ALTER TABLE dbo.Answers
	DROP CONSTRAINT FK_Answers_AspNetUsers
GO
ALTER TABLE dbo.Comments
	DROP CONSTRAINT FK_Comments_AspNetUsers
GO
ALTER TABLE dbo.Questions
	DROP CONSTRAINT FK_Questions_AspNetUsers
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Questions
	DROP CONSTRAINT DF_Questions_Added
GO
ALTER TABLE dbo.Questions
	DROP CONSTRAINT DF_Questions_Bounty
GO
ALTER TABLE dbo.Questions
	DROP CONSTRAINT DF_Questions_Score
GO
CREATE TABLE dbo.Tmp_Questions
	(
	ID uniqueidentifier NOT NULL,
	UserID nvarchar(128) NULL,
	CategoryID uniqueidentifier NOT NULL,
	Title nvarchar(128) NOT NULL,
	QuestionBody nvarchar(MAX) NULL,
	Added datetime NOT NULL,
	Modified datetime NULL,
	Bounty int NOT NULL,
	Score int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Questions SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Questions ADD CONSTRAINT
	DF_Questions_Added DEFAULT (sysdatetime()) FOR Added
GO
ALTER TABLE dbo.Tmp_Questions ADD CONSTRAINT
	DF_Questions_Bounty DEFAULT ((0)) FOR Bounty
GO
ALTER TABLE dbo.Tmp_Questions ADD CONSTRAINT
	DF_Questions_Score DEFAULT ((0)) FOR Score
GO
IF EXISTS(SELECT * FROM dbo.Questions)
	 EXEC('INSERT INTO dbo.Tmp_Questions (ID, UserID, CategoryID, Title, QuestionBody, Added, Modified, Bounty, Score)
		SELECT ID, UserID, CategoryID, Title, QuestionBody, Added, Modified, CONVERT(int, Bounty), CONVERT(int, Score) FROM dbo.Questions WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.QuestionViews
	DROP CONSTRAINT FK_QuestionViews_Questions
GO
ALTER TABLE dbo.QuestionTags
	DROP CONSTRAINT FK_QuestionTags_Questions
GO
ALTER TABLE dbo.Answers
	DROP CONSTRAINT FK_Answers_Questions
GO
DROP TABLE dbo.Questions
GO
EXECUTE sp_rename N'dbo.Tmp_Questions', N'Questions', 'OBJECT' 
GO
ALTER TABLE dbo.Questions ADD CONSTRAINT
	PK_Questions PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Questions ADD CONSTRAINT
	FK_Questions_AspNetUsers FOREIGN KEY
	(
	UserID
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Questions ADD CONSTRAINT
	FK_Questions_Category FOREIGN KEY
	(
	CategoryID
	) REFERENCES dbo.Category
	(
	ID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT
	FK_QuestionTags_Questions FOREIGN KEY
	(
	QuestionID
	) REFERENCES dbo.Questions
	(
	ID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.QuestionTags SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.QuestionViews ADD CONSTRAINT
	FK_QuestionViews_Questions FOREIGN KEY
	(
	QuestionID
	) REFERENCES dbo.Questions
	(
	ID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.QuestionViews SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Comments
	DROP CONSTRAINT DF_Comments_Added
GO
ALTER TABLE dbo.Comments
	DROP CONSTRAINT DF_Comments_Score
GO
CREATE TABLE dbo.Tmp_Comments
	(
	ID uniqueidentifier NOT NULL,
	ParentID uniqueidentifier NOT NULL,
	UserID nvarchar(128) NULL,
	CommentBody nvarchar(MAX) NOT NULL,
	Added datetime NOT NULL,
	Modified datetime NULL,
	Score int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Comments SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Comments ADD CONSTRAINT
	DF_Comments_Added DEFAULT (sysdatetime()) FOR Added
GO
ALTER TABLE dbo.Tmp_Comments ADD CONSTRAINT
	DF_Comments_Score DEFAULT ((0)) FOR Score
GO
IF EXISTS(SELECT * FROM dbo.Comments)
	 EXEC('INSERT INTO dbo.Tmp_Comments (ID, ParentID, UserID, CommentBody, Added, Modified, Score)
		SELECT ID, ParentID, UserID, CommentBody, Added, Modified, CONVERT(int, Score) FROM dbo.Comments WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Comments
GO
EXECUTE sp_rename N'dbo.Tmp_Comments', N'Comments', 'OBJECT' 
GO
ALTER TABLE dbo.Comments ADD CONSTRAINT
	PK_Comments PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Comments ADD CONSTRAINT
	FK_Comments_AspNetUsers FOREIGN KEY
	(
	UserID
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Answers
	DROP CONSTRAINT DF_Answers_Score
GO
ALTER TABLE dbo.Answers
	DROP CONSTRAINT DF_Answers_Added
GO
ALTER TABLE dbo.Answers
	DROP CONSTRAINT DF_Answers_Accepted
GO
CREATE TABLE dbo.Tmp_Answers
	(
	ID uniqueidentifier NOT NULL,
	UserID nvarchar(128) NULL,
	QuestionID uniqueidentifier NOT NULL,
	AnswerBody nvarchar(MAX) NOT NULL,
	Score int NOT NULL,
	Added datetime NOT NULL,
	Accepted bit NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Answers SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Answers ADD CONSTRAINT
	DF_Answers_Score DEFAULT ((0)) FOR Score
GO
ALTER TABLE dbo.Tmp_Answers ADD CONSTRAINT
	DF_Answers_Added DEFAULT (sysdatetime()) FOR Added
GO
ALTER TABLE dbo.Tmp_Answers ADD CONSTRAINT
	DF_Answers_Accepted DEFAULT ((0)) FOR Accepted
GO
IF EXISTS(SELECT * FROM dbo.Answers)
	 EXEC('INSERT INTO dbo.Tmp_Answers (ID, UserID, QuestionID, AnswerBody, Score, Added, Accepted)
		SELECT ID, UserID, QuestionID, AnswerBody, CONVERT(int, Score), Added, Accepted FROM dbo.Answers WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Answers
GO
EXECUTE sp_rename N'dbo.Tmp_Answers', N'Answers', 'OBJECT' 
GO
ALTER TABLE dbo.Answers ADD CONSTRAINT
	PK_Answers PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Answers ADD CONSTRAINT
	FK_Answers_Questions FOREIGN KEY
	(
	QuestionID
	) REFERENCES dbo.Questions
	(
	ID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.Answers ADD CONSTRAINT
	FK_Answers_AspNetUsers FOREIGN KEY
	(
	UserID
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ScoreItems
	DROP CONSTRAINT DF_ScoreItems_Score
GO
ALTER TABLE dbo.ScoreItems
	DROP CONSTRAINT DF_ScoreItems_Added
GO
CREATE TABLE dbo.Tmp_ScoreItems
	(
	ItemID uniqueidentifier NOT NULL,
	UserID nvarchar(128) NOT NULL,
	Score int NOT NULL,
	Added datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_ScoreItems SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_ScoreItems ADD CONSTRAINT
	DF_ScoreItems_Score DEFAULT ((0)) FOR Score
GO
ALTER TABLE dbo.Tmp_ScoreItems ADD CONSTRAINT
	DF_ScoreItems_Added DEFAULT (sysdatetime()) FOR Added
GO
IF EXISTS(SELECT * FROM dbo.ScoreItems)
	 EXEC('INSERT INTO dbo.Tmp_ScoreItems (ItemID, UserID, Score, Added)
		SELECT ItemID, UserID, CONVERT(int, Score), Added FROM dbo.ScoreItems WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.ScoreItems
GO
EXECUTE sp_rename N'dbo.Tmp_ScoreItems', N'ScoreItems', 'OBJECT' 
GO
ALTER TABLE dbo.ScoreItems ADD CONSTRAINT
	PK_ScoreItems PRIMARY KEY CLUSTERED 
	(
	ItemID,
	UserID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ScoreItems ADD CONSTRAINT
	FK_ScoreItems_AspNetUsers FOREIGN KEY
	(
	UserID
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT DF_UserLevel_UserLevelID
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT DF_UserLevel_Level
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT DF_UserLevel_Obtained
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT DF_UserLevel_XP
GO
CREATE TABLE dbo.Tmp_UserLevel
	(
	UserID nvarchar(128) NOT NULL,
	CategoryID uniqueidentifier NOT NULL,
	UserLevelID uniqueidentifier NOT NULL,
	[Level] int NOT NULL,
	Obtained datetime NOT NULL,
	XP int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_UserLevel SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_UserLevel ADD CONSTRAINT
	DF_UserLevel_UserLevelID DEFAULT (newid()) FOR UserLevelID
GO
ALTER TABLE dbo.Tmp_UserLevel ADD CONSTRAINT
	DF_UserLevel_Level DEFAULT ((0)) FOR [Level]
GO
ALTER TABLE dbo.Tmp_UserLevel ADD CONSTRAINT
	DF_UserLevel_Obtained DEFAULT (sysdatetime()) FOR Obtained
GO
ALTER TABLE dbo.Tmp_UserLevel ADD CONSTRAINT
	DF_UserLevel_XP DEFAULT ((0)) FOR XP
GO
IF EXISTS(SELECT * FROM dbo.UserLevel)
	 EXEC('INSERT INTO dbo.Tmp_UserLevel (UserID, CategoryID, UserLevelID, [Level], Obtained, XP)
		SELECT UserID, CategoryID, UserLevelID, [Level], Obtained, CONVERT(int, XP) FROM dbo.UserLevel WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.UserBadges
	DROP CONSTRAINT FK_UserBadges_UserLevel
GO
DROP TABLE dbo.UserLevel
GO
EXECUTE sp_rename N'dbo.Tmp_UserLevel', N'UserLevel', 'OBJECT' 
GO
ALTER TABLE dbo.UserLevel ADD CONSTRAINT
	PK_UserLevel_1 PRIMARY KEY CLUSTERED 
	(
	UserLevelID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.UserLevel ADD CONSTRAINT
	FK_UserLevel_AspNetUsers FOREIGN KEY
	(
	UserID
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.UserLevel ADD CONSTRAINT
	FK_UserLevel_Category FOREIGN KEY
	(
	CategoryID
	) REFERENCES dbo.Category
	(
	ID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.UserBadges ADD CONSTRAINT
	FK_UserBadges_Badges FOREIGN KEY
	(
	BadgeID
	) REFERENCES dbo.Badges
	(
	ID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.UserBadges ADD CONSTRAINT
	FK_UserBadges_UserLevel FOREIGN KEY
	(
	UserLevelID
	) REFERENCES dbo.UserLevel
	(
	UserLevelID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.UserBadges SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
