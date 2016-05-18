(function (appControllers) {

    'use strict';

    DataRecordStorageFilterEditorController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService', 'VRUIUtilsService','VRNotificationService'];

    function DataRecordStorageFilterEditorController($scope, VRNavigationService, VR_GenericData_DataRecordFieldAPIService, UtilsService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VRUIUtilsService, VRNotificationService) {

        var fields = [];
        var filterObj;
        var groupFilterAPI;
        var groupFilterReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                fields = parameters.Fields;
                filterObj = parameters.FilterObj;
            }
        }

        function defineScope() {
            $scope.title = 'Advanced Filter';
            $scope.onGroupFilterReady = function (api) {
                groupFilterAPI = api;
                groupFilterReadyDeferred.resolve();
            };

            $scope.save = function () {
                if ($scope.onDataRecordFieldTypeFilterAdded != undefined) {
                    $scope.onDataRecordFieldTypeFilterAdded(groupFilterAPI.getData(), groupFilterAPI.getExpression());
                }

                $scope.modalContext.closeModal();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.isLoading = true;

            loadDataRecordFieldTypeConfig().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadGroupFilter]).then(function () {

            }).finally(function () {
                $scope.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
        }

        function loadGroupFilter() {
            //var promises = [];

            var groupFilterLoadDeferred = UtilsService.createPromiseDeferred();

            groupFilterReadyDeferred.promise.then(function () {
                var payload = { context: buildContext(), filterObj: filterObj };
                VRUIUtilsService.callDirectiveLoad(groupFilterAPI, payload, groupFilterLoadDeferred);
            });
            return groupFilterLoadDeferred.promise;
            //promises.push(groupFilterLoadDeferred.promise);
            //return UtilsService.waitMultiplePromises(promises);
        }

        function buildContext() {
            var context = {
                getFields: function () { return fields },
                getRuleEditor: getRuleFilterEditorByFieldType
            }
            return context;
        }

        function getRuleFilterEditorByFieldType(configId) {
            var dataRecordFieldTypeConfig = UtilsService.getItemByVal($scope.dataRecordFieldTypesConfig, configId, 'DataRecordFieldTypeConfigId');
            if (dataRecordFieldTypeConfig != undefined) {
                return dataRecordFieldTypeConfig.RuleFilterEditor;
            }
        }

        function loadDataRecordFieldTypeConfig() {
            $scope.dataRecordFieldTypesConfig = [];
            return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                if (response) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.dataRecordFieldTypesConfig.push(response[i]);
                    }
                }
            });
        }
    }
    appControllers.controller('VR_GenericData_DataRecordTypeFieldFilterEditorController', DataRecordStorageFilterEditorController);

})(appControllers);