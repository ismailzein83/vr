app.constant('VR_GenericData_MappingFieldTypeEnum', {
    DateTime: {
        name:"",
        description: "DateTime", value: {
            $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDateTimeType, Vanrise.GenericData.MainExtensions",
            ConfigId: 3,
            DataType: 0,
            IsNullable: true,
        }

    },
    Text: {
        description: "Text", value: {
            $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType, Vanrise.GenericData.MainExtensions",
            ConfigId:1,
        }
    },
    Number: {
        description: "Number", value: {
            $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions",
            ConfigId:2,
            DataPrecision: 1,
            DataType: 0,
            IsNullable: false
        }
    },
});