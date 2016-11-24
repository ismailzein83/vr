
(function (appControllers) {

    "use strict";

    AccountService.$inject = ['VRModalService','WhS_BE_CarrierAccountService'];

    function AccountService(NPModalService, WhS_BE_CarrierAccountService) {

        function addAccount(CarrierId, onAccountAdded) {
            var settings = {};

            var parameters = {
                CarrierId: CarrierId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountAdded = onAccountAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Account/AccountEditor.html', parameters, settings);
        };
        function editAccount(AccountId, onAccountUpdated) {
            var settings = {};

            var parameters = {
                AccountId: AccountId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountUpdated = onAccountUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Account/AccountEditor.html', parameters, settings);
        }

        //function registerDrillDownToCarrierAccount() {
        //    var drillDownDefinition = {};

        //    drillDownDefinition.title = "IV Switch Accounts";
        //    drillDownDefinition.directive = "np-ivswitch-account-grid";
        //    drillDownDefinition.hideDrillDownFunction = function (dataItem) {
        //        return false;
        //    };
        //    drillDownDefinition.loadDirective = function (directiveAPI, carrierAccountItem) {
        //        carrierAccountItem.ivSwitchAccountGridAPI = directiveAPI;
 
        //         var payload = {
        //            CarrierAccountId: carrierAccountItem.Entity.CarrierAccountId
        //        };
        //       //     hideCustomerColumn: true
        //      //  };
        //        return carrierAccountItem.ivSwitchAccountGridAPI.load(payload);
        //    };
        //    drillDownDefinition.parentMenuActions = [{
        //            name: 'Add  Account',
        //            clicked: function (carrierAccountItem) {
        //                //if (EndPointTab.setTabSelected != undefined)
        //                //    EndPointTab.setTabSelected(parentAccount);
        //                var onAccountAdded = function (addedAccount) {
        //                     if (carrierAccountItem.ivSwitchAccountGridAPI != undefined)
        //                    carrierAccountItem.ivSwitchAccountGridAPI.onAccountAdded(addedAccount);
        //                };
        //                addAccount(carrierAccountItem.Entity.CarrierAccountId, onAccountAdded);
        //            },
        //    }];

           

        //    WhS_BE_CarrierAccountService.addDrillDownDefinition(drillDownDefinition);

        //}
        return {
            addAccount: addAccount,
            editAccount: editAccount,
          //  registerDrillDownToCarrierAccount: registerDrillDownToCarrierAccount
        };
    }

    appControllers.service('NP_IVSwitch_AccountService', AccountService);

})(appControllers);