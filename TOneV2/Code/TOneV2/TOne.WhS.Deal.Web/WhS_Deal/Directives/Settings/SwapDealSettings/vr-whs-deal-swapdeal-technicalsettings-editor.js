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

            var componentTypeSelectorAPI;
            var componentTypePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onComponentTypeSelectorReady = function (api) {
                    componentTypeSelectorAPI = api;
                    componentTypePromiseDeferred.resolve();
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
                    var componentTypeSelectorLoadPromise = getComponentTypeSelectorLoadPromise();
                    promises.push(componentTypeSelectorLoadPromise);


                    function getComponentTypeSelectorLoadPromise() {
                        var componentTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        componentTypePromiseDeferred.promise.then(function () {

                            var componentTypeSelectorPayload;
                            if (swapDealTechnicalSettingData != undefined) {
                                componentTypeSelectorPayload = {
                                    filter: {
                                        Filters: [{
                                            $type: 'Vanrise.Common.Business.VRRuleDefinitionComponentTypeFilter, Vanrise.Common.Business'
                                        }]
                                    },
                                    selectedIds: swapDealTechnicalSettingData.SwapDealBuyRouteRuleDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(componentTypeSelectorAPI, componentTypeSelectorPayload, componentTypeSelectorLoadDeferred);
                        });

                        return componentTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: 'TOne.WhS.Deal.Entities.SwapDealTechnicalSettingData, TOne.WhS.Deal.Entities',
                        SwapDealBuyRouteRuleDefinitionId: componentTypeSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
    }]);