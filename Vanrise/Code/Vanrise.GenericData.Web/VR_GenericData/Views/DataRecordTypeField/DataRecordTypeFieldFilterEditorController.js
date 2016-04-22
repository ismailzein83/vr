(function (appControllers) {

    'use strict';

    DataRecordStorageFilterEditorController.$inject = ['$scope', 'VRNavigationService'];

    function DataRecordStorageFilterEditorController($scope, VRNavigationService) {

        $scope.dataRecordTypeId;
        loadParameters();
        defineScope();

        var context = { getFields: function () { return this.dataRecordFieldTypeConfigs }, dataRecordFieldTypeConfigs: [] };
        var groupFilterAPI;

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                $scope.dataRecordTypeId = parameters.DataRecordTypeId;
            }
        }

        function defineScope() {
            $scope.onGroupFilterReady = function (api) {
                groupFilterAPI = api;
                loadContext();
                var payload = { dataRecordTypeId: $scope.dataRecordTypeId, context: context };
                groupFilterAPI.load(payload);
            };

            $scope.save = function () {
                $scope.modalContext.closeModal();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function loadContext() {
            var obj1 = { name: 'TextField', editor: 'vr-genericdata-datarecordtypefield-texteditor' };
            var obj2 = { name: 'DecimalField', editor: 'vr-genericdata-datarecordtypefield-decimaleditor' };
            var obj3 = { name: 'DateTimeField', editor: 'vr-genericdata-datarecordtypefield-datetimeeditor' };
            var obj4 = { name: 'ChoiceField', editor: 'vr-genericdata-datarecordtypefield-choiceeditor', values: [{ Value: 1, Text: 'Val1' }, { Value: 2, Text: 'Val2' }, { Value: 3, Text: 'Val3' }] };
            var obj5 = { name: 'BoolField', editor: 'vr-genericdata-datarecordtypefield-booleditor' };
            var obj6 = { name: 'BusinessEntityField', editor: 'vr-genericdata-datarecordtypefield-businessEntityeditor', selector: 'vr-whs-be-customer-selector' };
            context.dataRecordFieldTypeConfigs.push(obj1);
            context.dataRecordFieldTypeConfigs.push(obj2);
            context.dataRecordFieldTypeConfigs.push(obj3);
            context.dataRecordFieldTypeConfigs.push(obj4);
            context.dataRecordFieldTypeConfigs.push(obj5);
            context.dataRecordFieldTypeConfigs.push(obj6);
        }
    }

    appControllers.controller('VR_GenericData_DataRecordTypeFieldFilterEditorController', DataRecordStorageFilterEditorController);

})(appControllers);