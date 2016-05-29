DROP  PROCEDURE Search
GO
CREATE PROCEDURE Search
(
	@Terms nvarchar(512)
)
AS

BEGIN
	DECLARE question_cursor CURSOR FOR 
		SELECT q.ID, q.UserID, q.CategoryID, q.Title, q.QuestionBody, q.Added, q.Modified, q.Score, q.Bounty FROM Questions q;
	DECLARE @temp_table TABLE 
		(
			[ID] [uniqueidentifier],
			[UserID] [nvarchar](128),
			[CategoryID] [uniqueidentifier],
			[Title] [nvarchar](128),
			[QuestionBody] [nvarchar](max),
			[Added] [datetime],
			[Modified] [datetime],
			[Score] [int],
			[Bounty] [int],
			[Rank] [int]
		)
	DECLARE
		@ID uniqueidentifier,
		@UserID nvarchar(128),
		@CategoryID uniqueidentifier,
		@Title nvarchar(128),
		@QuestionBody nvarchar(max),
		@Added datetime,
		@Modified datetime,
		@Score int,
		@Bounty int,
		@Rank int,
		@Term nvarchar(128)

	OPEN question_cursor
	FETCH NEXT FROM question_cursor 
	INTO @ID, @UserID, @CategoryID, @Title, @QuestionBody, @Added, @Modified, @Score, @Bounty

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT @Rank = 0;

		--1 Check in the title
		SELECT @Rank += DIFFERENCE(q.Title, @Terms) * 7 FROM Questions q WHERE q.ID = @ID 

		--2 Check for tags
		SELECT @Rank += (COUNT(Name) * 6)  
		FROM Questions q
			INNER JOIN QuestionTags qt ON (q.ID = qt.QuestionID)
			INNER JOIN Tags t ON (qt.TagID = t.ID)
		WHERE q.ID = @ID AND @Terms LIKE '%' + t.Name + '%'

		--3 Check Question Body
		SELECT @Rank += DIFFERENCE(q.QuestionBody, @Terms) * 5 FROM Questions q WHERE q.ID = @ID 

		--4 Check Answers (a)
		SELECT @Rank += (COUNT(*) * 4)  
		FROM Questions q
			INNER JOIN Answers a ON (q.ID = a.QuestionID)
		WHERE q.ID = @ID AND a.Accepted = 1 AND a.AnswerBody LIKE '%' + @Terms + '%'

		--4 Check Answers (b)
		SELECT @Rank += (COUNT(*) * 3)  
		FROM Questions q
			INNER JOIN Answers a ON (q.ID = a.QuestionID)
		WHERE q.ID = @ID AND a.Accepted <> 1 AND a.AnswerBody LIKE '%' + @Terms + '%'

		--5 Check Users

		--6 Check Comments

		IF @Rank > 0
			INSERT INTO @temp_table(ID, UserID, CategoryID, Title, QuestionBody, Added, Modified, Score, Bounty, [Rank])
			VALUES(@ID, @UserID, @CategoryID, @Title, @QuestionBody, @Added, @Modified, @Score, @Bounty, @Rank)

		FETCH NEXT FROM question_cursor 
		INTO @ID, @UserID, @CategoryID, @Title, @QuestionBody, @Added, @Modified, @Score, @Bounty
	END
	CLOSE question_cursor;
	DEALLOCATE question_cursor;

	SELECT ID, UserID, CategoryID, Title, QuestionBody, Added, Modified, Score, Bounty, [Rank] FROM @temp_table ORDER BY Rank DESC
END