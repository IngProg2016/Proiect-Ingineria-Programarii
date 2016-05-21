update Answers set Accepted=0 where Accepted is NULL;
ALTER TABLE [dbo].[Answers] ALTER COLUMN [Accepted] BIT NOT NULL;