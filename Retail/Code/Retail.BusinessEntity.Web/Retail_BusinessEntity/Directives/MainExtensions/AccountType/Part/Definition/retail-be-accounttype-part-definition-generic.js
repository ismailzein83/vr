'use strict';

app.directive('retailBeAccounttypePartDefinitionGeneric', ["VRUIUtilsService", "UtilsService","VR_GenericData_DataRecordTypeAPIService", function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordTypeAPIService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeGenericPartDefinition = new AccountTypeGenericPartDefinition($scope, ctrl, $attrs);
            accountTypeGenericPartDefinition.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeGenericPartDefinitionTemplate.html'
    };

    function AccountTypeGenericPartDefinition($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var recordTypeEntity;
        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var selectedDataRecordTypeReadyPromiseDeferred;
        var currentPayload;

        var genericDirectiveAPI;
        var genericDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGenericDirectiveReady = function (api) {
                genericDirectiveAPI = api;
                genericDirectiveReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onDataRecordTypeSelectorDirectiveReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onRecordTypeSelectionChanged = function (value) {
                if (value != undefined) {
                    getDataRecordType(value.DataRecordTypeId).then(function () {
                        var payload = { recordTypeFields: recordTypeEntity.Fields };
                        var setLoader = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, genericDirectiveAPI, payload, setLoader, selectedDataRecordTypeReadyPromiseDeferred);
                    });
                }
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                currentPayload = payload != undefined ? payload.partDefinitionSettings : undefined;
                var promiseDeffered = UtilsService.createPromiseDeferred();

                selectedDataRecordTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                
                UtilsService.waitMultipleAsyncOperations([loadDataRecordTypeSelector, getDataRecordType]).then(function () {
                    loadGenericSection().then(function () {
                        promiseDeffered.resolve();
                    }).catch(function(error){
                        promiseDeffered.reject(error);
                    });
                });
                return promiseDeffered.promise;
            };

            api.getData = function () {
                var sections = genericDirectiveAPI.getData();
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartGenericDefinition,Retail.BusinessEntity.MainExtensions',
                    RecordTypeId: $scope.scopeModel.selectedDataRecordType.DataRecordTypeId,
                    UISections: sections !=undefined ?sections.Sections:undefined
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadDataRecordTypeSelector() {
            var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                var directivePayload = (currentPayload != undefined) ? { selectedIds: currentPayload.RecordTypeId } : undefined;
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                });

            return loadDataRecordTypeSelectorPromiseDeferred.promise;
        }

        function loadGenericSection() {
            if (currentPayload == undefined)
                selectedDataRecordTypeReadyPromiseDeferred.resolve();

            var loadGenericPromiseDeferred = UtilsService.createPromiseDeferred();
            UtilsService.waitMultiplePromises([genericDirectiveReadyPromiseDeferred.promise, selectedDataRecordTypeReadyPromiseDeferred.promise]).then(function () {
                var directivePayload = (currentPayload != undefined && recordTypeEntity != undefined) ? { sections: currentPayload.UISections, recordTypeFields: recordTypeEntity.Fields } : undefined;
                    genericDirectiveReadyPromiseDeferred = undefined;
                    selectedDataRecordTypeReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(genericDirectiveAPI, directivePayload, loadGenericPromiseDeferred);
                });
            return loadGenericPromiseDeferred.promise;
        }

        function getDataRecordType(dataRecordTypeId) {
            var dataRecordTypeIdValue = dataRecordTypeId;
            if (dataRecordTypeId == undefined && currentPayload != undefined)
                dataRecordTypeIdValue = currentPayload.RecordTypeId;

            if (dataRecordTypeIdValue == undefined)
                return;
            return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeIdValue).then(function (response) {
                recordTypeEntity = response;
            });
        }

    }
}]);