(
function (appControllers) {
    "use strict";
    priceListConversionController.$inject = ['$window', '$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_SupPL_SupplierPriceListTemplateAPIService', 'WhS_BE_CarrierAccountAPIService', 'BusinessProcess_BPInstanceAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'WhS_SupPL_SupplierPriceListTypeEnum', 'WhS_BP_SPLDefinitionEnum', 'VRCommon_VRExclusiveSessionTypeService', 'VRCommon_VRExclusiveSessionTypeAPIService', 'WhS_BE_ExclusiveSessionTypeIdEnum'];
    function priceListConversionController($window, $scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_SupPL_SupplierPriceListAPIService, WhS_SupPL_SupplierPriceListTemplateAPIService, WhS_BE_CarrierAccountAPIService, BusinessProcess_BPInstanceAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, WhS_SupPL_SupplierPriceListTypeEnum, WhS_BP_SPLDefinitionEnum, VRCommon_VRExclusiveSessionTypeService, VRCommon_VRExclusiveSessionTypeAPIService, WhS_BE_ExclusiveSessionTypeIdEnum) {

        var inputWorkBookApi;
        var supplierPriceListConfigurationAPI;

        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencyDirectiveAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var priceListTemplateEntity;

        var exclusiveSessionObject = null;

        defineScope();
        load();

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.showMapping = true;

            $scope.scopeModel.priceListTypes = UtilsService.getArrayEnum(WhS_SupPL_SupplierPriceListTypeEnum);
            $scope.scopeModel.reset = function () {
                supplierPriceListConfigurationAPI.reset();
            };
            $scope.scopeModel.testConversion = function () {
                $scope.scopeModel.isLoading = true;
                var priceListTemplateObject =
                    {
                        FileId: $scope.scopeModel.inPutFile.fileId,
                        PricelistDate: $scope.scopeModel.priceListDate,
                        Settings: buildSupplierPriceListConfigurationObj()

                    };
                return WhS_SupPL_SupplierPriceListTemplateAPIService.TestConversionForSupplierPriceList(priceListTemplateObject).then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    UtilsService.downloadFile(response.data, response.headers);
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                currencyDirectiveAPI = api;
                currencyReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.carrierAccountSelectItem = function (dataItem) {
                releaseSession();
                if (dataItem == null)
                    return;
                $scope.scopeModel.isLoading = true;
                var selectedCarrierAccountId = dataItem.CarrierAccountId;
                hasRunningProcessesForSupplier(selectedCarrierAccountId).then(function (response) {
                    tryTakeSession(selectedCarrierAccountId).then(function (response) {
                        if (response.IsSucceeded) {
                            buildPricelistTemplate(selectedCarrierAccountId);
                        }
                        else onTryTakeFailure(response).then(function () {
                            $scope.isLoading = false;
                        });
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {

                });
            };

            $scope.scopeModel.onReadyWoorkBook = function (api) {
                inputWorkBookApi = api;
            };

            $scope.scopeModel.onSupplierPriceListConfigurationSelectiveReady = function (api) {
                supplierPriceListConfigurationAPI = api;
            };

            $scope.scopeModel.saveSupplierPriceListConfiguration = function () {
                if (priceListTemplateEntity)
                    return saveExistingPriceListTemplate();
                else
                    return saveNewPriceListTemplate();
            };

            $scope.scopeModel.startImport = function () {

                var promiseDeffered = UtilsService.createPromiseDeferred();
                var priceListTemplateObject = buildPriceListTemplateObjFromScope(true);
                if (!priceListTemplateEntity) {
                    insertPriceListTemplate(priceListTemplateObject).then(function (response) {
                        if (response != undefined && response.InsertedObject != undefined) {
                            priceListTemplateEntity = response.InsertedObject;
                            startImportSupplierPriceList(response.InsertedObject.SupplierPriceListTemplateId).then(function () {
                                promiseDeffered.resolve();
                            }).catch(function (error) {
                                promiseDeffered.reject(error);
                            });
                        } else {
                            promiseDeffered.resolve();
                        }
                    }).catch(function (error) {
                        promiseDeffered.reject(error);
                    });
                } else {
                    return updatePriceListTemplate(priceListTemplateObject).then(function (response) {
                        if (response != undefined && response.UpdatedObject != undefined) {
                            priceListTemplateEntity = response.UpdatedObject;
                            startImportSupplierPriceList(response.UpdatedObject.SupplierPriceListTemplateId).then(function () {
                                promiseDeffered.resolve();
                            }).catch(function (error) {
                                promiseDeffered.reject(error);
                            });
                        } else {
                            promiseDeffered.resolve();
                        }
                    }).catch(function (error) {
                        promiseDeffered.reject(error);
                    });
                }
                return promiseDeffered.promise;
            };

            $scope.scopeModel.hassaveSupplierPriceListConfigurationPermission = function () {
                if (priceListTemplateEntity)
                    return WhS_SupPL_SupplierPriceListTemplateAPIService.HasUpdateSupplierPriceListTemplatePermission();
                else
                    return WhS_SupPL_SupplierPriceListTemplateAPIService.HasaddSupplierPriceListTemplatePermission();
            };

            $window.onbeforeunload = function () {
                releaseSession();
            };
        }

        function startImportSupplierPriceList(supplierPriceListTemplateId) {
            var promiseDeffered = UtilsService.createPromiseDeferred();
            hasRunningProcessesForSupplier(carrierAccountDirectiveAPI.getSelectedIds()).then(function (response) {
                if (!response.hasRunningProcesses) {
                    createNewProcess(supplierPriceListTemplateId).then(function () {
                        promiseDeffered.resolve();
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                }
                else {
                    VRNotificationService.showWarning("Cannot start process because another instance is still running");
                    promiseDeffered.resolve();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

            return promiseDeffered.promise;
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountSelector, loadCurrencySelector])
           .catch(function (error) {
               $scope.scopeModel.isLoading = false;
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
           .finally(function () {
               $scope.scopeModel.isLoading = false;
           });
        }

        function loadCarrierAccountSelector() {
            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
            });

            return loadCarrierAccountPromiseDeferred.promise;

        }

        function loadCurrencySelector() {
            var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            currencyReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, undefined, loadCurrencySelectorPromiseDeferred);
            });
            return loadCurrencySelectorPromiseDeferred.promise;

        }
        function buildPricelistTemplate(selectedCarrierAccountId) {
            if (selectedCarrierAccountId != undefined) {
                $scope.scopeModel.showMapping = false;
                $scope.scopeModel.isLoadingCurrencySelector = true;
                $scope.scopeModel.inPutFile = undefined;
                WhS_BE_CarrierAccountAPIService.GetCarrierAccountCurrencyId(selectedCarrierAccountId).then(function (currencyId) {
                    currencyDirectiveAPI.selectedCurrency(currencyId);
                    $scope.scopeModel.isLoadingCurrencySelector = false;

                });

                getPriceListTemplate(selectedCarrierAccountId).then(function (response) {
                    $scope.scopeModel.showMapping = true;
                    priceListTemplateEntity = response;
                    var payload = {
                        context: buildContext(),
                        configDetails: response != undefined && response.ConfigDetails != undefined ? response.ConfigDetails : undefined
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingSupplierPriceListTemplate = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, supplierPriceListConfigurationAPI, payload, setLoader);
                });
            }
        }
        function hasRunningProcessesForSupplier(carrierAccountId) {
            var entityIds = [];
            entityIds.push(carrierAccountId);
            var hasRunningProcessesPromiseDeferred = UtilsService.createPromiseDeferred();
            WhS_BE_CarrierAccountAPIService.GetCarrierAccountName(carrierAccountId).then(function (carrierAccountName) {
                var runningInstanceEditorSettings = {
                    message: "Importing pricelist for supplier '" + carrierAccountName + "' is still pending"
                };
                BusinessProcess_BPInstanceService.displayRunningInstancesIfExist(WhS_BP_SPLDefinitionEnum.BPDefinitionId.value, entityIds, runningInstanceEditorSettings).then(function (response) {
                    hasRunningProcessesPromiseDeferred.resolve(response);
                });
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
            return hasRunningProcessesPromiseDeferred.promise;
        }
        function createNewProcess(supplierPriceListTemplateId) {
            var promiseDeffered = UtilsService.createPromiseDeferred();

            var inputArguments = {
                $type: "TOne.WhS.SupplierPriceList.BP.Arguments.SupplierPriceListProcessInput, TOne.WhS.SupplierPriceList.BP.Arguments",
                SupplierPriceListType: $scope.scopeModel.selectedPriceListType.value,
                SupplierAccountId: carrierAccountDirectiveAPI.getSelectedIds(),
                CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                FileId: $scope.scopeModel.inPutFile.fileId,
                PriceListDate: $scope.scopeModel.priceListDate,
                SupplierPriceListTemplateId: supplierPriceListTemplateId
            };
            var input = {
                InputArguments: inputArguments
            };
            BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                    var context = {
                        onClose: function () {
                        }
                    };

                    BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, context);

                }
                promiseDeffered.resolve();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

            return promiseDeffered.promise;
        }
        function getPriceListTemplate(selectedCarrierAccountId) {
            return WhS_SupPL_SupplierPriceListTemplateAPIService.GetSupplierPriceListTemplateBySupplierId(selectedCarrierAccountId);
        }

        function buildContext() {
            var context = {
                getSelectedCell: getSelectedCell,
                setSelectedCell: selectCellAtSheet,
                getSelectedSheet: getSelectedSheet,
                getFileId: getFileId
            };
            function selectCellAtSheet(row, col, s) {
                var a = parseInt(row);
                var b = parseInt(col);
                if (inputWorkBookApi != undefined && inputWorkBookApi.getSelectedSheetApi() != undefined)
                    inputWorkBookApi.selectCellAtSheet(a, b, s);
            }
            function getSelectedCell() {
                if (inputWorkBookApi != undefined && inputWorkBookApi.getSelectedSheetApi() != undefined)
                    return inputWorkBookApi.getSelectedSheetApi().getSelected();
            }
            function getSelectedSheet() {
                if (inputWorkBookApi != undefined)
                    return inputWorkBookApi.getSelectedSheet();
            }
            function getFileId() {
                if ($scope.scopeModel.inPutFile != undefined)
                    return $scope.scopeModel.inPutFile.fileId;
            }
            return context;
        }

        function buildSupplierPriceListConfigurationObj() {
            if (supplierPriceListConfigurationAPI != undefined)
                return supplierPriceListConfigurationAPI.getData();
        }

        function buildPriceListTemplateObjFromScope(isDraft) {
            var priceListTemplateObject = {
                SupplierPriceListTemplateId: priceListTemplateEntity != undefined ? priceListTemplateEntity.SupplierPriceListTemplateId : undefined,
                SupplierId: carrierAccountDirectiveAPI.getSelectedIds(),
            };

            if (!isDraft) {
                priceListTemplateObject.ConfigDetails = buildSupplierPriceListConfigurationObj();
            }
            if (isDraft) {
                if (priceListTemplateEntity != undefined && priceListTemplateEntity.ConfigDetails != undefined) {
                    priceListTemplateObject.ConfigDetails = priceListTemplateEntity.ConfigDetails;
                }
                priceListTemplateObject.Draft = buildSupplierPriceListConfigurationObj();
            }
            return priceListTemplateObject;
        }

        function saveNewPriceListTemplate() {
            $scope.scopeModel.isLoading = true;
            var priceListTemplateObject = buildPriceListTemplateObjFromScope();
            return insertPriceListTemplate(priceListTemplateObject).then(function (response) {
                if (response != undefined)
                    priceListTemplateEntity = response.InsertedObject;

                VRNotificationService.notifyOnItemAdded('Supplier Pricelist Template', response, 'Name');
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function insertPriceListTemplate(priceListTemplateObject) {
            return WhS_SupPL_SupplierPriceListTemplateAPIService.AddSupplierPriceListTemplate(priceListTemplateObject);
        }

        function saveExistingPriceListTemplate() {
            $scope.scopeModel.isLoading = true;
            var priceListTemplateObject = buildPriceListTemplateObjFromScope();
            return updatePriceListTemplate(priceListTemplateObject).then(function (response) {
                if (response != undefined)
                    priceListTemplateEntity = response.UpdatedObject;
                VRNotificationService.notifyOnItemAdded('Supplier Pricelist Template', response, 'Name');
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updatePriceListTemplate(priceListTemplateObject) {
            return WhS_SupPL_SupplierPriceListTemplateAPIService.UpdateSupplierPriceListTemplate(priceListTemplateObject);
        }

        function tryTakeSession(targetId) {
            var exclusiveSessionInput = {
                SessionTypeId: WhS_BE_ExclusiveSessionTypeIdEnum.Supplier.value,
                TargetId: targetId,
            };
            var promiseDeffered = UtilsService.createPromiseDeferred();
            VRCommon_VRExclusiveSessionTypeService.tryTakeSession($scope, exclusiveSessionInput).then(function (response) {
                promiseDeffered.resolve(response);
                if (response.IsSucceeded) {
                    response.onTryTakeFailure = onTryTakeFailure;
                    $scope.$on("$destroy", function () {
                        response.Release();
                    });
                    exclusiveSessionObject = response;
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
            return promiseDeffered.promise;
        }

        function onTryTakeFailure(failureObject) {
            return VRNotificationService.showPromptWarning(failureObject.FailureMessage).then(function () {
                $scope.scopeModel.selectedSupplier = null;
            });
        }

        function releaseSession() {
            if (exclusiveSessionObject != null) {
                exclusiveSessionObject.Release();
                exclusiveSessionObject = null;
            }
        }
    }

    appControllers.controller('WhS_SupPL_PriceListConversionController', priceListConversionController);
}
)(appControllers);