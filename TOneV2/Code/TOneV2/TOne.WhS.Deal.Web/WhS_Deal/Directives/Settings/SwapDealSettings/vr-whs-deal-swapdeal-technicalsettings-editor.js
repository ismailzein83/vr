'use strict';

app.directive('vrWhsDealSwapdealTechnicalsettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SwapDealAnalysisTechnicalSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/Settings/SwapDealSettings/Templates/SwapDealTechnicalSettingsEditorTemplate.html'
        };

        function SwapDealAnalysisTechnicalSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var vrRuleDefinitionSelectorAPI;
            var vrRuleDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVRRuleDefinitionSelectorReady = function (api) {
                    vrRuleDefinitionSelectorAPI = api;
                    vrRuleDefinitionPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var swapDealTechnicalSettingData;

                    if (payload != undefined && payload.data != undefined) {
                        swapDealTechnicalSettingData = payload.data;
                    }

                    //Loading SupplierZone selector 
                    var vrRuleDefinitionSelectorLoadPromise = getVRRuleDefinitionSelectorLoadPromise();
                    promises.push(vrRuleDefinitionSelectorLoadPromise);


                    function getVRRuleDefinitionSelectorLoadPromise() {
                        var vrRuleDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        vrRuleDefinitionPromiseDeferred.promise.then(function () {

                            var vrRuleDefinitionSelectorPayload;
                            if (swapDealTechnicalSettingData != undefined) {
                                vrRuleDefinitionSelectorPayload = {
                                    selectedIds: swapDealTechnicalSettingData.SwapDealBuyRouteRuleDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(vrRuleDefinitionSelectorAPI, vrRuleDefinitionSelectorPayload, vrRuleDefinitionSelectorLoadDeferred);
                        });

                        return vrRuleDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: 'TOne.WhS.Deal.Entities.SwapDealTechnicalSettingData, TOne.WhS.Deal.Entities',
                        SwapDealBuyRouteRuleDefinitionId: vrRuleDefinitionSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }]);