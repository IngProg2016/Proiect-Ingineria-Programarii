using OutOfRange.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OutOfRange.utils
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
        public static void addCreditsAndXP(String userId, Guid categoryId, int credits, decimal xp)
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
        
    }
}