'use strict';

app.directive('retailBeDidSynchronizerEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBeDidSynchronizerEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/DID/Templates/DIDSynchronizerEditor.html"
        };

        function retailBeDidSynchronizerEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountDefinitionSelectorApi = api;
                accountDefinitionSelectorPromiseDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var loadAccountDefinitionTypePromise = getAccountDefinitionSelectorLoadPromise();
                    promises.push(loadAccountDefinitionTypePromise);

                    function getAccountDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.BusinessEntity.Business.DIDSynchronizer, Retail.BusinessEntity.Business",
                        Name: "DID Synchronizer",
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);