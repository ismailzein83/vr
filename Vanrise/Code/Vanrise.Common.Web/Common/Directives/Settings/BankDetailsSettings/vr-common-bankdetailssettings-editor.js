'use strict';

app.directive('vrCommonBankdetailssettingsEditor', ['UtilsService', 'VRUIUtilsService','VRCommon_BankDetailService',
    function (UtilsService, VRUIUtilsService, VRCommon_BankDetailService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BankDetailsSettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/Settings/BankDetailsSettings/Templates/BankDetailsSettingsTemplate.html"
        };

        function BankDetailsSettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyPromiseDeferred.resolve();
                };

               

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var bankDetailsSettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        bankDetailsSettingsPayload = payload.data;
                    }

                    function loadGrid() {
                        var gridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        gridReadyPromiseDeferred.promise.then(function () {
                            var gridPayload = {
                                BankDetails: bankDetailsSettingsPayload != undefined ? bankDetailsSettingsPayload.BankDetails : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(gridAPI, gridPayload, gridLoadPromiseDeferred);

                        });
                        return gridLoadPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises([loadGrid()]);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.BankDetailsSettings, Vanrise.Entities",
                        BankDetails: gridAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }
         
         
        }
    }]);