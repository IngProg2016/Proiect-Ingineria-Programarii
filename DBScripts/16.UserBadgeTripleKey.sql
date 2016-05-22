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
	DROP CONSTRAINT FK_UserBadges_UserLevel
GO
ALTER TABLE dbo.UserLevel SET (LOCK_ESCALATION = TABLE)
GO
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
CREATE TABLE dbo.Tmp_UserBadges
	(
	UserLevelID uniqueidentifier NOT NULL,
	BadgeID uniqueidentifier NOT NULL,
	Obtained datetime NULL,
	ItemID uniqueidentifier NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_UserBadges SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.UserBadges)
	 EXEC('INSERT INTO dbo.Tmp_UserBadges (UserLevelID, BadgeID, Obtained, ItemID)
		SELECT UserLevelID, BadgeID, Obtained, ItemID FROM dbo.UserBadges WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.UserBadges
GO
EXECUTE sp_rename N'dbo.Tmp_UserBadges', N'UserBadges', 'OBJECT' 
GO
ALTER TABLE dbo.UserBadges ADD CONSTRAINT
	PK_UserBadges_1 PRIMARY KEY CLUSTERED 
	(
	UserLevelID,
	BadgeID,
	ItemID
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
	UserLevelID
	) REFERENCES dbo.UserLevel
	(
	UserLevelID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
