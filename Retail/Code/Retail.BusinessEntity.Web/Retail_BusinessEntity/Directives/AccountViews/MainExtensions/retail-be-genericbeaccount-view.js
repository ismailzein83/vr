(function (app) {

    'use strict';

    GenericBEAccountViewDirective.$inject = ['UtilsService',  'Retail_BE_AccountBEAPIService'];

    function GenericBEAccountViewDirective(UtilsService, Retail_BE_AccountBEAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBEAccountViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/GenericBEAccountViewTemplate.html'
        };

        function GenericBEAccountViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var accountViewDefinitionSettings;
            var parentIdFieldValue;
            var parentBusinessEntityDefinitionId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.isGridLoading = true;
                    var gridPayload;

                    if (payload != undefined && payload.accountViewDefinition != undefined && payload.accountViewDefinition.Settings != undefined) {
                        parentBusinessEntityDefinitionId = payload.accountBEDefinitionId;
                        accountViewDefinitionSettings = payload.accountViewDefinition.Settings;
                        parentIdFieldValue = payload.parentAccountId;
                        var businessEntityDefinitionId = accountViewDefinitionSettings.BusinessEntityDefinitionId;
                        gridPayload = {
                            businessEntityDefinitionId: businessEntityDefinitionId,
                            fieldValues: buildMappingfields(),
                            filterValues: buildMappingfields()
                        };
                    }
                    return gridAPI.load(gridPayload).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function buildMappingfields() {

                var fields = {};
                var accountIdMappingField = accountViewDefinitionSettings.AccountIdMappingField;
                var accountBEDefinitionMappingField = accountViewDefinitionSettings.AccountBEDefinitionMappingField;

                if (accountIdMappingField != undefined) {
                    fields[accountIdMappingField] = {
                        value: parentIdFieldValue,
                        isHidden: true,
                        isDisabled: false
                    };
                }
                if (accountBEDefinitionMappingField != undefined) {
                    fields[accountBEDefinitionMappingField] = {
                        value: parentBusinessEntityDefinitionId,
                        isHidden: true,
                        isDisabled: false
                    };
                }
                return fields;
            }
        }
    }

    app.directive('retailBeGenericbeaccountView', GenericBEAccountViewDirective);

})(app);