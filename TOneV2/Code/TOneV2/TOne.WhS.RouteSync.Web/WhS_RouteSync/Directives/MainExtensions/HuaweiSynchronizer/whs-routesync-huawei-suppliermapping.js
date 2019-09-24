﻿'use strict';

app.directive('whsRoutesyncHuaweiSuppliermapping', ['UtilsService',
    function (UtilsService) {
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

            var context;
            var isRouteNameValid = true;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.isSupplierMappingExists = function () {
                    $scope.scopeModel.supplierMappingExists = isMappingExists();
                    updateSupplierDescriptions();
                };

                $scope.scopeModel.isRouteNameValid = function () {

                    var routeNameValidationMsg = getRouteNameValidationMsg();
                    isRouteNameValid = routeNameValidationMsg == null;

                    var isSupplierMappingValid = isSupplierMappingValidFunc();

                    context.updateErrorDescription(isRouteNameValid && isSupplierMappingValid, false);
                    return routeNameValidationMsg;
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

                    $scope.scopeModel.supplierMappingExists = isMappingExists();

                    return UtilsService.waitMultiplePromises(promises);
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
                if (context == undefined)
                    return;

                var routeNameValidationMsg = getRouteNameValidationMsg();
                isRouteNameValid = routeNameValidationMsg == null;

                var isSupplierMappingValid = isSupplierMappingValidFunc();

                context.updateErrorDescription(isRouteNameValid && isSupplierMappingValid, false);
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
                var routeName = $scope.scopeModel.routeName;
                var isup = $scope.scopeModel.isup;

                if ((routeName == undefined || routeName == "") && (isup == undefined || isup == ""))
                    return null;

                return {
                    RouteName: routeName,
                    ISUP: isup
                };
            }

            function getRouteNameValidationMsg() {
                if (!$scope.scopeModel.supplierMappingExists)
                    return null;

                if (context == undefined || context.isRouteNameValid == undefined || typeof (context.isRouteNameValid) != 'function')
                    return null;

                return context.isRouteNameValid($scope.scopeModel.routeName);
            }

            function isSupplierMappingValidFunc() {
                if (!$scope.scopeModel.supplierMappingExists)
                    return true;

                var routeName = $scope.scopeModel.routeName;
                var isup = $scope.scopeModel.isup;
                if (routeName != undefined && routeName != "" && isup != undefined && isup != "")
                    return true;

                return false;
            }
        }
    }]);