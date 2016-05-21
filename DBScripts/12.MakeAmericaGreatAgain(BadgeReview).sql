/*
   21 mai 201619:21:20
   User: 
   Server: (localdb)\MSSQLLocalDB
   Database: OutOfRange
   Application: 
*/

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
ALTER TABLE dbo.Badges ADD
	ImageLoc nvarchar(MAX) NULL,
	IdentifierName nvarchar(MAX) NULL
GO
ALTER TABLE dbo.Badges SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/*
   21 mai 201619:11:18
   User: 
   Server: (localdb)\MSSQLLocalDB
   Database: OutOfRange
   Application: 
*/

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
ALTER TABLE dbo.Badges
	DROP CONSTRAINT FK_Badges_Category
GO
ALTER TABLE dbo.Category SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Badges
	DROP COLUMN CategoryID
GO
ALTER TABLE dbo.Badges SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

/*
   21 mai 201619:30:56
   User: 
   Server: (localdb)\MSSQLLocalDB
   Database: OutOfRange
   Application: 
*/

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
ALTER TABLE dbo.UserBadges
	DROP CONSTRAINT FK_UserBadges_Badges
GO
ALTER TABLE dbo.Badges SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT FK_UserLevel_Category
GO
ALTER TABLE dbo.Category SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.UserBadges
	DROP CONSTRAINT FK_UserBadges_AspNetUsers
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT FK_UserLevel_AspNetUsers
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_UserBadges
	(
	UserLevelID uniqueidentifier NOT NULL,
	BadgeID uniqueidentifier NOT NULL,
	Obtained datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_UserBadges SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.UserBadges)
	 EXEC('INSERT INTO dbo.Tmp_UserBadges (BadgeID, Obtained)
		SELECT BadgeID, Obtained FROM dbo.UserBadges WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.UserBadges
GO
EXECUTE sp_rename N'dbo.Tmp_UserBadges', N'UserBadges', 'OBJECT' 
GO
ALTER TABLE dbo.UserBadges ADD CONSTRAINT
	PK_UserBadges_1 PRIMARY KEY CLUSTERED 
	(
	UserLevelID,
	BadgeID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
	UserLevelID,
	BadgeID
	) REFERENCES dbo.UserBadges
	(
	UserLevelID,
	BadgeID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
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
	XP decimal(18, 0) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_UserLevel SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_UserLevel ADD CONSTRAINT
	DF_UserLevel_UserLevelID DEFAULT newid() FOR UserLevelID
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
	 EXEC('INSERT INTO dbo.Tmp_UserLevel (UserID, CategoryID, [Level], Obtained, XP)
		SELECT UserID, CategoryID, [Level], Obtained, XP FROM dbo.UserLevel WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.UserLevel
GO
EXECUTE sp_rename N'dbo.Tmp_UserLevel', N'UserLevel', 'OBJECT' 
GO
ALTER TABLE dbo.UserLevel ADD CONSTRAINT
	PK_UserLevel PRIMARY KEY CLUSTERED 
	(
	UserID,
	CategoryID,
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

/*
   21 mai 201619:36:34
   User: 
   Server: (localdb)\MSSQLLocalDB
   Database: OutOfRange
   Application: 
*/

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
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT PK_UserLevel
GO
ALTER TABLE dbo.UserLevel ADD CONSTRAINT
	PK_UserLevel_1 PRIMARY KEY CLUSTERED 
	(
	UserLevelID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.UserLevel SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.UserBadges
	DROP CONSTRAINT FK_UserBadges_UserLevel
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

/*
   21 mai 201619:53:20
   User: 
   Server: (localdb)\MSSQLLocalDB
   Database: OutOfRange
   Application: 
*/

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
ALTER TABLE dbo.Badges ADD
	Rarity int NOT NULL CONSTRAINT DF_Badges_Rarity DEFAULT ((0))
GO
ALTER TABLE dbo.Badges ADD CONSTRAINT
	DF_Badges_ID DEFAULT newid() FOR ID
GO
ALTER TABLE dbo.Badges SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'85a4b34c-55ad-40c5-ba34-1099560d3e2a', N'Wood Badge Of Scores', N'Obtained more than 10 score.', NULL, CAST(15 AS Decimal(18, 0)), N'woodbadge.png', N'woodscore', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'dff7b430-e4a7-4356-80ea-11ad64184882', N'FIRST QUESTION', N'Asked your first question !', NULL, CAST(10 AS Decimal(18, 0)), N'woodbadge.png', N'woodquestion', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'077a9840-8850-49f4-b371-38323ff587e0', N'Gold Badge Of Views', N'Obtained more than 1000 views.', NULL, CAST(500 AS Decimal(18, 0)), N'goldbadge.png', N'goldviews', 3)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'ab736c51-65cf-48a3-99b6-4f6bdcfe234f', N'Bronze Badge Of Views', N'Obtained more than 300 views.', NULL, CAST(100 AS Decimal(18, 0)), N'bronzebadge.png', N'bronzeviews', 1)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'7c1f8128-bb92-4990-9e4a-5612564cc38b', N'Bronze Badge Of Scores', N'Obtained more than 50 score.', NULL, CAST(100 AS Decimal(18, 0)), N'bronzebadge.png', N'bronzescores', 1)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'b55a8434-a494-4940-a3c9-7e3b390a1bf8', N'Wood Badge Of Views', N'Obtained more than 100 views.', NULL, CAST(50 AS Decimal(18, 0)), N'woodbadge.png', N'woodviews', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'428f86bb-db83-4b9c-9733-7f90694039fd', N'Silver Badge Of Views', N'Obtained more than 500 views.', NULL, CAST(300 AS Decimal(18, 0)), N'silverbadge.png', N'silverviews', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'64678676-202e-4bec-a4ec-a2a4e4f4df30', N'Hunger for Questions', N'Asked 100 questions !', NULL, CAST(500 AS Decimal(18, 0)), N'silverbadge.png', N'silverquestion', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'5886e148-78aa-46e0-b5d1-ad3cdd722ef4', N'Silver Badge Of Scores', N'Obtained more than 100 score.', NULL, CAST(250 AS Decimal(18, 0)), N'silverbadge.png', N'silverscore', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'e58b8a7c-b837-4e1b-82e6-c1bac17a8535', N'Gold Badge Of Scores', N'Obtained more than 500 score.', NULL, CAST(1500 AS Decimal(18, 0)), N'goldbadge.png', N'goldscore', 3)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'04312f2c-ecd3-424c-a693-f2b1358b452f', N'More Questions', N'Asked 10 questions !', NULL, CAST(50 AS Decimal(18, 0)), N'bronzebadge.png', N'bronzequestion', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'414ffab0-f752-4c14-bd70-fc76ba4d36e2', N'Question Guru', N'Asked 500 questions !', NULL, CAST(2500 AS Decimal(18, 0)), N'goldbadge.png', N'goldquestion', 3)
GO


