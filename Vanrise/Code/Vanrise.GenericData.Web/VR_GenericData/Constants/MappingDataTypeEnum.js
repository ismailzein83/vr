app.constant('VR_GenericData_MappingFieldTypeEnum', {
    DateTime: {
        name:"",
        description: "DateTime", value: {
            $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions",
            ConfigId: "b8712417-83ab-4d4b-9ee1-109d20ceb909",
            DataType: 0,
            IsNullable: true,
        }

    },
    Text: {
        description: "Text", value: {
            $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions",
            ConfigId: "3f29315e-b542-43b8-9618-7de216cd9653",
        }
    },
    Number: {
        description: "Number", value: {
            $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions",
            ConfigId: "75aef329-27bd-4108-b617-f5cc05ff2aa3",
            DataPrecision: 1,
            DataType: 0,
            IsNullable: false
        }
    }
});