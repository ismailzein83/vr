﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class CreateZoneMatchTempTable : CodeActivity
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }        
       
        protected override void Execute(CodeActivityContext context)
        {
            IZoneMatchDataManager dataManager = DataManagerFactory.GetDataManager<IZoneMatchDataManager>();
            dataManager.CreateTempTable(this.IsFuture.Get(context));
        }
    }
}
