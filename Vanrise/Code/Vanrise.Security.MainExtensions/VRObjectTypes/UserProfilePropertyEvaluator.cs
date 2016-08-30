﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Security.MainExtensions.VRObjectTypes
{
    public enum UserField { Email = 0, Name = 1, Description = 2, Status = 3}

    public class UserProfilePropertyEvaluator : VRObjectPropertyEvaluator
    {
        public UserField UserField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            User user = context.Object as User;

            if (user == null)
                throw new NullReferenceException("user");

            switch (this.UserField)
            {
                case UserField.Email: return user.Email;

                case UserField.Name: return user.Name;

                case UserField.Description: return user.Description;

                case UserField.Status: return user.Status;
            }

            return null;
        }
    }
}

