"use strict";

app.directive("vrWhsBeStatebackupSupplier", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var stateBackupSupplierConstructor = new StateBackupSupplierConstructor($scope, ctrl);
                stateBackupSupplierConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/StateBackup/Templates/SupplierStateBackupEditor.html"
        };

        function StateBackupSupplierConstructor($scope, ctrl) {

            var supplierSelectorAPI;
            var supplierSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onSupplierSelectorReady = function (api) {
                    supplierSelectorAPI = api;
                    supplierSelectorReadyPromiseDeferred.resolve();
                    defineAPI();
                }
            }

            function defineAPI() {

                var api = {};

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.SupplierStateBackupFilter, TOne.WhS.BusinessEntity.Entities",
                        SupplierIds : supplierSelectorAPI.getSelectedIds()
                };
            };

            api.load = function (payload) {
                return UtilsService.waitMultipleAsyncOperations([loadSupplierSelector])
                     .catch(function (error) {
                         VRNotificationService.notifyExceptionWithClose(error, $scope);
                     });

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadSupplierSelector() {

            var loadSupplierSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            supplierSelectorReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(supplierSelectorAPI, undefined, loadSupplierSelectorPromiseDeferred);
            });

            return loadSupplierSelectorPromiseDeferred.promise;
        }
         
        this.initializeController = initializeController;
    }

        return directiveDefinitionObject;
}]);
