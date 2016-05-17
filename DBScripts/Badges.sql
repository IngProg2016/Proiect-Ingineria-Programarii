BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO
DECLARE
	@CategoryID uniqueidentifier,
	@CategoryName nvarchar(50);

DECLARE categories_cursor CURSOR 
		FOR SELECT ID, Name FROM Category

BEGIN
	OPEN categories_cursor;
	FETCH NEXT FROM categories_cursor
	INTO @CategoryID, @CategoryName;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO Badges
		(ID, CategoryID, Name, [Description], [Image], Reward)
		VALUES(NEWID(), @CategoryID, @CategoryName + ' Wood Score Badge', 'Condition: 10+ Score', NULL, 15);

		INSERT INTO Badges
		(ID, CategoryID, Name, [Description], [Image], Reward)
		VALUES(NEWID(), @CategoryID, @CategoryName + ' Bronze Score Badge', 'Condition: 50+ Score', NULL, 100);

		INSERT INTO Badges
		(ID, CategoryID, Name, [Description], [Image], Reward)
		VALUES(NEWID(), @CategoryID, @CategoryName + ' Silver Score Badge', 'Condition: 100+ Score', NULL, 250);

		INSERT INTO Badges
		(ID, CategoryID, Name, [Description], [Image], Reward)
		VALUES(NEWID(), @CategoryID, @CategoryName + ' Gold Score Badge', 'Condition: 500+ Score', NULL, 1500);

		INSERT INTO Badges
		(ID, CategoryID, Name, [Description], [Image], Reward)
		VALUES(NEWID(), @CategoryID, @CategoryName + ' Wood Views Badge', 'Condition: 100+ Views', NULL, 50);

		INSERT INTO Badges
		(ID, CategoryID, Name, [Description], [Image], Reward)
		VALUES(NEWID(), @CategoryID, @CategoryName + ' Bronze Views Badge', 'Condition: 300+ Views', NULL, 100);

		INSERT INTO Badges
		(ID, CategoryID, Name, [Description], [Image], Reward)
		VALUES(NEWID(), @CategoryID, @CategoryName + ' Silver Views Badge', 'Condition: 500+ Views', NULL, 300);

		INSERT INTO Badges
		(ID, CategoryID, Name, [Description], [Image], Reward)
		VALUES(NEWID(), @CategoryID, @CategoryName + ' Gold Views Badge', 'Condition: 1000+ Views', NULL, 500);

		FETCH NEXT FROM categories_cursor
		INTO @CategoryID, @CategoryName;
	END
	CLOSE categories_cursor;
	DEALLOCATE categories_cursor;
END
GO
COMMIT