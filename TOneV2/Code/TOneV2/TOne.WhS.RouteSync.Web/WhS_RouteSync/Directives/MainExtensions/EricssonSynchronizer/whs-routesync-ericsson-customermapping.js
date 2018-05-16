'use strict';

app.directive('whsRoutesyncEricssonCustomermapping', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_RouteSync_TrunkTypeEnum',
    function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_RouteSync_TrunkTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EricssonCustomerMappingDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonCustomerMappingTemplate.html'
        };

        function EricssonCustomerMappingDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var isFirstLoad = true;
            var context;

            var trunkGridAPI;
            var trunkGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.trunks = [];
                $scope.scopeModel.customerMappingExists = false;
                $scope.scopeModel.trunkTypes = UtilsService.getArrayEnum(WhS_RouteSync_TrunkTypeEnum);

                $scope.scopeModel.onTrunkGridReady = function (api) {
                    trunkGridAPI = api;
                    trunkGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onTrunkAdded = function () {
                    $scope.scopeModel.trunks.push({
                        TrunkId: UtilsService.guid(),
                        TrunkName: undefined,
                        selectedTrunkType: UtilsService.getEnum(WhS_RouteSync_TrunkTypeEnum, 'value', 0),
                        //IsRouting: true
                    });

                    $scope.scopeModel.isCustomerMappingExists();
                };

                $scope.scopeModel.onTrunkDeleted = function (deleteItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.trunks, deleteItem.TrunkId, 'TrunkId');
                    $scope.scopeModel.trunks.splice(index, 1);

                    $scope.scopeModel.isCustomerMappingExists();
                };

                $scope.scopeModel.isCustomerMappingExists = function () {
                    if (isFirstLoad)
                        return;

                    if ($scope.scopeModel.trunks.length > 0) {
                        $scope.scopeModel.customerMappingExists = true;
                    } else {
                        $scope.scopeModel.customerMappingExists = isBOMappingExists();
                    }

                    updateCustomerDescriptions();
                };

                $scope.scopeModel.isTrunksValid = function () {
                    if (!isFirstLoad) {
                        var trunks = $scope.scopeModel.trunks;
                        if (trunks.length == 0) {
                            $scope.scopeModel.customerMappingExists = isBOMappingExists();
                            if ($scope.scopeModel.customerMappingExists == true) {
                                return "You should define at least one trunk";
                            }
                        } else {
                            var trunkNames = [];
                            for (var index = 0; index < trunks.length; index++) {
                                var currentTrunk = trunks[index];
                                if (trunkNames.includes(currentTrunk.TrunkName))
                                    return "Trunk Name is unique";

                                if (currentTrunk.TrunkName != undefined && currentTrunk.TrunkName != "")
                                    trunkNames.push(currentTrunk.TrunkName);
                            }
                        }
                    }
                    return null;
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
                            $scope.scopeModel.bo = customerMapping.BO;
                            $scope.scopeModel.nationalOBA = customerMapping.NationalOBA;
                            $scope.scopeModel.internationalOBA = customerMapping.InternationalOBA;
                        }
                    }

                    if (customerMapping != undefined && customerMapping.InTrunks != undefined) {
                        var trunkGridLoadPromise = getTrunkGridLoadPromise(customerMapping.InTrunks);
                        promises.push(trunkGridLoadPromise);
                    }

                    function getTrunkGridLoadPromise(trunks) {
                        var trunkGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        trunkGridReadyDeferred.promise.then(function () {
                            var _promises = [];

                            for (var index = 0; index < trunks.length; index++) {
                                var trunkTypeLoadSelectorDeferred = UtilsService.createPromiseDeferred();
                                _promises.push(trunkTypeLoadSelectorDeferred.promise);

                                var currentTrunk = trunks[index];
                                extendTrunkEntity(currentTrunk, trunkTypeLoadSelectorDeferred);
                                $scope.scopeModel.trunks.push(currentTrunk);
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                trunkGridLoadPromiseDeferred.resolve();
                            });
                        });

                        return trunkGridLoadPromiseDeferred.promise;
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

            function extendTrunkEntity(trunk, trunkTypeLoadSelectorDeferred) {
                trunk.onTrunkTypeSelectorReady = function (api) {
                    //trunk.trunkTypeSelectorGridAPI = api;
                    trunk.selectedTrunkType = UtilsService.getEnum(WhS_RouteSync_TrunkTypeEnum, 'value', trunk.TrunkType);
                    trunkTypeLoadSelectorDeferred.resolve();
                };
            }

            function isBOMappingExists() {
                var bo = $scope.scopeModel.bo;
                var nationalOBA = $scope.scopeModel.nationalOBA;
                var internationalOBA = $scope.scopeModel.internationalOBA;

                var isBOFilled = bo != undefined && bo != "";
                var isNationalOBAFilled = nationalOBA != undefined && nationalOBA != "";
                var isInternationalOBAFilled = internationalOBA != undefined && internationalOBA != "";

                if (isBOFilled || isNationalOBAFilled || isInternationalOBAFilled) {
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

                function getTrunks() {
                    var trunks = [];
                    for (var index = 0; index < $scope.scopeModel.trunks.length; index++) {
                        var currentTrunk = $scope.scopeModel.trunks[index];
                        trunks.push({
                            TrunkId: currentTrunk.TrunkId,
                            TrunkName: currentTrunk.TrunkName,
                            TrunkType: currentTrunk.selectedTrunkType.value,
                            //IsRouting: currentTrunk.IsRouting
                        });
                    }
                    return trunks.length > 0 ? trunks : undefined;
                }

                var inTrunks = getTrunks();
                if($scope.scopeModel.bo == undefined &&  $scope.scopeModel.nationalOBA == undefined && $scope.scopeModel.internationalOBA == undefined && inTrunks == undefined)
                    return undefined;

                return {
                    BO: $scope.scopeModel.bo,
                    NationalOBA: $scope.scopeModel.nationalOBA,
                    InternationalOBA: $scope.scopeModel.internationalOBA,
                    InTrunks: inTrunks
                };
            }
        }
    }]);