'use strict';

app.directive('vrWhsBeGenericruledefinitionsettingsMarginrule', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new MarginRuleDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MarginRule/Templates/MarginRuleDefinitionSettings.html"
        };

        function MarginRuleDefinitionSettings($scope, ctrl, attrs) {

            this.initializeController = initializeController;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];
                    var marginCategoryBEDefinitionId;

                    if (payload != undefined) {
                        marginCategoryBEDefinitionId = payload.MarginCategoryBEDefinitionId;
                    }

                    var loadBEDefinitionSelectorPromise = loadBEDefinitionSelector();
                    initialPromises.push(loadBEDefinitionSelectorPromise);

                    function loadBEDefinitionSelector() {
                        var beDefinitionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorReadyDeferred.promise.then(function () {
                            var beDefinitionPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Common.Business.StatusDefinitionBEFilter, Vanrise.Common.Business"
                                    }]
                                }
                            };

                            if (marginCategoryBEDefinitionId != undefined) {
                                beDefinitionPayload.selectedIds = marginCategoryBEDefinitionId;
                            }

                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, beDefinitionPayload, beDefinitionSelectorLoadPromiseDeferred);
                        });

                        return beDefinitionSelectorLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: initialPromises });
                };

                api.getData = function () {
                    return {
                        $type: 'TOne.WhS.BusinessEntity.Entities.MarginRuleDefinitionSettings, TOne.WhS.BusinessEntity.Entities',
                        MarginCategoryBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

    }]);