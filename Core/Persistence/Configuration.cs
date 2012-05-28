using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;
using Core.Model;

namespace Core.Persistence
{
    public class Configuration : DbMigrationsConfiguration<Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Context context)
        {
            var trackPointTypes = new List<TrackPointType> { 
                new TrackPointType(){ Code="VALLEY", Name="Valley"},
                new TrackPointType(){ Code="PEAK", Name="Peak" }
            };

            trackPointTypes.ForEach(item => context.TrackPointTypes.Add(item));

            context.SaveChanges();
        }
    }
}