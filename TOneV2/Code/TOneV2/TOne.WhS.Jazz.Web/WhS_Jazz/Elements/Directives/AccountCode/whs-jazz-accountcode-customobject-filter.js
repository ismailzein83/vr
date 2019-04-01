(function (app) {

    'use strict';

    whsJazzAccountCodeCustomObjectFilter.$inject = ["UtilsService", "VRUIUtilsService","VR_GenericData_ListRecordFilterOperatorEnum"];

    function whsJazzAccountCodeCustomObjectFilter(UtilsService, VRUIUtilsService, VR_GenericData_ListRecordFilterOperatorEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onselectionchanged: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Jazz/Elements/Directives/AccountCode/Templates/AccountCodeCustomObjectRuntimeFilter.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var carriersSelectorAPI;
            var carriersSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onCarriersSelectorReady = function (api) {
                    carriersSelectorAPI = api;
                    carriersSelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.normalColNum = ctrl.normalColNum ? ctrl.normalColNum : 8;  
                
                defineAPI();
            }

            function loadCarriersSelector(payload) {
                var carriersSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                carriersSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(carriersSelectorAPI, payload, carriersSelectorLoadPromiseDeferred);
                });
                return carriersSelectorLoadPromiseDeferred.promise;
            } 

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var fieldValue = payload.fieldValue;
                    return loadCarriersSelector({ selectedIds: (fieldValue != undefined) ? fieldValue.CarriersIds : undefined });
                };

                api.getData = function () {
                   var data= {
                       $type: "TOne.WhS.Jazz.Entities.AccountCodeCarriersRecordFilter,TOne.WhS.Jazz.Entities",
                       CarriersIds: carriersSelectorAPI.getSelectedIds(),
                       CompareOperator: VR_GenericData_ListRecordFilterOperatorEnum.In.value
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsJazzAccountcodeCustomobjectFilter', whsJazzAccountCodeCustomObjectFilter);

})(app);
