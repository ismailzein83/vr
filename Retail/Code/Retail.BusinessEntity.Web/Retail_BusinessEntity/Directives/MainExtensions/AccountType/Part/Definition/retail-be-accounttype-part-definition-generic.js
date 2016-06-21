'use strict';

app.directive('retailBeAccounttypePartDefinitionGeneric', [function () {
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

    function AccountTypeGenericPartDefinition($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;
        var recordTypeEntity;
        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var genericDirectiveAPI;
        var genericDirectiveReadyPromiseDeferred  = UtilsService.createPromiseDeferred();
        function initializeController()
        {
            $scope.scopeModel = {};
            $scope.scopeModal.onGenericDirectiveReady = function (api) {
                genericDirectiveAPI = api;
                genericDirectiveReadyPromiseDeferred.resolve();
            }
            $scope.scopeModal.onDataRecordTypeSelectorDirectiveReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModal.onRecordTypeSelectionChanged = function () {
                var selectedDataRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                if (selectedDataRecordTypeId != undefined)
                {
                    getDataRecordType(selectedDataRecordTypeId).then(function () {
                        var payload = { recordTypeFields: recordTypeEntity.Fields };
                        var setLoader = function (value) { $scope.isLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, genericDirectiveAPI, payload, setLoader, genericDirectiveReadyPromiseDeferred);
                    });
                }
            }
            defineAPI();
        }
        function defineAPI()
        {
            var api = {};

            api.load = function (payload) {
                
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartGenericDefinition,Retail.BusinessEntity.MainExtensions',
                    RecordTypeId:dataRecordTypeSelectorAPI.getSelectedIds(),
                    UISections:genericDirectiveAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
    }
    function getDataRecordType(dataRecordTypeId)
    {
        return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
            recordTypeEntity = response;

        });
    }

    }
}]);