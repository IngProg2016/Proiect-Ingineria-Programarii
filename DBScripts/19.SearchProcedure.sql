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
			[Score] [decimal](18, 0),
			[Bounty] [decimal](18, 0),
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
		@Score decimal(18, 0),
		@Bounty decimal(18, 0),
		@Rank int

	OPEN question_cursor
	FETCH NEXT FROM question_cursor 
	INTO @ID, @UserID, @CategoryID, @Title, @QuestionBody, @Added, @Modified, @Score, @Bounty

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT @Rank = 0;

		--1 Check in the title
		SELECT @Rank += (COUNT(Name) * 7)  
		FROM Quesntions q
		WHERE q.ID = @ID AND t.Title LIKE '%' + @Terms + '%'

		--2 Check for tags
		SELECT @Rank += (COUNT(Name) * 6)  
		FROM Quesntions q
			INNER JOIN QuestionTags qt ON (q.ID = qt.QuestionID)
			INNER JOIN Tags t ON (qt.TagID = t.ID)
		WHERE q.ID = @ID AND t.Name LIKE '% ' + @Terms + ' %'

		--3 Check Question Body

		--4 Check Answers
		SELECT @Rank += (COUNT(Name) * 4)  
		FROM Quesntions q
			INNER JOIN Answers a ON (q.ID = a.QuestionID)
		WHERE q.ID = @ID AND a.AnswerBody LIKE '%' + @Terms + '%'

		--5 Check Users

		--6 Check Comments

		IF @Rank > 0
			INSERT INTO @temp_table(ID, UserID, CategoryID, Title, QuestionBody, Added, Modified, Score, Bounty, [Rank])
			VALUES(@ID, @UserID, @CategoryID, @Title, @QuestionBody, @Added, @Modified, @Score, @Bounty, @Rank)
	END
	CLOSE question_cursor;
	DEALLOCATE question_cursor;

	SELECT ID, UserID, CategoryID, Title, QuestionBody, Added, Modified, Score, Bounty FROM @temp_table ORDER BY Rank DESC
END