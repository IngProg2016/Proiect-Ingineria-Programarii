USE [OutOfRange]
delete from Badges;
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'85a4b34c-55ad-40c5-ba34-1099560d3e2a', N'Wood Badge Of Scores', N'Obtained more than 10 score.', NULL, CAST(15 AS Decimal(18, 0)), N'woodbadge.png', N'score', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'dff7b430-e4a7-4356-80ea-11ad64184882', N'FIRST QUESTION', N'Asked your first question !', NULL, CAST(10 AS Decimal(18, 0)), N'woodbadge.png', N'question', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'077a9840-8850-49f4-b371-38323ff587e0', N'Gold Badge Of Views', N'Obtained more than 1000 views.', NULL, CAST(500 AS Decimal(18, 0)), N'goldbadge.png', N'views', 3)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'ab736c51-65cf-48a3-99b6-4f6bdcfe234f', N'Bronze Badge Of Views', N'Obtained more than 300 views.', NULL, CAST(100 AS Decimal(18, 0)), N'bronzebadge.png', N'views', 1)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'7c1f8128-bb92-4990-9e4a-5612564cc38b', N'Bronze Badge Of Scores', N'Obtained more than 50 score.', NULL, CAST(100 AS Decimal(18, 0)), N'bronzebadge.png', N'scores', 1)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'b55a8434-a494-4940-a3c9-7e3b390a1bf8', N'Wood Badge Of Views', N'Obtained more than 100 views.', NULL, CAST(50 AS Decimal(18, 0)), N'woodbadge.png', N'views', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'428f86bb-db83-4b9c-9733-7f90694039fd', N'Silver Badge Of Views', N'Obtained more than 500 views.', NULL, CAST(300 AS Decimal(18, 0)), N'silverbadge.png', N'views', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'64678676-202e-4bec-a4ec-a2a4e4f4df30', N'Hunger for Questions', N'Asked 100 questions !', NULL, CAST(500 AS Decimal(18, 0)), N'silverbadge.png', N'question', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'5886e148-78aa-46e0-b5d1-ad3cdd722ef4', N'Silver Badge Of Scores', N'Obtained more than 100 score.', NULL, CAST(250 AS Decimal(18, 0)), N'silverbadge.png', N'score', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'e58b8a7c-b837-4e1b-82e6-c1bac17a8535', N'Gold Badge Of Scores', N'Obtained more than 500 score.', NULL, CAST(1500 AS Decimal(18, 0)), N'goldbadge.png', N'score', 3)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'04312f2c-ecd3-424c-a693-f2b1358b452f', N'More Questions', N'Asked 10 questions !', NULL, CAST(50 AS Decimal(18, 0)), N'bronzebadge.png', N'question', 1)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'414ffab0-f752-4c14-bd70-fc76ba4d36e2', N'Question Guru', N'Asked 500 questions !', NULL, CAST(2500 AS Decimal(18, 0)), N'goldbadge.png', N'question', 3)
GO
