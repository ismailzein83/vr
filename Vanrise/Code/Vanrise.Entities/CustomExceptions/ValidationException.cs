﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {

        }
    }

    public class MissingArgumentValidationException : ValidationException
    {
        public MissingArgumentValidationException(string message)
            : base(message)
        {

        }
    }
    
    public class DataIntegrityValidationException : ValidationException
    {
        public DataIntegrityValidationException(string message)
            : base(message)
        {

        }
    }

}
