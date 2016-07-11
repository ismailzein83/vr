(
    function (appControllers) {
        "use strict";
        priceListConversionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_SupPL_SupplierPriceListTemplateService', 'WhS_SupPL_SupplierPriceListTemplateAPIService','WhS_BE_CarrierAccountAPIService'];
        function priceListConversionController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_SupPL_SupplierPriceListAPIService, WhS_SupPL_SupplierPriceListTemplateService, WhS_SupPL_SupplierPriceListTemplateAPIService, WhS_BE_CarrierAccountAPIService) {

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

                $scope.scopeModel.validate = function () {

                    var onOutputPriceListTemplateChoosen = function (choosenPriceListTemplateObj) {
                        if (choosenPriceListTemplateObj != undefined && choosenPriceListTemplateObj.pricelistTemplateIds != undefined) {
                            $scope.scopeModel.isLoading = true;
                            var promises = [];
                            var requests = [];
                            for (var i = 0; i < choosenPriceListTemplateObj.pricelistTemplateIds.length; i++) {

                                var fileName = UtilsService.getUploadedFileName($scope.scopeModel.inPutFile.fileName);

                                var supplierPriceListInput = {
                                    InputFileId: $scope.scopeModel.inPutFile.fileId,
                                    SupplierPriceListSettings: buildInputConfigurationObj(),
                                }
                                promises.push(validate(supplierPriceListInput));
                            }
                            return UtilsService.waitMultiplePromises(promises).finally(function () {
                                $scope.scopeModel.isLoading = false;;
                            }).catch(function (error) {
                            });
                        }
                    };
                 
                }

                $scope.scopeModel.saveInputConfiguration = function () {
                    if (priceListTemplateEntity)
                        return updatePriceListTemplate();
                    else
                        return insertPriceListTemplate();
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

                function hassaveInputConfigurationPermission() {
                    return WhS_SupPL_SupplierPriceListTemplateAPIService.HassaveInputConfigurationPermission();
                };
               
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

            function buildPriceListTemplateObjFromScope() {
                var priceListTemplateObject = {
                    SupplierPriceListTemplateId: priceListTemplateEntity != undefined ? priceListTemplateEntity.SupplierPriceListTemplateId : undefined,
                    SupplierId: carrierAccountDirectiveAPI.getSelectedIds(),
                    ConfigDetails: buildInputConfigurationObj()
                };
                return priceListTemplateObject;
            }

            function insertPriceListTemplate() {
                $scope.scopeModel.isLoading = true;

                var priceListTemplateObject = buildPriceListTemplateObjFromScope();

                return WhS_SupPL_SupplierPriceListTemplateAPIService.AddSupplierPriceListTemplate(priceListTemplateObject)
                .then(function (response) {
                    VRNotificationService.notifyOnItemAdded('Supplier Price List Template', response, 'Name');
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });

            }

            function updatePriceListTemplate() {
                $scope.scopeModel.isLoading = true;

                var priceListTemplateObject = buildPriceListTemplateObjFromScope();

                return WhS_SupPL_SupplierPriceListTemplateAPIService.UpdateSupplierPriceListTemplate(priceListTemplateObject).then(function (response) {
                    VRNotificationService.notifyOnItemUpdated('Supplier Price List Template', response, 'Name');
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
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