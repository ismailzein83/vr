(
    function (appControllers) {
        "use strict";
        priceListConversionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_SupPL_PriceListTemplateService', 'WhS_SupPL_PriceListTemplateAPIService'];
        function priceListConversionController($scope, UtilsService, VRNotificationService, VRUIUtilsService, WhS_SupPL_SupplierPriceListAPIService, WhS_SupPL_PriceListTemplateService, WhS_SupPL_PriceListTemplateAPIService) {

            var inputWorkBookApi;
            var inputPriceListName;
            var inputConfigurationAPI;
            var inputConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var inputPriceListTemplateAPI;
            var inputPriceListTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            defineScope();
            load();

            function defineScope() {

                $scope.scopeModel = {};

                $scope.scopeModel.onReadyWoorkBook = function (api) {
                    inputWorkBookApi = api;
                }

                $scope.scopeModel.onInputConfigurationSelectiveReady = function (api) {
                    inputConfigurationAPI = api;
                    inputConfigurationReadyPromiseDeferred.resolve();
                }

                $scope.scopeModel.startConvert = function () {

                    var onOutputPriceListTemplateChoosen = function (choosenPriceListTemplateObj) {
                        if (choosenPriceListTemplateObj != undefined && choosenPriceListTemplateObj.pricelistTemplateIds != undefined) {
                            $scope.scopeModel.isLoading = true;
                            var promises = [];
                            var requests = [];
                            for (var i = 0; i < choosenPriceListTemplateObj.pricelistTemplateIds.length; i++) {

                                var fileName = UtilsService.getUploadedFileName($scope.scopeModel.inPutFile.fileName);

                                var priceListConversion = {
                                    InputFileId: $scope.scopeModel.inPutFile.fileId,
                                    InputPriceListSettings: buildInputConfigurationObj(),
                                }
                                promises.push(convert(priceListConversion));
                            }
                            return UtilsService.waitMultiplePromises(promises).finally(function () {
                                $scope.scopeModel.isLoading = false;;
                            }).catch(function (error) {
                            });
                        }
                    };
                 
                }

                $scope.scopeModel.saveInputConfiguration = function () {
                    return saveInputConfiguration();
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
                    return WhS_SupPL_PriceListTemplateAPIService.HassaveInputConfigurationPermission();
                };
               
            }

            function load() {
                $scope.scopeModel.isLoading = true;
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([loadInputConfiguration, loadInputPriceListTemplateSelector])
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

            function loadInputPriceListTemplateSelector() {
                var loadInputPriceListTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                inputPriceListTemplateReadyPromiseDeferred.promise.then(function () {
                    var payload;
                    VRUIUtilsService.callDirectiveLoad(inputPriceListTemplateAPI, payload, loadInputPriceListTemplatePromiseDeferred);
                });

                return loadInputPriceListTemplatePromiseDeferred.promise;
            }

            function getPriceListTemplate(priceListTemplateId) {
                return WhS_SupPL_PriceListTemplateAPIService.GetPriceListTemplate(priceListTemplateId);
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

            function saveInputConfiguration() {
                var priceListTemplateId;
                if (inputPriceListTemplateAPI != undefined)
                    priceListTemplateId = inputPriceListTemplateAPI.getSelectedIds();
                var onPriceListTemplatSaved = function (priceListTemplateObj) {
                };
                WhS_SupPL_PriceListTemplateService.saveInputPriceListTemplate(onPriceListTemplatSaved, buildInputConfigurationObj(), priceListTemplateId);
            }

            function convert(priceListConversion) {
                return WhS_SupPL_SupplierPriceListAPIService.ConvertPriceList(priceListConversion).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                }).catch(function (error) {
                });
            }

        }

        appControllers.controller('WhS_SupPL_PriceListConversionController', priceListConversionController);
    }
)(appControllers);