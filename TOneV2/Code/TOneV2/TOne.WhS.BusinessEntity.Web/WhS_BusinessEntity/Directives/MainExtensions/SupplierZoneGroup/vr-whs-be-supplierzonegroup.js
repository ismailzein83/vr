'use strict';
app.directive('vrWhsBeSupplierzonegroup', ['UtilsService', '$compile', 'WhS_BE_SupplierZoneAPIService', 'VRNotificationService', 'VRUIUtilsService',
function (UtilsService, $compile, WhS_BE_SupplierZoneAPIService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new beSupplierZoneGroup(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SupplierZoneGroup/Templates/SupplierZoneTemplate.html"

    };


    function beSupplierZoneGroup(ctrl, $scope, $attrs) {

        var payloadFilter;
        var supplierZoneGroupDirectiveAPI;
        var supplierZoneGroupDirectiveReadyPromiseDeferred;

        function initializeController() {
            $scope.onSupplierZoneGroupDirectiveReady = function (api) {
                supplierZoneGroupDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSupplierZoneGroupDirective = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierZoneGroupDirectiveAPI, payloadFilter, setLoader, supplierZoneGroupDirectiveReadyPromiseDeferred);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                $scope.supplierZoneGroupTemplates = [];
                var supplierZoneConfigId;
                var supplierZoneGroupPayload;
                if (payload != undefined) {
                    payloadFilter = {
                        supplierZoneFilterSettings: payload.supplierZoneFilterSettings
                    };

                    supplierZoneGroupPayload = {
                        supplierZoneFilterSettings: payload.supplierZoneFilterSettings,
                        supplierZoneGroupSettings: payload.supplierZoneGroupSettings != undefined ? payload.supplierZoneGroupSettings : payload
                    };
                    supplierZoneConfigId = payload.supplierZoneGroupSettings != undefined ? payload.supplierZoneGroupSettings.ConfigId : payload.ConfigId;
                }
                var promises = [];

                var loadSupplierZoneGroupTemplatesPromise = WhS_BE_SupplierZoneAPIService.GetSupplierZoneGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.supplierZoneGroupTemplates.push(item);
                    });

                    if (supplierZoneConfigId != undefined)
                        $scope.selectedSupplierZoneGroupTemplate = UtilsService.getItemByVal($scope.supplierZoneGroupTemplates, supplierZoneConfigId, "ExtensionConfigurationId");

                });
                promises.push(loadSupplierZoneGroupTemplatesPromise);

                if (supplierZoneConfigId != undefined) {
                    supplierZoneGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var supplierZoneGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(supplierZoneGroupDirectiveLoadPromiseDeferred.promise);

                    supplierZoneGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                        supplierZoneGroupDirectiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(supplierZoneGroupDirectiveAPI, supplierZoneGroupPayload, supplierZoneGroupDirectiveLoadPromiseDeferred);
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var supplierZoneGroupSettings;
                if ($scope.selectedSupplierZoneGroupTemplate != undefined) {
                    if (supplierZoneGroupDirectiveAPI != undefined) {
                        supplierZoneGroupSettings = supplierZoneGroupDirectiveAPI.getData();
                        supplierZoneGroupSettings.ConfigId = $scope.selectedSupplierZoneGroupTemplate.ExtensionConfigurationId;
                    }
                }
                return supplierZoneGroupSettings;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);