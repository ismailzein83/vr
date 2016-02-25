
app.service('CP_SupplierPricelist_CustomerUserService', ['VRModalService', 'CP_SupplierPricelist_CustomerService', 'CP_SupplierPricelist_CustomerUserAPIService', 'VRNotificationService',
    function (VRModalService, customerService, customerUserAPIService, vRNotificationService) {


        function assignUser( onCustomerUserAdded , customerId) {
            var modalSettings = {
            };
            var parameters = {
                customerId: customerId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCustomerUserAdded = onCustomerUserAdded;
            };
            VRModalService.showModal('/Client/Modules/CP_SupplierPricelist/Views/Customer/CustomerUserEditor.html', parameters, modalSettings);
        }
        
        function registerDrillDownToCustomer() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Users";
            drillDownDefinition.directive = "vr-cp-customeruser-grid";
            drillDownDefinition.parentMenuActions = [{
                name: "Assign User",
                clicked: function (customerItem) {
                    if (drillDownDefinition.setTabSelected != undefined)
                        drillDownDefinition.setTabSelected(customerItem);
                    var query = {
                        CustomerId: customerItem.Entity.CustomerId
                    }
                    var onCustomerUserAdded = function (codeGroupObj) {
                        if (customerItem.customerUserGridAPI != undefined) {
                            customerItem.customerUserGridAPI.onCustomerUserAdded(codeGroupObj);
                        }
                    };
                    assignUser(onCustomerUserAdded, customerItem.Entity.CustomerId);
                }
            }];

            drillDownDefinition.loadDirective = function (directiveAPI, customerItem) {
                customerItem.customerUserGridAPI = directiveAPI;
                var query = {
                    CustomerId: customerItem.Entity.CustomerId
                };
                return customerItem.customerUserGridAPI.loadGrid(query);
            };

            customerService.addDrillDownDefinition(drillDownDefinition);
        }
        function unassignUser(scope, userId, onCustomerUserDeleted) {
            vRNotificationService.showConfirmation()
                .then(function (response) {

                    if (response) {
                        return customerUserAPIService.DeleteCustomerUser(userId)
                            .then(function (deletionResponse) {
                                vRNotificationService.notifyOnItemDeleted("Customer User", deletionResponse);
                                onCustomerUserDeleted();
                            })
                            .catch(function (error) {
                                vRNotificationService.notifyException(error, scope);
                            });
                    }
                });
        }

        return ({
            assignUser: assignUser,
            registerDrillDownToCustomer: registerDrillDownToCustomer,
            unassignUser: unassignUser
        });
    }]);
