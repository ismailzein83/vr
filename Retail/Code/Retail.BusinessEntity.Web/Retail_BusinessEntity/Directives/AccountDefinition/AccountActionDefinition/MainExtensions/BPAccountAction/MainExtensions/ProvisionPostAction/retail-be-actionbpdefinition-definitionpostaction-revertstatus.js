(function (app) {

    'use strict';

    RevertStatusDefinitionPostActionDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RevertStatusDefinitionPostActionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RevertStatusDefinitionPostAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/MainExtensions/ProvisionPostAction/Templates/RevertStatusDefinitionPostActionTemplate.html"

        };
        function RevertStatusDefinitionPostAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var accountBEDefinitionId;

            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                    statusDefinitionSelectorAPI = api;
                    statusDefinitionSelectorReadyDeferred.resolve();
                };
              
                UtilsService.waitMultiplePromises([statusDefinitionSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var accountProvisionDefinitionPostAction;

                    if (payload != undefined) {
                        mainPayload = payload;
                        accountProvisionDefinitionPostAction = payload.accountProvisionDefinitionPostAction;
                        accountBEDefinitionId = payload.accountBEDefinitionId;

                    }
                    function loadStatusDefinitionSelector() {
                        var statusDefinitionSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter,Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: accountBEDefinitionId
                                }]
                            }
                        };

                        if (accountProvisionDefinitionPostAction != undefined) {
                            statusDefinitionSelectorPayload.selectedIds = accountProvisionDefinitionPostAction.RevertToStatusDefinitionId;
                        }
                        return statusDefinitionSelectorAPI.load(statusDefinitionSelectorPayload);
                    }
                    promises.push(loadStatusDefinitionSelector());
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.RevertStatusDefinitionPostAction, Retail.BusinessEntity.MainExtensions",
                        RevertToStatusDefinitionId: statusDefinitionSelectorAPI.getSelectedIds(),
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionDefinitionpostactionRevertstatus',RevertStatusDefinitionPostActionDirective);

})(app);