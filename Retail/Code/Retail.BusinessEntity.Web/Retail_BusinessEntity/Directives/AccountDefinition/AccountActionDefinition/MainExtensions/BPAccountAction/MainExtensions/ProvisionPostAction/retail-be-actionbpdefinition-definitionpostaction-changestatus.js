(function (app) {

    'use strict';

    ChangeStatusDefinitionPostActionDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ChangeStatusDefinitionPostActionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ChangeStatusDefinitionPostAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/MainExtensions/ProvisionPostAction/Templates/ChangeStatusDefinitionPostActionTemplate.html"

        };
        function ChangeStatusDefinitionPostAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            var accountBEDefinitionId;
            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var existingStatusDefinitionsSelectorAPI;
            var existingStatusDefinitionsSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                    statusDefinitionSelectorAPI = api;
                    statusDefinitionSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onExistingStatusDefinitionsSelectorReady = function (api) {
                    existingStatusDefinitionsSelectorAPI = api;
                    existingStatusDefinitionsSelectorReadyDeferred.resolve();
                };
               
                UtilsService.waitMultiplePromises([statusDefinitionSelectorReadyDeferred.promise, existingStatusDefinitionsSelectorReadyDeferred.promise]).then(function () {
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
                            statusDefinitionSelectorPayload.selectedIds = accountProvisionDefinitionPostAction.NewStatusDefinitionId;
                        }
                        return statusDefinitionSelectorAPI.load(statusDefinitionSelectorPayload);
                    }
                    promises.push(loadStatusDefinitionSelector());

                    function loadExistingStatusDefinitionsSelector() {
                        var exisitngStatusDefinitionsSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter,Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: accountBEDefinitionId
                                }]
                            }
                        };
                        if (accountProvisionDefinitionPostAction != undefined)
                        {
                            exisitngStatusDefinitionsSelectorPayload.selectedIds = accountProvisionDefinitionPostAction.ExistingStatusDefinitionIds;
                        }
                        return existingStatusDefinitionsSelectorAPI.load(exisitngStatusDefinitionsSelectorPayload);
                    }
                    promises.push(loadExistingStatusDefinitionsSelector());
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.ChangeStatusDefinitionPostAction, Retail.BusinessEntity.MainExtensions",
                        NewStatusDefinitionId: statusDefinitionSelectorAPI.getSelectedIds(),
                        ExistingStatusDefinitionIds: existingStatusDefinitionsSelectorAPI.getSelectedIds(),
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionDefinitionpostactionChangestatus', ChangeStatusDefinitionPostActionDirective);

})(app);