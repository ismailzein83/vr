'use strict';

app.directive('retailRingoAccountConvertorEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBeAccountConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Ringo/Directives/MainExtensions/Account/Templates/AccountConvertorEditor.html"
        };

        function retailBeAccountConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var agentDefinitionSelectorApi;
            var agentDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var posDefinitionSelectorApi;
            var posDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var distributorDefinitionSelectorApi;
            var distributorDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onAgentDefinitionSelectorReady = function (api) {
                agentDefinitionSelectorApi = api;
                agentDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onPosDefinitionSelectorReady = function (api) {
                posDefinitionSelectorApi = api;
                posDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDistributorDefinitionSelectorReady = function (api) {
                distributorDefinitionSelectorApi = api;
                distributorDefinitionSelectorPromiseDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var agentDefinitionSelectorLoadPromise = getAgentDefinitionSelectorLoadPromise();
                    promises.push(agentDefinitionSelectorLoadPromise);

                    var posDefinitionSelectorLoadPromise = getPosDefinitionSelectorLoadPromise();
                    promises.push(posDefinitionSelectorLoadPromise);

                    var distributorDefinitionSelectorLoadPromise = getDistributorDefinitionSelectorLoadPromise();
                    promises.push(distributorDefinitionSelectorLoadPromise);

                    function getAgentDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        agentDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AgentBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(agentDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    function getPosDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        posDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.PosBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(posDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    function getDistributorDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        distributorDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.DistributorBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(distributorDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.Ringo.MainExtensions.AccountConvertor, Retail.Ringo.MainExtensions",
                        Name: "Account Convertor",
                        DistributorBEDefinitionId: distributorDefinitionSelectorApi.getSelectedIds(),
                        AgentBEDefinitionId: agentDefinitionSelectorApi.getSelectedIds(),
                        PosBEDefinitionId: posDefinitionSelectorApi.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);