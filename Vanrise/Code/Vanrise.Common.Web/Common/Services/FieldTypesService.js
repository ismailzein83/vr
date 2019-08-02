(function (appControllers) {
    'use stict';
    FieldTypesService.$inject = [];
    function FieldTypesService() {

        function getTextFieldType() {
            return {
                $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType,Vanrise.GenericData.MainExtensions",
                ConfigId: "3f29315e-b542-43b8-9618-7de216cd9653",
                RuntimeEditor: "vr-genericdata-fieldtype-text-runtimeeditor",
                ViewerEditor: "vr-genericdata-fieldtype-text-viewereditor",
            };
        }
        function getNumberFieldType(numberDataType) {
            return {
                $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType,Vanrise.GenericData.MainExtensions",
                ConfigId: "75aef329-27bd-4108-b617-f5cc05ff2aa3",
                RuntimeEditor: "vr-genericdata-fieldtype-number-runtimeeditor",
                ViewerEditor: "vr-genericdata-fieldtype-number-viewereditor",
                DataType: numberDataType
            };
        }
        function getArrayFieldType(fieldType) {
            return {
                $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldArrayType,Vanrise.GenericData.MainExtensions",
                ConfigId: "034021E9-3BA1-4971-8AA9-CCF6ED2C2C80",
                RuntimeEditor: "vr-genericdata-fieldtype-array-runtimeeditor",
                FieldType: fieldType
            };
        }
        return {
            getTextFieldType: getTextFieldType,
            getNumberFieldType: getNumberFieldType,
            getArrayFieldType: getArrayFieldType
        };
    }

    appControllers.service('VRCommon_FieldTypesService', FieldTypesService);

})(appControllers);
