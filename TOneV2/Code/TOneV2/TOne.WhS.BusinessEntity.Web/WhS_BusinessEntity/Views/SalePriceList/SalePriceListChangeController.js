(function (appControllers) {
    "use strict";

    salePriceListPreviewController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_BE_SalePriceListChangeAPIService', 'VRModalService', 'WhS_BE_SalePriceListChangeService', 'VRCommon_VRMailAPIService'];

    function salePriceListPreviewController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, whSBeSalePriceListChangeApiService, VRModalService, WhS_BE_SalePriceListChangeService, VRCommon_VRMailAPIService) {
        var priceListId;
        var filter = {};

        var ownerName;

        var codeChangeGridApi;
        var rateChangeGridApi;
        var rpChangeGridApi;
        var countryCodeDirectiveApi;
        var countryRateDirectiveApi;

        var countryRPDirectiveApi;
        var countryRateReadyPromiseDeferred = utilsService.createPromiseDeferred();

        var priceLisTypeSelectorAPI;
        var priceListTypeSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var pricelistTemplateSelectorAPI;
        var pricelistTemplateSelectorReadyDeferred = utilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.onCodeChangeGridReady = function (api) {
                codeChangeGridApi = api;
                codeChangeGridApi.loadGrid(filter);
            };
            $scope.onRateChangeGridReady = function (api) {
                rateChangeGridApi = api;
                rateChangeGridApi.loadGrid(filter);
            };
            $scope.onRPChangeGridReady = function (api) {
                rpChangeGridApi = api;
                rpChangeGridApi.loadGrid(filter);
            };
            $scope.onCountryCodeReady = function (api) {
                countryCodeDirectiveApi = api;
                var setLoader = function (value) { $scope.isLoadingCountryCode = value };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, countryCodeDirectiveApi, undefined, setLoader);

            };
            $scope.onCountryRateReady = function (api) {
                countryRateDirectiveApi = api;
                countryRateReadyPromiseDeferred.resolve();
            };
            $scope.onCountryRPReady = function (api) {
                countryRPDirectiveApi = api;
                var setLoader = function (value) { $scope.isLoadingRPCountryCode = value };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, countryRPDirectiveApi, undefined, setLoader);
            };
            $scope.onPriceListTypeSelectorReady = function (api) {
                priceLisTypeSelectorAPI = api;
                var payload = {
                    selectfirstitem: true
                };
                var setLoader = function (value) { $scope.isLoadingPriceListTypeFormatSelector = value };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, priceLisTypeSelectorAPI, payload, setLoader, priceListTypeSelectorReadyDeferred);
            };
            $scope.onPricelistTemplateSelectorReady = function (api) {
                pricelistTemplateSelectorAPI = api;
                var payload = {
                };
                var setLoader = function (value) { $scope.isLoadingPricelistTemplateSelector = value };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, pricelistTemplateSelectorAPI, payload, setLoader, pricelistTemplateSelectorReadyDeferred);
            };
            $scope.searchCodeClicked = function () {
                SetFilteredCodeObject();
                return codeChangeGridApi.loadGrid(filter);
            };
            $scope.searchRateClicked = function () {
                SetFilteredRateObject();
                return rateChangeGridApi.loadGrid(filter);
            };
            $scope.searchRPClicked = function () {
                SetFilteredRPObject();
                return rpChangeGridApi.loadGrid(filter);
            };
            $scope.DownloadPriceList = function () {
                return whSBeSalePriceListChangeApiService.DownloadSalePriceList(BuildSalePriceListInput()).then(function (response) {
                    utilsService.downloadFile(response.data, response.headers);
                })
                .catch(function (error) {
                    vrNotificationService.notifyException(error, $scope);
                });
            };

            $scope.SendPriceListByEmail = function () {
                $scope.isLoadingFilter = true;
                whSBeSalePriceListChangeApiService.GenerateAndEvaluateSalePriceListEmail(BuildSalePriceListInput()).then(function (emailResponse) {
                    WhS_BE_SalePriceListChangeService.sendEmail(emailResponse, onSalePriceListEmailSent);
                }).catch(function (error) {
                    vrNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoadingFilter = false;
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function BuildSalePriceListInput() {
            return {
                PriceListTypeId: priceLisTypeSelectorAPI.getSelectedIds(),
                PricelistTemplateId: pricelistTemplateSelectorAPI.getSelectedIds(),
                PriceListId: priceListId
            };
        }
        function onSalePriceListEmailSent(evaluatedEmail) {
            $scope.isLoadingFilter = true;
            var promises = [];
            evaluatedEmail.CompressFile = $scope.scopeModel.compressPriceListFile;
            var sendEmailPromise = VRCommon_VRMailAPIService.SendEmail(evaluatedEmail);
            promises.push(sendEmailPromise);

            var setPriceListAsSentDeferred = utilsService.createPromiseDeferred();
            promises.push(setPriceListAsSentDeferred.promise);

            sendEmailPromise.then(function () {
                whSBeSalePriceListChangeApiService.SetPriceListAsSent(priceListId).then(function () {
                    setPriceListAsSentDeferred.resolve();
                }).catch(function (error) {
                    setPriceListAsSentDeferred.reject(error);
                });
            });

            return utilsService.waitMultiplePromises(promises).then(function () {
                if ($scope.onSalePriceListPreviewClosed != undefined)
                    $scope.onSalePriceListPreviewClosed();
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }
        function SetFilteredCodeObject() {
            if (priceListId != undefined) {
                filter =
                {
                    PriceListId: priceListId,
                    Countries: countryCodeDirectiveApi.getSelectedIds()
                }
            }
        }
        function SetFilteredRateObject() {
            if (priceListId != undefined) {
                filter =
                {
                    PriceListId: priceListId,
                    Countries: countryRateDirectiveApi.getSelectedIds()
                }
            }
        }
        function SetFilteredRPObject() {
            if (priceListId != undefined) {
                filter =
                {
                    PriceListId: priceListId,
                    Countries: countryRPDirectiveApi.getSelectedIds()
                }
            }
        }
        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined) {
                priceListId = parameters.PriceListId;
                filter.PriceListId = priceListId;
            }
        }
        function loadCountryRateSelector() {
            var countryLoadPromiseDeferred = utilsService.createPromiseDeferred();
            countryRateReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    vruiUtilsService.callDirectiveLoad(countryRateDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }
        function loadPriceListTypeSelector(ownerPriceListTypeId) {
            var priceListTypeSelectorLoadDeferred = utilsService.createPromiseDeferred();

            priceListTypeSelectorReadyDeferred.promise.then(function () {
                priceListTypeSelectorReadyDeferred = undefined;
                var priceListTypeSelectorPayload = {
                    selectedIds: ownerPriceListTypeId
                };
                vruiUtilsService.callDirectiveLoad(priceLisTypeSelectorAPI, priceListTypeSelectorPayload, priceListTypeSelectorLoadDeferred);
            });
            return priceListTypeSelectorLoadDeferred.promise;
        }
        function loadPricelistTemplateSelector(ownerPricelistTemplateId) {
            var pricelistTemplateSelectorLoadDeferred = utilsService.createPromiseDeferred();

            pricelistTemplateSelectorReadyDeferred.promise.then(function () {
                pricelistTemplateSelectorReadyDeferred = undefined;
                var pricelistTemplateSelectorPayload = {
                    selectedIds: ownerPricelistTemplateId
                };
                vruiUtilsService.callDirectiveLoad(pricelistTemplateSelectorAPI, pricelistTemplateSelectorPayload, pricelistTemplateSelectorLoadDeferred);
            });
            return pricelistTemplateSelectorLoadDeferred.promise;
        }
        function setTitle() {
            $scope.title = 'Sale Pricelist for ' + ownerName;
        }
        function GetOwnerOptions() {
            return whSBeSalePriceListChangeApiService.GetOwnerOptions(priceListId)
                 .then(function (zipOption) {
                     $scope.scopeModel.compressPriceListFile = zipOption.CompressPriceListFile;
                     ownerName = zipOption.OwnerName;
                 });
        }
        function GetOwnerPriceListType() {
            return whSBeSalePriceListChangeApiService.GetOwnerPriceListType(priceListId)
                .then(function (priceListTypeId) {
                    loadPriceListTypeSelector(priceListTypeId);
                });
        }
        function GetOwnerPricelistTemplateId() {
            return whSBeSalePriceListChangeApiService.GetOwnerPricelistTemplateId(priceListId)
                .then(function (pricelistTemplateId) {
                    loadPricelistTemplateSelector(pricelistTemplateId);
                });
        }

        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([loadCountryRateSelector, GetOwnerOptions, GetOwnerPriceListType, GetOwnerPricelistTemplateId])
                .then(function () {
                    setTitle();
                })
              .catch(function (error) {
                  vrNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }
    }
    appControllers.controller('WhS_BE_SalePriceListPreviewController', salePriceListPreviewController);
})(appControllers);