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
                    accountBEDefinitionId = mainpayload != undefined ? mainpayload.ExtendedSettings != undefined ? mainpayload.ExtendedSettings.AccountBEDefinitionId : undefined : undefined;

                    var businessEntityDefinitionSelectorLoadPromise = loadBusinessEntityDefinitionSelector();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);

                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);

                    return UtilsService.waitMultiplePromises([loadDirectivePromise, businessEntityDefinitionSelectorLoadPromise]);
                };

                api.getData = function getData() {
                    var data = directiveAPI.getData();
                    if (data != undefined)
                        data.AccountBEDefinitionId = beDefinitionSelectorAPI.getSelectedIds();
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
                    }
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