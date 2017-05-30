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

        loadParameters();
        defineScope();
        loadAllControls();

        function defineScope() {
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
                return whSBeSalePriceListChangeApiService.DownloadSalePriceList(priceListId, priceLisTypeSelectorAPI.getSelectedIds()).then(function (response) {
                    utilsService.downloadFile(response.data, response.headers);
                })
                .catch(function (error) {
                    vrNotificationService.notifyException(error, $scope);
                });
            };

            $scope.SendPriceListByEmail = function () {
                $scope.isLoadingFilter = true;

                whSBeSalePriceListChangeApiService.GenerateAndEvaluateSalePriceListEmail(priceListId, priceLisTypeSelectorAPI.getSelectedIds())
                    .then(function (emailResponse) {
                    WhS_BE_SalePriceListChangeService.sendEmail(emailResponse, onSalePriceListSendingEmail);
                }).catch(function (error) {

                    $scope.isLoadingFilter = false;
                    vrNotificationService.notifyException(error, $scope);

                }).finally(function () {
                    $scope.isLoadingFilter = false;
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function onSalePriceListSendingEmail(evaluatedEmail) {
            VRCommon_VRMailAPIService.SendEmail(evaluatedEmail)
            .then(function () {
                whSBeSalePriceListChangeApiService.SetPriceListAsSent(priceListId)
                        .then(function () {
                            $scope.modalContext.closeModal();
                        });
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
        function setTitle() {
            $scope.title = 'Sale Pricelist for ' + ownerName;
        }
        function GetOwner() {
            return whSBeSalePriceListChangeApiService.GetOwnerName(priceListId)
                .then(function (name) {
                    ownerName = name;
                });
        }
        function GetOwnerPriceListType() {
            return whSBeSalePriceListChangeApiService.GetOwnerPriceListType(priceListId)
                .then(function (priceListTypeId) {
                    loadPriceListTypeSelector(priceListTypeId);
                });
        }

        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([loadCountryRateSelector, GetOwner, GetOwnerPriceListType])
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