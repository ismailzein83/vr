﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class AttachmentFieldTypeEntity
    {
        public long FileId { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedTime { get; set; }
    }
    public class AttachmentFieldTypeEntityChangeInfo
    {
        public string FileName { get; set; }
        public long FileId { get; set; }
        public string Description { get; set; }
    }
    public class AttachmentFieldTypeEntityCollection : List<AttachmentFieldTypeEntity>
    {

    }
}
