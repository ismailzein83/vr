(function (app) {

    'use strict';

    whsJazzRegionSettingsCustomObjectStaticEditor.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function whsJazzRegionSettingsCustomObjectStaticEditor(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onselectionchanged: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Jazz/Elements/Directives/RegionSettings/Templates/RegionSettingsCustomObjectStaticEditor.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

           
                $scope.scopeModel.addRegion = function (item) {
                    var entity = {};
                    
                    entity.onRegionSelectorReady = function (api) {
                        entity.regionDirectiveAPI = api;
                        var setLoader = function (value) { $scope.scopeModel.isRegionDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, undefined, setLoader);
                    };
                  
                    $scope.scopeModel.datasource.push(entity);
                };

                defineAPI();
            }
            function loadRateCalculationTypeSelector() {
                var rateCalculationTypeSelectorLoadromiseDeferred = UtilsService.createPromiseDeferred();
                rateCalculationTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(rateCalculationTypeSelectorAPI, undefined, rateCalculationTypeSelectorLoadromiseDeferred);
                });
                return rateCalculationTypeSelectorLoadromiseDeferred.promise;
            }
            //function prepareDataItem(market) {

            //    var entity = {
            //        data: market.payload
            //    };
            //    entity.onMarketSelectorReady = function (api) {
            //        entity.marketDirectiveAPI = api;
            //        market.marketReadyPromiseDeferred.resolve();
            //    };
            //    entity.onCustomerTypeSelectorReady = function (api) {
            //        entity.customerTypeDirectiveAPI = api;
            //        market.customerTypeReadyPromiseDeferred.resolve();
            //    };

            //    market.fieldStateReadyPromiseDeferred.promise.then(function () {
            //        var payloadFieldState = { selectedIds: genericBEFieldObject.payload.FieldState };
            //        VRUIUtilsService.callDirectiveLoad(entity.fieldStateDirectiveAPI, payloadFieldState, genericBEFieldObject.fieldStateLoadPromiseDeferred);
            //    });
            //    fieldSelectedPromiseDeferred.promise.then(function () {
            //        var item = UtilsService.getItemByVal($scope.scopeModel.selectedFields, genericBEFieldObject.payload.FieldName, "Name");
            //        entity.runtimeEditor = item.Type.RuntimeEditor;
            //        genericBEFieldObject.runtimeFieldReadyPromiseDeferred.promise.then(function () {

            //            var payloadRuntimeEditor = {
            //                fieldTitle: item.Title,
            //                fieldValue: genericBEFieldObject.payload.DefaultValue,
            //                fieldType: item.Type
            //            };
            //            VRUIUtilsService.callDirectiveLoad(entity.runtimeDirectiveAPI, payloadRuntimeEditor, genericBEFieldObject.runtimeFieldLoadPromiseDeferred);
            //        });
            //    });
            //    ctrl.datasource.push(entity);
            //}
            function defineAPI() {
                var api = {};

                api.load = function (payload) {console.log(payload)

                };

                api.setData = function (payload) {
                    var regionOptions;
                    if ($scope.scopeModel.datasource != undefined) {
                        regionOptions = [];
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                           var item = $scope.scopeModel.datasource[i];
                            regionOptions.push({
                                RegionCodeId: item.regionDirectiveAPI.getSelectedIds(),
                                Percentage: item.percentage
                            });
                        }
                    }
                    payload.RegionSettings = {
                        RegionOptions: regionOptions
                    }; console.log(payload)
                    return payload;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsJazzRegionsettingsCustomobjectStaticeditor', whsJazzRegionSettingsCustomObjectStaticEditor);

})(app);


