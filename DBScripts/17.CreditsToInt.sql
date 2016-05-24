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
update AspNetUsers set Credits=0 where Credits is null
GO
ALTER TABLE dbo.AspNetUsers
	DROP CONSTRAINT DF_AspNetUsers_Credits
GO
CREATE TABLE dbo.Tmp_AspNetUsers
	(
	Id nvarchar(128) NOT NULL,
	Email nvarchar(256) NULL,
	EmailConfirmed bit NOT NULL,
	PasswordHash nvarchar(MAX) NULL,
	SecurityStamp nvarchar(MAX) NULL,
	PhoneNumber nvarchar(MAX) NULL,
	PhoneNumberConfirmed bit NOT NULL,
	TwoFactorEnabled bit NOT NULL,
	LockoutEndDateUtc datetime NULL,
	LockoutEnabled bit NOT NULL,
	AccessFailedCount int NOT NULL,
	UserName nvarchar(256) NOT NULL,
	Credits int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_AspNetUsers ADD CONSTRAINT
	DF_AspNetUsers_Credits DEFAULT ((0)) FOR Credits
GO
IF EXISTS(SELECT * FROM dbo.AspNetUsers)
	 EXEC('INSERT INTO dbo.Tmp_AspNetUsers (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, Credits)
		SELECT Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, CONVERT(int, Credits) FROM dbo.AspNetUsers WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.Comments
	DROP CONSTRAINT FK_Comments_AspNetUsers
GO
ALTER TABLE dbo.Answers
	DROP CONSTRAINT FK_Answers_AspNetUsers
GO
ALTER TABLE dbo.ScoreItems
	DROP CONSTRAINT FK_ScoreItems_AspNetUsers
GO
ALTER TABLE dbo.UserLevel
	DROP CONSTRAINT FK_UserLevel_AspNetUsers
GO
ALTER TABLE dbo.QuestionViews
	DROP CONSTRAINT FK_QuestionViews_AspNetUsers
GO
ALTER TABLE dbo.AspNetUserClaims
	DROP CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE dbo.AspNetUserLogins
	DROP CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE dbo.AspNetUserRoles
	DROP CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE dbo.Questions
	DROP CONSTRAINT FK_Questions_AspNetUsers
GO
DROP TABLE dbo.AspNetUsers
GO
EXECUTE sp_rename N'dbo.Tmp_AspNetUsers', N'AspNetUsers', 'OBJECT' 
GO
ALTER TABLE dbo.AspNetUsers ADD CONSTRAINT
	[PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
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
ALTER TABLE dbo.Questions SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.AspNetUserRoles ADD CONSTRAINT
	[FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.AspNetUserRoles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.AspNetUserLogins ADD CONSTRAINT
	[FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.AspNetUserLogins SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.AspNetUserClaims ADD CONSTRAINT
	[FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY
	(
	UserId
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.AspNetUserClaims SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.QuestionViews ADD CONSTRAINT
	FK_QuestionViews_AspNetUsers FOREIGN KEY
	(
	UserID
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.QuestionViews SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
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
ALTER TABLE dbo.UserLevel SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
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
ALTER TABLE dbo.ScoreItems SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
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
ALTER TABLE dbo.Answers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
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
ALTER TABLE dbo.Comments SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
