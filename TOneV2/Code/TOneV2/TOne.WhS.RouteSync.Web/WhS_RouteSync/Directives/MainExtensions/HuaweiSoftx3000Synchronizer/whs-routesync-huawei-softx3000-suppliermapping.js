(function () {

    'use strict';

    whsRoutesyncHuaweiSoftX3000Suppliermapping.$inject = ['UtilsService']

    function whsRoutesyncHuaweiSoftX3000Suppliermapping(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSoftX3000SupplierMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSoftX3000Synchronizer/Templates/HuaweiSoftX3000SupplierMappingTemplate.html'
        };

        function HuaweiSoftX3000SupplierMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.isSupplierMappingExists = function () {
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
                            $scope.scopeModel.srt = supplierMapping.SRT;
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
                var srt = $scope.scopeModel.srt;
                var isSRTFilled = srt != undefined && srt != "";

                if (isSRTFilled) {
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
                var srt = $scope.scopeModel.srt;
                if (srt == undefined || srt == "")
                    return null;

                return {
                    SRT: srt
                };
            }
        }
    }

    app.directive('whsRoutesyncHuaweiSoftx3000Suppliermapping', whsRoutesyncHuaweiSoftX3000Suppliermapping);
})(app);