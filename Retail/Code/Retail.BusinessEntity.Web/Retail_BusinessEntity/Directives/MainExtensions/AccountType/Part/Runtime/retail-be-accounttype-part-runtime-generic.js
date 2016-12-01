'use strict';

app.directive('retailBeAccounttypePartRuntimeGeneric', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericUIRuntimeAPIService', function (UtilsService, VRUIUtilsService, VR_GenericData_GenericUIRuntimeAPIService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypFinancialGeneric = new AccountTypFinancialGeneric($scope, ctrl, $attrs);
            accountTypFinancialGeneric.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeGenericPartRuntimeTemplate.html'
    };

    function AccountTypFinancialGeneric($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var genericpartDirectiveAPI;
        var genericpartDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericPartDirectiveReady = function (api) {
                genericpartDirectiveAPI = api;
                genericpartDirectiveReadyPromiseDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];


                if (payload != undefined && payload.partDefinition != undefined && payload.partDefinition.Settings != undefined)
                {
                    var loadGenericPartPromiseDeferred = UtilsService.createPromiseDeferred();
                    getRuntimeEditorSections(payload.partDefinition.Settings).then(function (response) {
                        if(response !=undefined)
                        {
                            genericpartDirectiveReadyPromiseDeferred.promise.then(function () {
                                var directivePayload = { sections: response };
                                if (payload.partSettings != undefined)
                                    directivePayload.selectedValues = payload.partSettings.DataRecord;
                                VRUIUtilsService.callDirectiveLoad(genericpartDirectiveAPI, directivePayload, loadGenericPartPromiseDeferred);
                            });
                            promises.push(loadGenericPartPromiseDeferred.promise);
                        }
                    });
                    return UtilsService.waitMultiplePromises(promises);
                }

            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartGeneric,Retail.BusinessEntity.MainExtensions',
                    DataRecord: genericpartDirectiveAPI.getData()
                };
            };


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function getRuntimeEditorSections(partDefinitionSettings)
        {
            var input = {
                Sections: partDefinitionSettings.UISections,
                DataRecordTypeId: partDefinitionSettings.RecordTypeId
            };
            return VR_GenericData_GenericUIRuntimeAPIService.GetGenericEditorRuntimeSections(input);
        }
    }
}]);