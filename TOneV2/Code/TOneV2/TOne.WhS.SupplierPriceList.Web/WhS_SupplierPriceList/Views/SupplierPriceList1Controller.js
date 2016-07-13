(
    function (appControllers) {
        "use strict";
        priceListConversionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_SupPL_SupplierPriceListTemplateService', 'WhS_SupPL_SupplierPriceListTemplateAPIService','WhS_BE_CarrierAccountAPIService','BusinessProcess_BPInstanceAPIService','WhS_BP_CreateProcessResultEnum','BusinessProcess_BPInstanceService'];
        function priceListConversionController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_SupPL_SupplierPriceListAPIService, WhS_SupPL_SupplierPriceListTemplateService, WhS_SupPL_SupplierPriceListTemplateAPIService, WhS_BE_CarrierAccountAPIService, BusinessProcess_BPInstanceAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService) {

            var inputWorkBookApi;
            var inputPriceListName;
            var inputConfigurationAPI;
            var inputConfigurationReadyPromiseDeferred;

            var inputPriceListTemplateAPI;
            var inputPriceListTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyDirectiveAPI;
            var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();
           
            var priceListTemplateEntity;

            defineScope();
            load();

            function defineScope() {

                $scope.scopeModel = {};

                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencyDirectiveAPI = api;
                    currencyReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.carrierAccountSelectItem = function (dataItem) {
                    var selectedCarrierAccountId = dataItem.CarrierAccountId;
                    if (selectedCarrierAccountId != undefined) {
                        $scope.scopeModel.isLoadingCurrencySelector = true;
                        $scope.scopeModel.inPutFile = undefined;
                        WhS_BE_CarrierAccountAPIService.GetCarrierAccountCurrency(selectedCarrierAccountId).then(function (currencyId) {

                            currencyDirectiveAPI.selectedCurrency(currencyId);
                            $scope.scopeModel.isLoadingCurrencySelector = false;
                        });

                        WhS_SupPL_SupplierPriceListTemplateAPIService.GetSupplierPriceListTemplateBySupplierId(selectedCarrierAccountId).then(function (response) {
                            priceListTemplateEntity = response;
                           
                            var payload = {
                                context: buildContext(),
                                configDetails: response != undefined ? response.ConfigDetails : undefined
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingInputPriceListTemplate = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, inputConfigurationAPI, payload, setLoader, inputConfigurationReadyPromiseDeferred);
                        });
                    }
                }

                $scope.scopeModel.onReadyWoorkBook = function (api) {
                    inputWorkBookApi = api;
                }

                $scope.scopeModel.onInputConfigurationSelectiveReady = function (api) {
                    inputConfigurationAPI = api;
                }

                $scope.scopeModel.saveInputConfiguration = function () {
                    if (priceListTemplateEntity)
                        return saveExistingPriceListTemplate();
                    else
                        return saveNewPriceListTemplate();
                }

                $scope.scopeModel.onInputPriceListTemplateSelectorReady = function (api) {
                    inputPriceListTemplateAPI = api;
                    inputPriceListTemplateReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.onInputPriceListTemplateSelectionChanged = function (api) {
                    if (inputPriceListTemplateAPI != undefined && inputPriceListTemplateAPI.getSelectedIds() == undefined) {
                        inputPriceListName = undefined;
                    }
                    if (inputPriceListTemplateAPI != undefined && !$scope.scopeModel.isLoading && inputConfigurationAPI != undefined && inputPriceListTemplateAPI.getSelectedIds() != undefined) {
                        $scope.scopeModel.isLoading = true;
                        getPriceListTemplate(inputPriceListTemplateAPI.getSelectedIds()).then(function (response) {

                            if (response) {
                                inputPriceListName = response.Name;
                                var payload = {
                                    context: buildContext(),
                                    configDetails: response.ConfigDetails
                                };
                                var setLoader = function (value) {
                                    $scope.scopeModel.isLoadingInputPriceListTemplate = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, inputConfigurationAPI, payload, setLoader, inputConfigurationReadyPromiseDeferred);
                            }


                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });

                    }

                }

                $scope.scopeModel.startImport = function () {
                    
                    var promiseDeffered = UtilsService.createPromiseDeferred();


                    if (!priceListTemplateEntity)
                    {
                        var priceListTemplateObject = buildPriceListTemplateObjFromScope(true);
                        insertPriceListTemplate(priceListTemplateObject).then(function (response) {
                            if (response != undefined && response.InsertedObject != undefined)
                            {
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
                        });;
                    }else
                    {
                        var priceListTemplateObject = buildPriceListTemplateObjFromScope(true);

                        return updatePriceListTemplate(priceListTemplateObject).then(function (response) {
                                if (response != undefined && response.UpdatedObject != undefined) {
                                    startImportSupplierPriceList(response.UpdatedObject.SupplierPriceListTemplateId).then(function () {
                                        promiseDeffered.resolve();
                                    }).catch(function (error) {
                                        promiseDeffered.reject(error);
                                    });
                                }else
                                {
                                    promiseDeffered.resolve();
                                }
                        }).catch(function (error) {
                            promiseDeffered.reject(error);
                        });
                    }
                    return promiseDeffered.promise;
                }

                function hassaveInputConfigurationPermission() {
                    return WhS_SupPL_SupplierPriceListTemplateAPIService.HassaveInputConfigurationPermission();
                };
               
            }

            function startImportSupplierPriceList(supplierPriceListTemplateId)
            {
                var promiseDeffered = UtilsService.createPromiseDeferred();

                var inputArguments = {
                    $type: "TOne.WhS.SupplierPriceList.BP.Arguments.SupplierPriceListProcessInput, TOne.WhS.SupplierPriceList.BP.Arguments",
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
                        }
                         BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, context).then(function () {
                            promiseDeffered.resolve();
                         }).catch(function (error) {
                             promiseDeffered.reject(error);
                         });
                    }else
                    {
                        promiseDeffered.resolve();
                    }
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

            function loadInputConfiguration() {
                var loadInputConfigurationPromiseDeferred = UtilsService.createPromiseDeferred();
                inputConfigurationReadyPromiseDeferred.promise.then(function () {
                    inputConfigurationReadyPromiseDeferred = undefined;
                    var payload = {
                        context: buildContext(),
                    };
                    VRUIUtilsService.callDirectiveLoad(inputConfigurationAPI, payload, loadInputConfigurationPromiseDeferred);
                });

                return loadInputConfigurationPromiseDeferred.promise;
            }

            function loadCarrierAccountSelector() {
                var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                carrierAccountReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred)
                });

                return loadCarrierAccountPromiseDeferred.promise;

            }

            function loadCurrencySelector() {
                var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                currencyReadyPromiseDeferred.promise.then(function () {
                   
                    VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, undefined, loadCurrencySelectorPromiseDeferred)

                })
                return loadCurrencySelectorPromiseDeferred.promise;

            }

            function loadInputPriceListTemplateSelector() {
                var loadInputPriceListTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                inputPriceListTemplateReadyPromiseDeferred.promise.then(function () {
                    var payload;
                    VRUIUtilsService.callDirectiveLoad(inputPriceListTemplateAPI, payload, loadInputPriceListTemplatePromiseDeferred);
                });

                return loadInputPriceListTemplatePromiseDeferred.promise;
            }

            function getPriceListTemplate(priceListTemplateId) {
                return WhS_SupPL_SupplierPriceListTemplateAPIService.GetPriceListTemplate(priceListTemplateId);
            }

            function buildContext() {
                var context = {
                    getSelectedCell: getSelectedCell,
                    setSelectedCell: selectCellAtSheet,
                    getSelectedSheet: getSelectedSheet
                }
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
                return context;
            }

            function buildInputConfigurationObj() {
                if (inputConfigurationAPI != undefined)
                    return inputConfigurationAPI.getData();
            }

            function buildPriceListTemplateObjFromScope(isDraft) {
                var priceListTemplateObject = {
                    SupplierPriceListTemplateId: priceListTemplateEntity != undefined ? priceListTemplateEntity.SupplierPriceListTemplateId : undefined,
                    SupplierId: carrierAccountDirectiveAPI.getSelectedIds(),
                };

                if(!isDraft)
                {
                   priceListTemplateObject.ConfigDetails =  buildInputConfigurationObj();
                }
                if(isDraft)
                {
                    if (priceListTemplateEntity != undefined && priceListTemplateEntity.ConfigDetails != undefined)
                    {
                        priceListTemplateObject.ConfigDetails = priceListTemplateEntity.ConfigDetails;
                    }
                    priceListTemplateObject.Draft = buildInputConfigurationObj();
                }
                return priceListTemplateObject;
            }

            function saveNewPriceListTemplate() {
                $scope.scopeModel.isLoading = true;
                var priceListTemplateObject = buildPriceListTemplateObjFromScope();
                return insertPriceListTemplate(priceListTemplateObject).then(function (response) {
                    VRNotificationService.notifyOnItemAdded('Supplier Price List Template', response, 'Name');
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
                    VRNotificationService.notifyOnItemAdded('Supplier Price List Template', response, 'Name');
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

            function updatePriceListTemplate(priceListTemplateObject) {
                return WhS_SupPL_SupplierPriceListTemplateAPIService.UpdateSupplierPriceListTemplate(priceListTemplateObject);
            }

            function validate(supplierPriceListInput) {
                return WhS_SupPL_SupplierPriceListAPIService.ValidateSupplierPriceList(supplierPriceListInput).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                }).catch(function (error) {
                });
            }

        }

        appControllers.controller('WhS_SupPL_PriceListConversionController', priceListConversionController);
    }
)(appControllers);