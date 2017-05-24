(function (appControllers) {
    "use strict";

    salePriceListPreviewController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_BE_SalePriceListChangeAPIService', 'VRModalService'];

    function salePriceListPreviewController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, whSBeSalePriceListChangeApiService, VRModalService) {
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
                var setLoader = function (value) { $scope.isLoadingPriceListTypeFormatSelector = value };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, priceLisTypeSelectorAPI, undefined, setLoader, priceListTypeSelectorReadyDeferred);
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
                });
            };
            $scope.SendPriceListByEmail = function () {

                $scope.isLoadingFilter = true;
                whSBeSalePriceListChangeApiService.GenerateSalePriceListFile(priceListId, priceLisTypeSelectorAPI.getSelectedIds()).then(function(response) {
                    var fileId = response;
                    whSBeSalePriceListChangeApiService.EvaluateSalePriceListEmail(priceListId).then(function(emailResponse) {
                        var parametrs =
                        {
                            evaluatedEmail: emailResponse,
                            fileId: fileId
                        };
                        VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageEvaluator.html', parametrs, null);
                    }).catch(function (error) {

                        $scope.isLoadingFilter = false;
                        vrNotificationService.notifyExceptionWithClose(error, $scope);

                    }).finally(function() {
                        $scope.isLoadingFilter = false;
                    });
                }).catch(function (error) {

                    $scope.isLoadingFilter = false;
                    vrNotificationService.notifyExceptionWithClose(error, $scope);

                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
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
        function setTitle() {
            $scope.title = 'Sale Pricelist for ' + ownerName;
        }
        function GetOwner() {
            return whSBeSalePriceListChangeApiService.GetOwnerName(priceListId)
                .then(function (name) {
                    ownerName = name;
                });
        }

        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([loadCountryRateSelector, GetOwner])
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