'use strict';

app.directive('whsRoutesyncCataleyaSuppliermapping', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaSupplierMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/Cataleya/Synchronizer/Templates/CataleyaSupplierMappingTemplate.html'
        };

        function CataleyaSupplierMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var isFirstLoad = true;

            var trunkGridAPI;
            var trunkGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.outTrunks = [];

                $scope.scopeModel.onTrunkGridReady = function (api) {
                    trunkGridAPI = api;
                    trunkGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.addTrunk = function () {
                    extendTrunk();
                    updateSupplierDescriptions();
                };

                $scope.scopeModel.onTrunkDeleted = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.outTrunks, item.tempId, 'tempId');
                    $scope.scopeModel.outTrunks.splice(index, 1);
                    updateSupplierDescriptions();
                };

                $scope.scopeModel.areTrunksValid = function () {
                    var outTrunks = [];
                    var percentageSummation = 0;
                    var undefinedPercentageExists = false;

                    for (var i = 0; i < $scope.scopeModel.outTrunks.length; i++) {
                        var outTrunk = $scope.scopeModel.outTrunks[i];

                        var index = UtilsService.getItemIndexByVal(outTrunks, outTrunk.Trunk, 'Trunk');
                        if (index > -1) {
                            return 'Trunk name should be unique';
                        }

                        outTrunks.push(outTrunk);
                        percentageSummation += outTrunk.Percentage != undefined ? parseInt(outTrunk.Percentage) : 0;

                        if (outTrunk.Percentage == undefined) {
                            undefinedPercentageExists = true;
                        }
                    }

                    if (percentageSummation > 0 && undefinedPercentageExists) {
                        return 'All trunks must have percentage';
                    }

                    if (percentageSummation != 0 && percentageSummation != 100) {
                        return 'Percentages summation should be 100';
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

                    var supplierMappings;
                    var outTrunks;

                    if (payload != undefined) {
                        context = payload.context;

                        supplierMappings = payload.supplierMappings;
                        if (supplierMappings != undefined) {
                            outTrunks = supplierMappings.OutTrunks;
                        }

                        if (outTrunks != undefined && outTrunks.length > 0) {

                            for (var i = 0; i < outTrunks.length; i++) {
                                var outTrunk = outTrunks[i];
                                promises.push(extendTrunk(outTrunk));
                            }
                            updateSupplierDescriptions();
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.getData = function () {
                    return getSupplierMappings();
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendTrunk(outTrunk) {
                if (outTrunk == undefined)
                    outTrunk = {};

                outTrunk.tempId = UtilsService.guid();

                outTrunk.onTrunkBlur = function () {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.outTrunks, outTrunk.tempId, "tempId");
                    if (index == -1) {
                        $scope.scopeModel.outTrunks.push(outTrunk);
                    } else {
                        $scope.scopeModel.outTrunks[index] = outTrunk;
                    }
                    updateSupplierDescriptions();
                };

                outTrunk.onPercentageBlur = function () {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.outTrunks, outTrunk.tempId, "tempId");
                    if (index == -1) {
                        $scope.scopeModel.outTrunks.push(outTrunk);
                    } else {
                        $scope.scopeModel.outTrunks[index] = outTrunk;
                    }
                    updateSupplierDescriptions();
                };

                outTrunk.onTrunkValueChanged = function () {
                    updateSupplierDescriptions();
                };

                outTrunk.onPercentageValueChanged = function () {
                    updateSupplierDescriptions();
                };

                $scope.scopeModel.outTrunks.push(outTrunk);

                return UtilsService.waitMultiplePromises([]);
            }

            function updateSupplierDescriptions() {
                setTimeout(function () {
                    $scope.$apply(function () {
                        updateSupplierMappingDescription();
                        updateIsSupplierMappingExists();
                    });
                }, 0);
            }

            function updateSupplierMappingDescription() {
                if (isFirstLoad || context == undefined)
                    return;

                context.updateSupplierMappingDescription(getSupplierMappings());
            }

            function updateIsSupplierMappingExists() {
                if (isFirstLoad || context == undefined)
                    return;

                context.updateIsSupplierMappingExists($scope.scopeModel.outTrunks.length > 0);
            }

            function getSupplierMappings() {

                var outTrunks = [];

                for (var i = 0; i < $scope.scopeModel.outTrunks.length; i++) {
                    var outTrunk = $scope.scopeModel.outTrunks[i];
                    outTrunks.push({
                        Trunk: outTrunk.Trunk,
                        Percentage: outTrunk.Percentage
                    });
                }

                return {
                    OutTrunks: outTrunks
                };
            }
        }
    }]);