(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerDefinitionsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/MainExtensions/Templates/ChangeUsersRGsActionDefinitionTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var mainpayload;
            var accountBEDefinitionId;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([directiveReadyDeferred.promise, beDefinitionSelectorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainpayload = payload != undefined ? payload.Settings : undefined;
                    var promises = [];
                    if (mainpayload != undefined) {
                        var extendedSettings = mainpayload.ExtendedSettings;
                        if (extendedSettings != undefined) {
                            accountBEDefinitionId = extendedSettings.AccountBEDefinitionId;
                            $scope.scopeModel.companyFieldName = extendedSettings.CompanyFieldName;
                            $scope.scopeModel.branchFieldName = extendedSettings.BranchFieldName;
                            $scope.scopeModel.userFieldName = extendedSettings.UserFieldName;
                        }
                    }

                    var businessEntityDefinitionSelectorLoadPromise = loadBusinessEntityDefinitionSelector();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);

                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);

                    return UtilsService.waitMultiplePromises([loadDirectivePromise, businessEntityDefinitionSelectorLoadPromise]);
                };

                api.getData = function getData() {
                    var data = directiveAPI.getData();
                    if (data != undefined) {
                        data.AccountBEDefinitionId = beDefinitionSelectorAPI.getSelectedIds();
                        data.CompanyFieldName = $scope.scopeModel.companyFieldName;
                        data.BranchFieldName = $scope.scopeModel.branchFieldName;
                        data.UserFieldName = $scope.scopeModel.userFieldName;
                    }
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

            }

            function loadDirective() {
                if (mainpayload != undefined) {
                    mainpayload.classFQTN = "Retail.Teles.Business.ChangeUsersRGsActionDefinition, Retail.Teles.Business";
                } else {
                    mainpayload = {
                        classFQTN: "Retail.Teles.Business.ChangeUsersRGsActionDefinition, Retail.Teles.Business"
                    };
                }
                return directiveAPI.load(mainpayload);
            }

            function loadBusinessEntityDefinitionSelector() {

                var selectorPayload = {
                    filter: {
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                        }]
                    },
                    selectedIds: accountBEDefinitionId
                };

                return beDefinitionSelectorAPI.load(selectorPayload);
            }

        }
    }

    app.directive('retailTelesProvisionerActiondefinitionChangeusersrgs', ProvisionerDefinitionsettingsDirective);

})(app);