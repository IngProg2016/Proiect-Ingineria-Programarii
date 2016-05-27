using OutOfRange.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OutOfRange.Utils
{
    public class PointsUtils
    {
        /// <summary>
        /// Set user credits and xp on category; also adjusts user level on the given category if necessary.
        /// Give negative values if you need to subtract points
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <param name="credits"></param>
        /// <param name="xp"></param>
        public static void AddCreditsAndXP(string userId, Guid categoryId, int credits, int xp)
        {
            OutOfRangeEntities db = new OutOfRangeEntities();
            AspNetUser user = db.AspNetUsers.Single(x => x.Id == userId);
            user.Credits += credits;
            UserLevel userLevel;

            List<UserLevel> list = db.UserLevels.Where(x => x.UserID == userId).Where(x => x.CategoryID == categoryId).ToList();
            if (list.Count == 0)
            {
                userLevel = new UserLevel();
                userLevel.UserID = userId;
                userLevel.CategoryID = categoryId;
                userLevel.XP = 0;
                userLevel.Obtained = DateTime.Now;
                userLevel.UserLevelID = Guid.NewGuid();
                user.UserLevels.Add(userLevel);
                db.SaveChanges();
            }
            else
            {
                userLevel = list.ElementAt(0);
            }

            userLevel.XP += xp;

            db.Entry(userLevel).State = EntityState.Modified;
            db.Entry(user).State = EntityState.Modified;

            int prevLevel = userLevel.Level;
            int currentLevel = (int)Math.Floor(Math.Sqrt((double)userLevel.XP) / 5);
            if (prevLevel != currentLevel)
            {
                //do something here? notify user? have fun
                userLevel.Level = currentLevel;
                userLevel.Obtained = DateTime.Now;
            }
            db.SaveChanges();
        }

        public static void giveBadge(string userId, Guid categoryId, String identifierName, int rarity, Guid itemId)
        {
            OutOfRangeEntities db = new OutOfRangeEntities();
            AspNetUser user = db.AspNetUsers.Single(x => x.Id == userId);
            Badge badge = db.Badges.Where(x => (x.IdentifierName == identifierName && x.Rarity == rarity)).FirstOrDefault();
            if (user != null && badge != null)
            {
                UserLevel userLevel = user.UserLevels.Where(x => x.CategoryID == categoryId).FirstOrDefault();
                if (userLevel != null)
                {
                    userLevel.UserBadges.Add(new UserBadge()
                    {
                        Badge = badge,
                        Obtained = DateTime.Now,
                        ItemID = itemId
                    });
                    user.Credits += badge.Reward;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public static int getCurrentScoreBadgeRarity(int score)
        {
            if (score < 10)
            {
                return -1;
            }
            else if (score >= 10 && score < 50)
            {
                return 0;
            }
            else if (score >= 50 && score < 100)
            {
                return 1;
            }
            else if (score >= 100 && score < 500)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public static int getQuestionBadgeRarity(int questionNumber)
        {
            if (questionNumber == 1)
            {
                return 0;
            }
            else if (questionNumber == 10)
            {
                return 1;
            }
            else if (questionNumber == 100)
            {
                return 2;
            }
            else if (questionNumber == 500)
            {
                return 3;
            }
            //otherwise no badge for you
            return -1;
        }
    }
}