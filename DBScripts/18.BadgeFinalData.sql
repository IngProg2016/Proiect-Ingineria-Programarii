USE [OutOfRange]
GO
delete from Category;
GO
delete from Badges;
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'85a4b34c-55ad-40c5-ba34-1099560d3e2a', N'Wood Badge Of Scores', N'Obtained more than 10 score.', NULL, 15, N'woodbadge.png', N'score', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'dff7b430-e4a7-4356-80ea-11ad64184882', N'FIRST QUESTION', N'Asked your first question !', NULL, 10, N'woodbadge.png', N'question', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'077a9840-8850-49f4-b371-38323ff587e0', N'Gold Badge Of Views', N'Obtained more than 1000 views.', NULL, 500, N'goldbadge.png', N'views', 3)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'ab736c51-65cf-48a3-99b6-4f6bdcfe234f', N'Bronze Badge Of Views', N'Obtained more than 300 views.', NULL, 100, N'bronzebadge.png', N'views', 1)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'7c1f8128-bb92-4990-9e4a-5612564cc38b', N'Bronze Badge Of Scores', N'Obtained more than 50 score.', NULL, 100, N'bronzebadge.png', N'score', 1)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'b55a8434-a494-4940-a3c9-7e3b390a1bf8', N'Wood Badge Of Views', N'Obtained more than 100 views.', NULL, 50, N'woodbadge.png', N'views', 0)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'428f86bb-db83-4b9c-9733-7f90694039fd', N'Silver Badge Of Views', N'Obtained more than 500 views.', NULL, 300, N'silverbadge.png', N'views', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'64678676-202e-4bec-a4ec-a2a4e4f4df30', N'Hunger for Questions', N'Asked 100 questions !', NULL, 500, N'silverbadge.png', N'question', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'5886e148-78aa-46e0-b5d1-ad3cdd722ef4', N'Silver Badge Of Scores', N'Obtained more than 100 score.', NULL, 250, N'silverbadge.png', N'score', 2)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'e58b8a7c-b837-4e1b-82e6-c1bac17a8535', N'Gold Badge Of Scores', N'Obtained more than 500 score.', NULL, 1500, N'goldbadge.png', N'score', 3)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'04312f2c-ecd3-424c-a693-f2b1358b452f', N'More Questions', N'Asked 10 questions !', NULL, 50, N'bronzebadge.png', N'question', 1)
GO
INSERT [dbo].[Badges] ([ID], [Name], [Description], [Image], [Reward], [ImageLoc], [IdentifierName], [Rarity]) VALUES (N'414ffab0-f752-4c14-bd70-fc76ba4d36e2', N'Question Guru', N'Asked 500 questions !', NULL, 2500, N'goldbadge.png', N'question', 3)
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'00000000-0000-0000-0000-000000000000', N'Categorie Goala', N'Gategorie de test')
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'255176a8-3fb8-45a5-a22f-066011a4ad6e', N'Go', NULL)
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'dadb873a-b9e9-4c50-8905-0b1b7409a6f3', N'Java', NULL)
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'599df2dd-9b8d-4957-8c32-134a4fb11491', N'C++', NULL)
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'3ce4ace8-c9f9-4a19-9961-3191ab4a8748', N'Ruby', NULL)
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'1398b955-6005-4b33-bdec-51aee5e3a35b', N'C#', NULL)
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'fb1b37db-d0ba-455f-b123-5f346ef345c1', N'Javascript', NULL)
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'1f8b3ebf-d26a-4bff-8050-c4c07c91d7cd', N'Haskell', NULL)
GO
INSERT [dbo].[Category] ([ID], [Name], [Description]) VALUES (N'31dafab1-4655-415f-af24-dc1c01f855db', N'C', NULL)
GO
