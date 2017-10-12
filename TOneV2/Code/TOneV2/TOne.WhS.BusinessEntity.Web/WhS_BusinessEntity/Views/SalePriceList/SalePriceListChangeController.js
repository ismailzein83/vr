(function (appControllers) {
    "use strict";

    salePriceListPreviewController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_BE_SalePriceListChangeAPIService', 'VRModalService', 'WhS_BE_SalePriceListChangeService', 'VRCommon_VRMailAPIService', 'WhS_BE_SalePricelistAPIService'];

    function salePriceListPreviewController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, whSBeSalePriceListChangeApiService, VRModalService, WhS_BE_SalePriceListChangeService, VRCommon_VRMailAPIService, WhS_BE_SalePricelistAPIService) {
        var priceListId;
        var filter = {
        };

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
        var priceListTemplateSelectedDeferred;

        var shouldOpenEmailPage;

        loadParameters();
        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.scopeModel = {
            };
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
                var setLoader = function (value) {
                    $scope.isLoadingCountryCode = value;
                };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, countryCodeDirectiveApi, undefined, setLoader);

            };
            $scope.onCountryRateReady = function (api) {
                countryRateDirectiveApi = api;
                countryRateReadyPromiseDeferred.resolve();
            };
            $scope.onCountryRPReady = function (api) {
                countryRPDirectiveApi = api;
                var setLoader = function (value) {
                    $scope.isLoadingRPCountryCode = value;
                };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, countryRPDirectiveApi, undefined, setLoader);
            };
            $scope.onPriceListTypeSelectorReady = function (api) {
                priceLisTypeSelectorAPI = api;
                var payload = {
                    selectfirstitem: true
                };
                var setLoader = function (value) {
                    $scope.isLoadingPriceListTypeFormatSelector = value;
                };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, priceLisTypeSelectorAPI, payload, setLoader, priceListTypeSelectorReadyDeferred);
            };
            $scope.onPricelistTemplateSelectorReady = function (api) {
                pricelistTemplateSelectorAPI = api;
                pricelistTemplateSelectorReadyDeferred.resolve();
            };
            $scope.onPriceListTemplateChanged = function () {
                if (priceListTemplateSelectedDeferred != undefined)
                    priceListTemplateSelectedDeferred.resolve();
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
                return whSBeSalePriceListChangeApiService.GetPricelistSalePricelistVRFile(BuildSalePriceListInput()).then(function (response) {
                    if (response.length === 1) {
                        whSBeSalePriceListChangeApiService.DownloadSalePriceList(response[0].FileId).then(function (bufferArrayRespone) {
                            utilsService.downloadFile(bufferArrayRespone.data, bufferArrayRespone.headers);
                        });
                    } else {
                        //show popup
                        PreviewPriceListFileByCurrency(response);
                    }
                })
                .catch(function (error) {
                    vrNotificationService.notifyException(error, $scope);
                });
            };

            $scope.SendPriceListByEmail = function () {
                $scope.isLoadingFilter = true;
                var sendPromiseDeferred = utilsService.createPromiseDeferred();
                WhS_BE_SalePricelistAPIService.CheckIfCustomerHasNotSendPricelist(priceListId).then(function (response) {
                    if (response == true) {
                        vrNotificationService.showConfirmation("This Customer has previous Pricelists not sent. Are you sure you want to continue ?").then(function (response) {
                            if (response) {
                                sendMail().then(function () { sendPromiseDeferred.resolve(); });
                            }
                            else {
                                sendPromiseDeferred.resolve();
                            }
                        });
                    }
                    else {
                        sendMail().then(function () { sendPromiseDeferred.resolve(); });
                    }
                }).catch(function (error) {
                    vrNotificationService.notifyException(error, $scope);
                });
                return sendPromiseDeferred.promise.then(function () { $scope.isLoadingFilter = false; });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function sendMail() {
            return whSBeSalePriceListChangeApiService.GenerateAndEvaluateSalePriceListEmail(BuildSalePriceListInput()).then(function (emailResponse) {
                WhS_BE_SalePriceListChangeService.sendEmail(emailResponse, onSalePriceListEmailSent);
        });
        }

        function PreviewPriceListFileByCurrency(vrFiles) {
            WhS_BE_SalePriceListChangeService.salePricelistFilePreview(vrFiles);
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
                };
            }
        }
        function SetFilteredRateObject() {
            if (priceListId != undefined) {
                filter =
                {
                    PriceListId: priceListId,
                    Countries: countryRateDirectiveApi.getSelectedIds()
                };
            }
        }
        function SetFilteredRPObject() {
            if (priceListId != undefined) {
                filter =
                {
                    PriceListId: priceListId,
                    Countries: countryRPDirectiveApi.getSelectedIds()
                };
            }
        }
        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined) {
                priceListId = parameters.PriceListId;
                filter.PriceListId = priceListId;
                shouldOpenEmailPage = parameters.shouldOpenEmailPage;
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

        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([loadCountryRateSelector, GetOwnerOptions, GetOwnerPriceListType, loadPriceListTemplateSection])
                .then(function () {
                    setTitle();
                })
              .catch(function (error) {
                  vrNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
                 if (shouldOpenEmailPage === true)
                     $scope.SendPriceListByEmail();
             });
        }
        function loadPriceListTemplateSection() {
            var promises = [];

            var priceListTemplateId;

            var getOwnerPriceListTemplateIdPromise = getOwnerPricelistTemplateId();
            promises.push(getOwnerPriceListTemplateIdPromise);

            var loadPriceListTemplateSelectorDeferred = utilsService.createPromiseDeferred();
            promises.push(loadPriceListTemplateSelectorDeferred.promise);

            priceListTemplateSelectedDeferred = utilsService.createPromiseDeferred();
            promises.push(priceListTemplateSelectedDeferred.promise);

            getOwnerPriceListTemplateIdPromise.then(function () {
                loadPriceListTemplateSelector().then(function () {
                    loadPriceListTemplateSelectorDeferred.resolve();
                }).catch(function (error) {
                    loadPriceListTemplateSelectorDeferred.reject(error);
                });
            });

            priceListTemplateSelectedDeferred.promise.then(function () {
                priceListTemplateSelectedDeferred = undefined;
            });

            function getOwnerPricelistTemplateId() {
                return whSBeSalePriceListChangeApiService.GetOwnerPricelistTemplateId(priceListId).then(function (response) {
                    priceListTemplateId = response;
                });
            }
            function loadPriceListTemplateSelector() {
                var pricelistTemplateSelectorLoadDeferred = utilsService.createPromiseDeferred();
                pricelistTemplateSelectorReadyDeferred.promise.then(function () {
                    var pricelistTemplateSelectorPayload = {
                        selectedIds: priceListTemplateId
                    };
                    vruiUtilsService.callDirectiveLoad(pricelistTemplateSelectorAPI, pricelistTemplateSelectorPayload, pricelistTemplateSelectorLoadDeferred);
                });
                return pricelistTemplateSelectorLoadDeferred.promise;
            }

            return utilsService.waitMultiplePromises(promises);
        }
    }
    appControllers.controller('WhS_BE_SalePriceListPreviewController', salePriceListPreviewController);
})(appControllers);