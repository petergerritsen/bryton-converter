﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Devtalk.EF.CodeFirst;

namespace bryton_convertor.Models
{
    public class SeedData : DontDropDbJustCreateTablesIfModelChanged<BrytonConvertorContext>
    {
        protected override void Seed(BrytonConvertorContext context)
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