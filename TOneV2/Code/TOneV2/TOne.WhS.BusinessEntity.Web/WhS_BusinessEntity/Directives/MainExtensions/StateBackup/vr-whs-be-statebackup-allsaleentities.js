"use strict";

app.directive("vrWhsBeStatebackupAllsaleentities", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var stateBackupAllSaleEntitiesConstructor = new StateBackupAllSaleEntitiesConstructor($scope, ctrl);
                stateBackupAllSaleEntitiesConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/StateBackup/Templates/AllSaleEntitiesStateBackupEditor.html"
        };

        function StateBackupAllSaleEntitiesConstructor($scope, ctrl) {

            var sellingNumberPlanSelectorAPI;
            var sellingNumberPlanSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onsellingNumberPlanSelectorReady = function (api) {
                    sellingNumberPlanSelectorAPI = api;
                    sellingNumberPlanSelectorReadyPromiseDeferred.resolve();
                    defineAPI();
                };
            }

            function defineAPI() {

                var api = {};

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.AllSaleEntitiesStateBackupFilter, TOne.WhS.BusinessEntity.Entities",
                        SellingNumberPlanIds: sellingNumberPlanSelectorAPI.getSelectedIds()
                    };
                };

                api.load = function (payload) {
                    return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlanSelector])
                         .catch(function (error) {
                             VRNotificationService.notifyExceptionWithClose(error, $scope);
                         });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadSellingNumberPlanSelector() {

                var loadSellingNumberPlanSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                sellingNumberPlanSelectorReadyPromiseDeferred.promise.then(function () {

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanSelectorAPI, undefined, loadSellingNumberPlanSelectorPromiseDeferred);
                });

                return loadSellingNumberPlanSelectorPromiseDeferred.promise;
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
