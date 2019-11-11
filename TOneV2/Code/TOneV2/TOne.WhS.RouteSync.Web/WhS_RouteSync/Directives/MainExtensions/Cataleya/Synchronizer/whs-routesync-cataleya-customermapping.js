'use strict';

app.directive('whsRoutesyncCataleyaCustomermapping', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaCustomerMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/Cataleya/Synchronizer/Templates/CataleyaCustomerMappingTemplate.html'
        };

        function CataleyaCustomerMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var isFirstLoad = true;

            var trunkGridAPI;
            var trunkGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.inTrunks = [];

                $scope.scopeModel.onTrunkGridReady = function (api) {
                    trunkGridAPI = api;
                    trunkGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.addTrunk = function () {
                    extendTrunk();
                    updateCustomerDescriptions();
                };

                $scope.scopeModel.onTrunkDeleted = function (trunk) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.inTrunks, trunk.tempId, 'tempId');
                    $scope.scopeModel.inTrunks.splice(index, 1);
                    updateCustomerDescriptions();
                };

                $scope.scopeModel.areTrunksValid = function () {
                    var inTrunks = [];
                    for (var i = 0; i < $scope.scopeModel.inTrunks.length; i++) {
                        var inTrunk = $scope.scopeModel.inTrunks[i];
                        var index = UtilsService.getItemIndexByVal(inTrunks, inTrunk.Trunk, 'Trunk');

                        if (index > -1) {
                            return 'Trunk name must be unique';
                        }
                        inTrunks.push(inTrunk);
                    }

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    isFirstLoad = true;
                    var promises = [];

                    var customerMappings;
                    var inTrunks;

                    if (payload != undefined) {
                        context = payload.context;

                        customerMappings = payload.customerMappings;
                        if (customerMappings != undefined) {
                            inTrunks = customerMappings.InTrunks;
                        }

                        if (inTrunks != undefined && inTrunks.length > 0) {

                            for (var i = 0; i < inTrunks.length; i++) {
                                var inTrunk = inTrunks[i];
                                promises.push(extendTrunk(inTrunk));
                            }
                            updateCustomerDescriptions();
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.getData = function () {
                    return getCustomerMappings();
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendTrunk(inTrunk) {

                if (inTrunk == undefined)
                    inTrunk = {};

                inTrunk.tempId = UtilsService.guid();

                inTrunk.onTrunkBlur = function () {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.inTrunks, inTrunk.tempId, "tempId");
                    $scope.scopeModel.inTrunks[index] = inTrunk;
                    updateCustomerDescriptions();
                };

                inTrunk.onTrunkValueChanged = function () {
                    updateCustomerDescriptions();
                };

                $scope.scopeModel.inTrunks.push(inTrunk);

                return UtilsService.waitMultiplePromises([]);
            }

            function updateCustomerDescriptions() {
                setTimeout(function () {
                    $scope.$apply(function () {
                        updateCustomerMappingDescription();
                        updateIsCustomerMappingExists();
                    });
                }, 0);
            }

            function updateCustomerMappingDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                context.updateCustomerMappingDescription(getCustomerMappings());
            }

            function updateIsCustomerMappingExists() {
                if (isFirstLoad || context == undefined)
                    return;

                context.updateIsCustomerMappingExists($scope.scopeModel.inTrunks.length > 0);
            }

            function getCustomerMappings() {

                var inTrunks = [];

                for (var i = 0; i < $scope.scopeModel.inTrunks.length; i++) {
                    var inTrunk = $scope.scopeModel.inTrunks[i];
                    inTrunks.push({
                        Trunk: inTrunk.Trunk
                    });
                }

                return {
                    InTrunks: inTrunks
                };
            }
        }
    }]);