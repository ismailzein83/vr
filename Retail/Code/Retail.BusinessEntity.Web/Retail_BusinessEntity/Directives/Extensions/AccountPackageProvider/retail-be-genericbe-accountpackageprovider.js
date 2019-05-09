'use strict';

app.directive('retailBeGenericbeAccountpackageprovider', ['UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_GenericData_GenericBEDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VRNavigationService, VR_GenericData_GenericBEDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PackageProviderCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/AccountPackageProvider/Templates/AccountPackageProviderTemplate.html"
        };

        function PackageProviderCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var context;
            var settings;
            var definitionSettings;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var beDefinitionSelectedPromiseDeferred;

            var idMappingFieldSelectorAPI;
            var idMappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountIdMappingFieldSelectorAPI;
            var accountIdMappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var bedMappingFieldSelectorAPI;
            var bedMappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var eedMappingFieldSelectorAPI;
            var eedMappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var packageMappingFieldSelectorAPI;
            var packageMappingFieldSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onIdMappingFieldSelectorReady = function (api) {
                    idMappingFieldSelectorAPI = api;
                    idMappingFieldSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAccountIdMappingFieldSelectorReady = function (api) {
                    accountIdMappingFieldSelectorAPI = api;
                    accountIdMappingFieldSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onBEDMappingFieldSelectorReady = function (api) {
                    bedMappingFieldSelectorAPI = api;
                    bedMappingFieldSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onEEDMappingFieldSelectorReady = function (api) {
                    eedMappingFieldSelectorAPI = api;
                    eedMappingFieldSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onPackageMappingFieldSelectorReady = function (api) {
                    packageMappingFieldSelectorAPI = api;
                    packageMappingFieldSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectorChanged = function (beDefinition) {
                    if (beDefinition != undefined) {
                        if (beDefinitionSelectedPromiseDeferred != undefined) {
                            beDefinitionSelectedPromiseDeferred.resolve();
                        }
                        else {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoading = value;
                            };
                            getGenericBEDefinitionSettings(beDefinition.BusinessEntityDefinitionId).then(function (genericBEDefinitionSettings) {
                                if (genericBEDefinitionSettings != undefined) {
                                    idMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                                        var idMappingFieldSelectorPayload = {
                                            dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId
                                        };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, idMappingFieldSelectorAPI, idMappingFieldSelectorPayload, setLoader, beDefinitionSelectedPromiseDeferred);
                                    });
                                    accountIdMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                                        var accountIdMappingFieldSelectorPayload = {
                                            dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId
                                        };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountIdMappingFieldSelectorAPI, accountIdMappingFieldSelectorPayload, setLoader, beDefinitionSelectedPromiseDeferred);
                                    });
                                    bedMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                                        var bedMappingFieldSelectorPayload = {
                                            dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId
                                        };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, bedMappingFieldSelectorAPI, bedMappingFieldSelectorPayload, setLoader, beDefinitionSelectedPromiseDeferred);
                                    });
                                    eedMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                                        var eedMappingFieldSelectorPayload = {
                                            dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId
                                        };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, eedMappingFieldSelectorAPI, eedMappingFieldSelectorPayload, setLoader, beDefinitionSelectedPromiseDeferred);
                                    });
                                    packageMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                                        var packageMappingFieldSelectorPayload = {
                                            dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId
                                        };
                                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, packageMappingFieldSelectorAPI, packageMappingFieldSelectorPayload, setLoader, beDefinitionSelectedPromiseDeferred);
                                    });
                                }
                            });
                        }
                    }
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        settings = payload;
                        if (settings != undefined) {
                            beDefinitionSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        }
                    }

                    function loadBusinessEntityDefinitionSelector() {
                        var beDefinitionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        beDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                            var beDefinitionSelectorPayload = {
                                selectedIds: settings != undefined ? settings.BusinessEntityDefinitionID : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, beDefinitionSelectorPayload, beDefinitionSelectorLoadPromiseDeferred);
                        });
                        return beDefinitionSelectorLoadPromiseDeferred.promise;
                    }
                    function loadIdMappingFieldSelector() {
                        var idMappingFieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        idMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                            var idMappingFieldSelectorPayload = {
                                dataRecordTypeId: definitionSettings != undefined ? definitionSettings.DataRecordTypeId : undefined,
                                selectedIds: settings != undefined ? settings.IDFieldName : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(idMappingFieldSelectorAPI, idMappingFieldSelectorPayload, idMappingFieldSelectorLoadPromiseDeferred);
                        });
                        return idMappingFieldSelectorLoadPromiseDeferred.promise;
                    }
                    function loadAccountIdMappingFieldSelector() {
                        var accountIdMappingFieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        accountIdMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                            var accountIdMappingFieldSelectorPayload = {
                                dataRecordTypeId: definitionSettings != undefined ? definitionSettings.DataRecordTypeId : undefined,
                                selectedIds: settings != undefined ? settings.AccountIDFieldName : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(accountIdMappingFieldSelectorAPI, accountIdMappingFieldSelectorPayload, accountIdMappingFieldSelectorLoadPromiseDeferred);
                        });
                        return accountIdMappingFieldSelectorLoadPromiseDeferred.promise;
                    }

                    function loadBEDMappingFieldSelector() {
                        var bedMappingFieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        bedMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                            var bedMappingFieldSelectorPayload = {
                                dataRecordTypeId: definitionSettings != undefined ? definitionSettings.DataRecordTypeId : undefined,
                                selectedIds: settings != undefined ? settings.BEDFieldName : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(bedMappingFieldSelectorAPI, bedMappingFieldSelectorPayload, bedMappingFieldSelectorLoadPromiseDeferred);
                        });
                        return bedMappingFieldSelectorLoadPromiseDeferred.promise;
                    }

                    function loadEEDMappingFieldSelector() {
                        var eedMappingFieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        eedMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                            var eedMappingFieldSelectorPayload = {
                                dataRecordTypeId: definitionSettings != undefined ? definitionSettings.DataRecordTypeId : undefined,
                                selectedIds: settings != undefined ? settings.EEDFieldName : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(eedMappingFieldSelectorAPI, eedMappingFieldSelectorPayload, eedMappingFieldSelectorLoadPromiseDeferred);
                        });
                        return eedMappingFieldSelectorLoadPromiseDeferred.promise;
                    }

                    function loadPackageMappingFieldSelector() {
                        var packageMappingFieldSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        packageMappingFieldSelectorReadyPromiseDeferred.promise.then(function () {
                            var packageMappingFieldSelectorPayload = {
                                dataRecordTypeId: definitionSettings != undefined ? definitionSettings.DataRecordTypeId : undefined,
                                selectedIds: settings != undefined ? settings.PackageFieldName : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(packageMappingFieldSelectorAPI, packageMappingFieldSelectorPayload, packageMappingFieldSelectorLoadPromiseDeferred);
                        });
                        return packageMappingFieldSelectorLoadPromiseDeferred.promise;
                    }


                    var rootPromiseNode = {
                        promises: [loadBusinessEntityDefinitionSelector()],
                        getChildNode: function () {
                            return {
                                promises: settings != undefined ? [getGenericBEDefinitionSettings(settings.BusinessEntityDefinitionID)] : [],
                                getChildNode: function () {
                                    return {
                                        promises: settings != undefined ? [loadIdMappingFieldSelector(), loadAccountIdMappingFieldSelector(), loadBEDMappingFieldSelector(), loadEEDMappingFieldSelector(), loadPackageMappingFieldSelector()] : []
                                    };
                                }
                            };
                        }
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                        beDefinitionSelectedPromiseDeferred = undefined;
                        settings = undefined;
                    });

                };
                api.getData = function () {
                    var obj = {
                        objectName: "AccountPackageProvider",
                        objectSettings: {
                            $type: "Retail.BusinessEntity.Business.RetailGenericBEAccountPackageProvider,Retail.BusinessEntity.Business",
                            BusinessEntityDefinitionID: beDefinitionSelectorAPI.getSelectedIds(),
                            AccountIDFieldName: accountIdMappingFieldSelectorAPI.getSelectedIds(),
                            BEDFieldName: bedMappingFieldSelectorAPI.getSelectedIds(),
                            EEDFieldName: eedMappingFieldSelectorAPI.getSelectedIds(),
                            IDFieldName: idMappingFieldSelectorAPI.getSelectedIds(),
                            PackageFieldName: packageMappingFieldSelectorAPI.getSelectedIds()
                        }
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getGenericBEDefinitionSettings(businessEntityDefinitionId) {
                var promiseDeferred = UtilsService.createPromiseDeferred();
                VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (genericBEDefinitionSettings) {
                    definitionSettings = genericBEDefinitionSettings;
                    promiseDeferred.resolve(genericBEDefinitionSettings);
                });
                return promiseDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);