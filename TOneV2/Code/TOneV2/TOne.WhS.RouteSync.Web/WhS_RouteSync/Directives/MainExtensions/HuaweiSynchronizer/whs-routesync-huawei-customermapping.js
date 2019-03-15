'use strict';

app.directive('whsRoutesyncHuaweiCustomermapping', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiCustomerMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSynchronizer/Templates/HuaweiCustomerMappingTemplate.html'
        };

        function HuaweiCustomerMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var isFirstLoad = true;
            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.customerMappingExists = false;

                $scope.scopeModel.isCustomerMappingExists = function () {
                    if (isFirstLoad)
                        return;
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
                            $scope.scopeModel.rssn = customerMapping.RSSN;
                            $scope.scopeModel.cscName = customerMapping.CSCName;
                            $scope.scopeModel.dnSet = customerMapping.DNSet;
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.getData = function () {
                    return getCustomerMappingEntity();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function isMappingExists() {
                var rssn = $scope.scopeModel.rssn;
                var cscName = $scope.scopeModel.cscName;
                var dnSet = $scope.scopeModel.dnSet;

                var isRSSNFilled = rssn != undefined && rssn != "";
                var isCSCNameFilled = cscName != undefined && cscName != "";
                var isDNsetFilled = dnSet != undefined && dnSet != "";
                if (isRSSNFilled || isCSCNameFilled || isDNsetFilled) {
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
                if (isFirstLoad || context == undefined)
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

                if ($scope.scopeModel.rssn == undefined && $scope.scopeModel.cscName == undefined && $scope.scopeModel.dnSet == undefined)
                    return null;

                return {
                    RSSN: $scope.scopeModel.rssn,
                    CSCName: $scope.scopeModel.cscName,
                    DNSet: $scope.scopeModel.dnSet
                };
            }
        }
    }]);