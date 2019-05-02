﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class MainProxy
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        public void InvokeMethod(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                SaveErrorLog(ex);
                throw ex;
            }
        }

        public T InvokeMehtod<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                SaveErrorLog(ex);
                throw ex;
            }
        }

        private void SaveErrorLog(Exception ex)
        {
            EntitySchema schema = BPM_UserConnection.EntitySchemaManager.GetInstanceByName("StErrorLog");
            Entity errorLog = schema.CreateEntity(BPM_UserConnection);

            errorLog.SetColumnValue("Id", Guid.NewGuid());
            errorLog.SetColumnValue("StIP", BPM_UserConnection.CurrentUser.ClientIP);
            errorLog.SetColumnValue("StUserId", BPM_UserConnection.CurrentUser.ContactId);
            errorLog.SetColumnValue("StMessage", ex.Message);
            errorLog.SetColumnValue("StDetails", ex.ToString());
            errorLog.Save();
        }
    }
}
