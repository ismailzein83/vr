(function (app) {

    'use strict';

    whsJazzAccountCodeCustomObjectFilterDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService','WhS_Jazz_AccountCodeAPIService'];

    function whsJazzAccountCodeCustomObjectFilterDirective(UtilsService, VRUIUtilsService, VRNotificationService, WhS_Jazz_AccountCodeAPIService) {
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
                $scope.scopeModel.normalColNum = ctrl.normalColNum;
                
                defineAPI();
            }

            function loadCarriersSelector() {
                var carriersSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                carriersSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(carriersSelectorAPI, undefined, carriersSelectorLoadPromiseDeferred);
                });
                return carriersSelectorLoadPromiseDeferred.promise;
            } 

            function defineAPI() {
                var api = {};

                api.load = function () {
                    return loadCarriersSelector();
                };

                api.getSelectedIds = function () {
                    var payload = {};
                    var Carriers = [];
                    if (carriersSelectorAPI != undefined && carriersSelectorAPI.getSelectedIds() != undefined) {
                        var selectedIds = carriersSelectorAPI.getSelectedIds();
                        for (var i = 0; i < selectedIds.length; i++) {
                            Carriers.push({
                                CarrierAccountId: selectedIds[i]
                            });
                        }
                    }
                    
                    payload.Carriers = {
                        $type:"TOne.WhS.Jazz.Entities.AccountCodeCarriers,TOne.WhS.Jazz.Entities",
                        Carriers: Carriers
                    };
                    return payload; 
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsJazzAccountcodeCustomobjectFilterDirective', whsJazzAccountCodeCustomObjectFilterDirective);

})(app);
