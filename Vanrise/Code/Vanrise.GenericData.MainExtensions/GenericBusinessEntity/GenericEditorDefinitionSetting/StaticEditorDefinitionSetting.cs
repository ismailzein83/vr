﻿using System;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class StaticEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("EC8B54D7-28AC-474F-B40A-D7AC02D89630"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-staticeditor-runtime"; } }

        public string DirectiveName { get; set; }
    }
}