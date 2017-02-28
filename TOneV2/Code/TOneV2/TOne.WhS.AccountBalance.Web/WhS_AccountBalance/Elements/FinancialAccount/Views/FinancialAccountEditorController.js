(function (appControllers) {

    'use strict';

    financialAccountEditorController.$inject = ['$scope', 'WhS_AccountBalance_FinancialAccountAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','VR_AccountBalance_AccountTypeAPIService'];

    function financialAccountEditorController($scope, WhS_AccountBalance_FinancialAccountAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_AccountBalance_AccountTypeAPIService) {
        var carrierAccountId;
        var carrierProfileId;

        var financialAccountTypeSelectorDirectiveAPI;
        var financialAccountTypeSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        var directiveAPI;
        var directiveReadyDeferred;

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                carrierAccountId = parameters.carrierAccountId;
                carrierProfileId = parameters.carrierProfileId;
            }

        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.beginEffectiveDate = new Date();
            $scope.scopeModel.onFinancialAccountTypeSelectorReady = function (api) {
                financialAccountTypeSelectorDirectiveAPI = api;
                financialAccountTypeSelectorDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onFinancialAccountTypeSelectionChanged = function (value) {
                if(value != undefined)
                {
                    $scope.scopeModel.isLoading = true;
                    getFinancialAccountTypeSetting().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                }else
                {
                    $scope.scopeModel.financialAccountTypeRuntimeDirective = undefined;
                }
            };
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.save = function () {
                return insertFinancialAccount();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function setTitle() {
                $scope.title = UtilsService.buildTitleForAddEditor('Financial Account');
            }
            function loadStaticData() {

            }
            function loadFinancialAccountTypeSelectorSelector() {
                var loadFinancialAccountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                financialAccountTypeSelectorDirectiveReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "TOne.WhS.AccountBalance.Business.AccountBalanceSettingsFilter, TOne.WhS.AccountBalance.Business",
                                CarrierProfileId: carrierProfileId,
                                CarrierAccountId:carrierAccountId
                            }]
                        }
                    }
                    VRUIUtilsService.callDirectiveLoad(financialAccountTypeSelectorDirectiveAPI, payload, loadFinancialAccountTypeSelectorPromiseDeferred);
                });
                return loadFinancialAccountTypeSelectorPromiseDeferred.promise;
            }

            function loadDirective() {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();

                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                directiveReadyDeferred.promise.then(function () {
                    directiveReadyDeferred = undefined;
                    var directivePayload = {};
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadFinancialAccountTypeSelectorSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    
        function getFinancialAccountTypeSetting()
        {
            var selectedfinancialAccountTypeId = financialAccountTypeSelectorDirectiveAPI.getSelectedIds();
            return VR_AccountBalance_AccountTypeAPIService.GetAccountTypeSettings(selectedfinancialAccountTypeId).then(function (response) {
                if (response != undefined && response.ExtendedSettings != undefined)
                    $scope.scopeModel.financialAccountTypeRuntimeDirective = response.ExtendedSettings.RuntimeEditor;
            });
        }

        function insertFinancialAccount() {
            $scope.scopeModel.isLoading = true;

            var financialAccountObj = buildFinancialAccountObjFromScope();

            return WhS_AccountBalance_FinancialAccountAPIService.AddFinancialAccount(financialAccountObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Financial Account', response, 'Name')) {
                    if ($scope.onFinancialAccountAdded != undefined)
                        $scope.onFinancialAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildFinancialAccountObjFromScope() {
            var obj = {
                CarrierAccountId: carrierAccountId,
                CarrierProfileId: carrierProfileId,
                Settings: {
                    AccountTypeId: financialAccountTypeSelectorDirectiveAPI.getSelectedIds(),
                    ExtendedSettings: directiveAPI.getData()
                },
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate
            };
            return obj;
        }
    }

    appControllers.controller('VR_AccountBalance_FinancialAccountEditorController', financialAccountEditorController);

})(appControllers);