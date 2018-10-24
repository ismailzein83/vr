'use strict';

app.directive('whsRoutesyncHuaweiSuppliermapping', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_RouteSync_TrunkTypeEnum',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_RouteSync_TrunkTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSupplierMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSynchronizer/Templates/HuaweiSupplierMappingTemplate.html'
        };

        function HuaweiSupplierMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var isFirstLoad = true;
            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.supplierMappingExists = false;

                $scope.scopeModel.isSupplierMappingExists = function () {
                    if (isFirstLoad)
                        return;

                    $scope.scopeModel.supplierMappingExists = isMappingExists();

                    updateSupplierDescriptions();
                };


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var supplierMapping;

                    if (payload != undefined) {
                        context = payload.context;

                        supplierMapping = payload.supplierMapping;
                        if (supplierMapping != undefined) {
                            $scope.scopeModel.routeName = supplierMapping.RouteName;
                            $scope.scopeModel.isup = supplierMapping.ISUP;
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.getData = function () {
                    return getSupplierMappingEntity();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function isMappingExists() {
                var isup = $scope.scopeModel.isup;
                var routeName = $scope.scopeModel.routeName;

                var isISUPFilled = isup != undefined && isup != "";
                var isRNFilled = routeName != undefined && routeName != "";

                if (isISUPFilled || isRNFilled) {
                    return true;
                } else {
                    return false;
                }
            }

            function updateSupplierDescriptions() {
                setTimeout(function () {
                    $scope.$apply(function () {
                        updateErrorDescription();
                        updateSupplierMappingDescription();
                    });
                }, 0);
            }

            function updateErrorDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                var validatationMessage = $scope.validationContext.validate();
                var isValid = validatationMessage == null;
                context.updateErrorDescription(isValid, true);
            }

            function updateSupplierMappingDescription() {
                if (context == undefined)
                    return;

                if ($scope.scopeModel.supplierMappingExists) {
                    context.updateSupplierMappingDescription(getSupplierMappingEntity());
                } else {
                    context.updateSupplierMappingDescription(null);
                }
            }

            function getSupplierMappingEntity() {

                if ($scope.scopeModel.isup == undefined && $scope.scopeModel.routeName == undefined)
                    return null;

                return {
                    ISUP: $scope.scopeModel.isup,
                    RouteName: $scope.scopeModel.routeName,
                };
            }
        }
    }]);