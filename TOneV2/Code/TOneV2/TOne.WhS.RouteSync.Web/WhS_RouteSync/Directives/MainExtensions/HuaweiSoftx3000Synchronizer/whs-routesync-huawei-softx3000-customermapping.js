(function (app) {

    'use strict';

    whsRoutesyncHuaweiSoftX3000Customermapping.$inject = ['UtilsService'];

    function whsRoutesyncHuaweiSoftX3000Customermapping(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSoftX3000CustomerMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSoftX3000Synchronizer/Templates/HuaweiSoftX3000CustomerMappingTemplate.html'
        };

        function HuaweiSoftX3000CustomerMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.isCustomerMappingExists = function () {
                    $scope.scopeModel.customerMappingExists = isMappingExists();
                    updateCustomerDescriptions();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var customerMapping;

                    if (payload != undefined) {
                        context = payload.context;
                        customerMapping = payload.customerMapping;

                        if (customerMapping != undefined) {
                            $scope.scopeModel.rssc = customerMapping.RSSC;
                            $scope.scopeModel.dnSet = customerMapping.DNSet;
                        }
                    }

                    $scope.scopeModel.customerMappingExists = isMappingExists();

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return getCustomerMappingEntity();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function isMappingExists() {
                var rssc = $scope.scopeModel.rssc;
                var dnSet = $scope.scopeModel.dnSet;

                var isRSSCFilled = rssc != undefined && rssc != "";
                var isDNsetFilled = dnSet != undefined && dnSet != "";

                if (isRSSCFilled || isDNsetFilled) {
                    return true;
                } else {
                    return false;
                }
            }

            function updateCustomerDescriptions() {
                setTimeout(function () {
                    $scope.$apply(function () {
                        updateErrorDescription();
                        updateCustomerMappingDescription();
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

            function updateCustomerMappingDescription() {
                if (context == undefined)
                    return;

                if ($scope.scopeModel.customerMappingExists) {
                    context.updateCustomerMappingDescription(getCustomerMappingEntity());
                } else {
                    context.updateCustomerMappingDescription(null);
                }
            }

            function getCustomerMappingEntity() {
                if ($scope.scopeModel.rssc == undefined && $scope.scopeModel.dnSet == undefined)
                    return null;

                return {
                    RSSC: $scope.scopeModel.rssc,
                    DNSet: $scope.scopeModel.dnSet
                };
            }
        }
    }

    app.directive('whsRoutesyncHuaweiSoftx3000Customermapping', whsRoutesyncHuaweiSoftX3000Customermapping);
})(app);