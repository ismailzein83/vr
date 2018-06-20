using System;

namespace Vanrise.Security.Entities
{
    public class RegisteredApplication
    {
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
    }
}