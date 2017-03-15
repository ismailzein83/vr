(
    function (appControllers) {
        "use strict";
        priceListConversionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'XBooster_PriceListConversion_PriceListConversionAPIService','XBooster_PriceListConversion_PriceListTemplateService','XBooster_PriceListConversion_PriceListTemplateAPIService'];
        function priceListConversionController($scope, UtilsService, VRNotificationService, VRUIUtilsService, XBooster_PriceListConversion_PriceListConversionAPIService, XBooster_PriceListConversion_PriceListTemplateService, XBooster_PriceListConversion_PriceListTemplateAPIService) {

            var inputWorkBookApi;
            var outPutWorkBookAPI;
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
                };

                $scope.scopeModel.onInputConfigurationSelectiveReady = function (api) {
                    inputConfigurationAPI = api;
                    inputConfigurationReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.startConvertAndDownload = function () {

                    var onOutputPriceListTemplateChoosen = function (choosenPriceListTemplateObj) {
                        if (choosenPriceListTemplateObj != undefined && choosenPriceListTemplateObj.pricelistTemplateIds != undefined) {
                            $scope.scopeModel.isLoading = true;
                            var promises = [];
                            var requests = [];
                            for (var i = 0; i < choosenPriceListTemplateObj.pricelistTemplateIds.length; i++) {

                                var fileName = UtilsService.getUploadedFileName($scope.scopeModel.inPutFile.fileName);

                                var priceListConversion = {
                                    InputPriceListName: fileName,
                                    InputFileId: $scope.scopeModel.inPutFile.fileId,
                                    InputPriceListSettings: buildInputConfigurationObj(),
                                    OutputPriceListTemplateId: choosenPriceListTemplateObj.pricelistTemplateIds[i]
                                };
                                promises.push(convert(priceListConversion));
                            }
                            return UtilsService.waitMultiplePromises(promises).finally(function () {
                                $scope.scopeModel.isLoading = false;;
                            }).catch(function (error) {
                            });
                        }


                    };
                    XBooster_PriceListConversion_PriceListTemplateService.openOutputPriceListTemplates(onOutputPriceListTemplateChoosen);
                };

                $scope.scopeModel.saveInputConfiguration = function () {
                    return saveInputConfiguration();
                };

                $scope.scopeModel.onInputPriceListTemplateSelectorReady = function (api) {
                    inputPriceListTemplateAPI = api;
                    inputPriceListTemplateReadyPromiseDeferred.resolve();
                };

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

                };

                function hassaveInputConfigurationPermission() {
                    return XBooster_PriceListConversion_PriceListTemplateAPIService.HassaveInputConfigurationPermission();
                };
                function hasConvertAndDownloadPermission() {
                    return XBooster_PriceListConversion_PriceListConversionAPIService.HasConvertAndDownloadPermission();
                };
            }

            function load() {
                $scope.scopeModel.isLoading = true;
                loadAllControls();
            }

            function loadAllControls()
            {
                return UtilsService.waitMultipleAsyncOperations([loadInputConfiguration, loadInputPriceListTemplateSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
            }

            function loadInputConfiguration()
            {
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
                return XBooster_PriceListConversion_PriceListTemplateAPIService.GetPriceListTemplate(priceListTemplateId);
            }

            function buildContext() {
                var context = {
                    getSelectedCell: getSelectedCell,
                    setSelectedCell: selectCellAtSheet,
                    getSelectedSheet: getSelectedSheet
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
                return context;
            }

            function buildInputConfigurationObj()
            {
                if (inputConfigurationAPI !=undefined)
                    return inputConfigurationAPI.getData();
            }

            function saveInputConfiguration()
            {
                var priceListTemplateId;
                if(inputPriceListTemplateAPI != undefined)
                    priceListTemplateId = inputPriceListTemplateAPI.getSelectedIds();
                var onPriceListTemplatSaved = function (priceListTemplateObj) {
                };
                XBooster_PriceListConversion_PriceListTemplateService.saveInputPriceListTemplate(onPriceListTemplatSaved, buildInputConfigurationObj(), priceListTemplateId);
            }

            function convert(priceListConversion)
            {
                return XBooster_PriceListConversion_PriceListConversionAPIService.ConvertAndDownloadPriceList(priceListConversion).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                }).catch(function (error) {
                });
            }

        }

        appControllers.controller('ExcelConversion_PriceListConversionController', priceListConversionController);
    }
)(appControllers);