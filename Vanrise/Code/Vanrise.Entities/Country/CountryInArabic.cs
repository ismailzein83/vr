using System;

namespace Vanrise.Entities
{
    public class CountryInArabic 
    {
        public int CountryInArabicId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedTime { get; set; }

        public int CreatedBy { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }
}