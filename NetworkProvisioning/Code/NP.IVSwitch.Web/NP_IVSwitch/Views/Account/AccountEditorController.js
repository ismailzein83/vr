(function (appControllers) {

    "use strict";

    AccountEditorController.$inject = ['$scope', 'NP_IVSwitch_AccountAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService','NP_IVSwitch_StateEnum'];

    function AccountEditorController($scope, NP_IVSwitch_AccountAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService, NP_IVSwitch_StateEnum) {

        var isEditMode;
        var selectorDirectiveAPI;

        var accountId;
        var accountEntity;

        var selectorDirectiveAPI;
        var selectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                accountId = parameters.AccountId;
            }


            isEditMode = (accountId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onSelectorDirectiveReady = function (api) {
                selectorDirectiveAPI = api;
                selectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectionChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                     $scope.scopeModel.TypeId = SelectedItem.value;
                }
            }

            $scope.scopeModel.onSelectionChangedState = function (SelectedItem) {
                if (SelectedItem != undefined) {                   
                    $scope.scopeModel.currentstate = SelectedItem;
                 }
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;
       
            if (isEditMode) {
                getAccount().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }


        }

         

        function getAccount() {
            return NP_IVSwitch_AccountAPIService.GetAccount(accountId).then(function (response) {
                accountEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSelectorDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var accountName = (accountEntity != undefined) ? accountEntity.FirstName : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(accountName, 'Account');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Account');
                }
            }
            function loadStaticData() {

                $scope.scopeModel.states = UtilsService.getArrayEnum(NP_IVSwitch_StateEnum);

                if (accountEntity == undefined) {

                     $scope.scopeModel.currentstate = $scope.scopeModel.states[0];
                      return;
                }
                $scope.scopeModel.TypeId = accountEntity.TypeId;
                $scope.scopeModel.firstname = accountEntity.FirstName;
                $scope.scopeModel.lastname = accountEntity.LastName;
                $scope.scopeModel.companyname = accountEntity.CompanyName;
                $scope.scopeModel.contactdisplay = accountEntity.ContactDisplay;
                $scope.scopeModel.email = accountEntity.Email;
                $scope.scopeModel.website = accountEntity.WebSite;
                $scope.scopeModel.logalias = accountEntity.LogAlias;
                $scope.scopeModel.currentbalance = accountEntity.CurrentBalance;
                $scope.scopeModel.peervendorid = accountEntity.PeerVendorId;
                $scope.scopeModel.address = accountEntity.Address;

                $scope.scopeModel.creditlimit = accountEntity.CreditLimit;
                $scope.scopeModel.creditthreshold = accountEntity.CreditThreshold;
                $scope.scopeModel.channelslimit = accountEntity.ChannelsLimit;
                $scope.scopeModel.billingcycle = accountEntity.BillingCycle;
                $scope.scopeModel.taxgroupid = accountEntity.TaxGroupId;
                $scope.scopeModel.paymentterms = accountEntity.PaymentTerms;

             
                if (accountEntity.CurrentState != undefined)
                    $scope.scopeModel.currentstate = $scope.scopeModel.states[accountEntity.CurrentState - 1];

             }

            function loadSelectorDirective() {
                var selectorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                selectorDirectiveReadyDeferred.promise.then(function () {
                    var selectorDirectivePayload;
                    if (accountEntity != undefined) {
                        selectorDirectivePayload = { selectedIds : accountEntity.TypeId };
                    }
                    VRUIUtilsService.callDirectiveLoad(selectorDirectiveAPI, selectorDirectivePayload, selectorDirectiveLoadDeferred);
                });

                return selectorDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;

            return NP_IVSwitch_AccountAPIService.AddAccount(buildAccountObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Account', response, 'Name')) {

                    if ($scope.onAccountAdded != undefined)
                        $scope.onAccountAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
           
            return NP_IVSwitch_AccountAPIService.UpdateAccount(buildAccountObjFromScope()).then(function (response) {


                if (VRNotificationService.notifyOnItemUpdated('Account', response, 'Name')) {

                    if ($scope.onAccountUpdated != undefined) {
                        $scope.onAccountUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildAccountObjFromScope() {
            return {
                AccountId: accountEntity != undefined ? accountEntity.AccountId : undefined,
                TypeId: $scope.scopeModel.TypeId,
                FirstName: $scope.scopeModel.firstname,
                LastName: $scope.scopeModel.lastname,
                CompanyName: $scope.scopeModel.companyname,
                ContactDisplay: $scope.scopeModel.contactdisplay,
                Email:  $scope.scopeModel.email,
                WebSite: $scope.scopeModel.website,
                LogAlias:$scope.scopeModel.logalias,
                CurrentBalance: $scope.scopeModel.currentbalance,
                PeerVendorId : $scope.scopeModel.peervendorid,
                Address: $scope.scopeModel.address,

                CreditLimit   :  $scope.scopeModel.creditlimit,
                CreditThreshold :  $scope.scopeModel.creditthreshold,
                CurrentState : $scope.scopeModel.currentstate.value,
                ChannelsLimit:  $scope.scopeModel.channelslimit,
                BillingCycle: $scope.scopeModel.billingcycle,
                TaxGroupId : $scope.scopeModel.taxgroupid,
                PaymentTerms: $scope.scopeModel.paymentterms,

             };
        }
    }

    appControllers.controller('NP_IVSwitch_AccountEditorController', AccountEditorController);

})(appControllers);