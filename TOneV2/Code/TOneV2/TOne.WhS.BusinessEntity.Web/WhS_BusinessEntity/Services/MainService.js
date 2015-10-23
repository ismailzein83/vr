
app.service('WhS_BE_MainService', ['WhS_BE_RouteRuleAPIService', 'WhS_BE_SellingProductAPIService', 'WhS_BE_CustomerSellingProductAPIService', 'VRModalService', 'VRNotificationService', 'WhS_Be_PricingTypeEnum', 'WhS_BE_SalePricingRuleAPIService', 'UtilsService','WhS_BE_PurchasePricingRuleAPIService', function (WhS_BE_RouteRuleAPIService, WhS_BE_SellingProductAPIService, WhS_BE_CustomerSellingProductAPIService, VRModalService, VRNotificationService, WhS_Be_PricingTypeEnum, WhS_BE_SalePricingRuleAPIService, UtilsService, WhS_BE_PurchasePricingRuleAPIService) {

    return ({
        addRouteRule: addRouteRule,
        editRouteRule: editRouteRule,
        deleteRouteRule: deleteRouteRule,
        addSellingProduct: addSellingProduct,
        editSellingProduct: editSellingProduct,
        deleteSellingProduct: deleteSellingProduct,
        addCustomerSellingProduct: addCustomerSellingProduct,
        deleteCustomerSellingProduct: deleteCustomerSellingProduct,
        addCarrierAccount: addCarrierAccount,
        editCarrierAccount: editCarrierAccount,
        addCarrierProfile:addCarrierProfile,
        editCarrierProfile: editCarrierProfile,
        addSalePricingRule: addSalePricingRule,
        editSalePricingRule: editSalePricingRule,
        deleteSalePricingRule: deleteSalePricingRule,
        addPurchasePricingRule: addPurchasePricingRule,
        editPurchasePricingRule: editPurchasePricingRule,
        deletePurchasePricingRule: deletePurchasePricingRule,
        editCountry: editCountry

    });

    function addRouteRule(onRouteRuleAdded)
    {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Route Rule";
            modalScope.onRouteRuleAdded = onRouteRuleAdded;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RouteRule/RouteRuleEditor.html', null, settings);
    }

    function editRouteRule(routeRuleObj, onRouteRuleUpdated) {
        var modalSettings = {
        };
        var parameters = {
            routeRuleId: routeRuleObj.Entity.RuleId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Route Rule";
            modalScope.onRouteRuleUpdated = onRouteRuleUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
    }

    function deleteRouteRule(scope, ruleId, onRouteRuleDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_RouteRuleAPIService.DeleteRule(ruleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Route Rule", deletionResponse);
                            onRouteRuleDeleted();
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, scope);
                        });
                }
            });
    }

    function addSellingProduct(onSellingProductAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Pricing Product";
            modalScope.onSellingProductAdded = onSellingProductAdded;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', null, settings);
    }

    function editSellingProduct(sellingProductObj, onSellingProductUpdated) {
        var modalSettings = {
        };
        var parameters = {
            SellingProductId: sellingProductObj.SellingProductId,
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Pricing Product";
            modalScope.onSellingProductUpdated = onSellingProductUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', parameters, modalSettings);
    }

    function deleteSellingProduct($scope, sellingProductObj, onSellingProductDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_SellingProductAPIService.DeleteSellingProduct(sellingProductObj.SellingProductId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Pricing Product", deletionResponse);
                            onSellingProductDeleted(sellingProductObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function addCustomerSellingProduct(onCustomerSellingProductAdded, dataItem) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Customer Pricing Product";
            modalScope.onCustomerSellingProductAdded = onCustomerSellingProductAdded;
        };
        var parameters=null;
        if (dataItem != undefined) {
             parameters = {
                 SellingProductId: dataItem.SellingProductId,
                 CarrierAccountId: dataItem.CarrierAccountId
            };
        }
            
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/CustomerSellingProductEditor.html', parameters, settings);
    }

    function deleteCustomerSellingProduct($scope,customerSellingProductObj, onCustomerSellingProductDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_CustomerSellingProductAPIService.DeleteCustomerSellingProduct(customerSellingProductObj.CustomerSellingProductId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Customer Pricing Product", deletionResponse);
                            onCustomerSellingProductDeleted(deletionResponse.UpdatedObject);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
    function addCarrierAccount(onCarrierAccountAdded, dataItem) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Carrier Account";
            modalScope.onCarrierAccountAdded = onCarrierAccountAdded;
        };
        var parameters;
        if (dataItem != undefined)
        {
            parameters = {
                CarrierProfileId: dataItem.CarrierProfileId,
            };
        }
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountEditor.html', parameters, settings);
    }
    function editCarrierAccount(carrierAccountObj, onCarrierAccountUpdated) {
        var modalSettings = {
        };
        var parameters = {
            CarrierAccountId: carrierAccountObj.CarrierAccountId,
            CarrierProfileId: carrierAccountObj.CarrierProfileId,
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Carrier Account";
            modalScope.onCarrierAccountUpdated = onCarrierAccountUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierAccountEditor.html', parameters, modalSettings);
    }

    function addCarrierProfile(onCarrierProfileAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Carrier Profile";
            modalScope.onCarrierProfileAdded = onCarrierProfileAdded;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileEditor.html', null, settings);
    }
    
    function editCarrierProfile(carrierProfileObj, onCarrierProfileUpdated) {
        var modalSettings = {
        };
        var parameters = {
            CarrierProfileId: carrierProfileObj.CarrierProfileId,
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Carrier Profile";
            modalScope.onCarrierProfileUpdated = onCarrierProfileUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CarrierAccount/CarrierProfileEditor.html', parameters, modalSettings);
    }

    function addSalePricingRule(onSalePricingRuleAdded, type) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onPricingRuleAdded = onSalePricingRuleAdded;
        };
        var  parameters={
            PricingType: WhS_Be_PricingTypeEnum.Sale.value  
        };
        if (type != undefined) {
            parameters.PricingRuleType = type;
        }
        
        
        
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
    }
    function editSalePricingRule(obj, onSalePricingRuleUpdated) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onPricingRuleUpdated = onSalePricingRuleUpdated;
        };
        var parameters = {
            RuleId: obj.RuleId,
            PricingType: obj.PricingType
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
    }
    
    function deleteSalePricingRule($scope, salePricingRuleObj, onSalePricingRuleDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_SalePricingRuleAPIService.DeleteRule(salePricingRuleObj.Entity.RuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Sale Pricing Rule", deletionResponse);
                            onSalePricingRuleDeleted(salePricingRuleObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
    function addPurchasePricingRule(onPurchasePricingRuleAdded, type) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onPricingRuleAdded = onPurchasePricingRuleAdded;
        };
        var parameters = {
            PricingType: WhS_Be_PricingTypeEnum.Purchase.value
        };
        if (type != undefined) {
            parameters.PricingRuleType = type;
        }



        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
    }
    function editPurchasePricingRule(obj, onPurchasePricingRuleUpdated) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onPricingRuleUpdated = onPurchasePricingRuleUpdated;
        };
        var parameters = {
            RuleId: obj.RuleId,
            PricingType: obj.PricingType
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingRule/PricingRuleEditor.html', parameters, settings);
    }

    function editCountry(obj, onCountryUpdated) {
        var settings = {
            useModalTemplate: true

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.Name, "Country");
            modalScope.onCountryUpdated = onCountryUpdated;
        };
        var parameters = {
            CountryId: obj.CountryId
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/Country/CountryEditor.html', parameters, settings);
    }

    function deletePurchasePricingRule($scope, purchasePricingRuleObj, onPurchasePricingRuleDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_PurchasePricingRuleAPIService.DeleteRule(purchasePricingRuleObj.Entity.RuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Purchase Pricing Rule", deletionResponse);
                            onPurchasePricingRuleDeleted(purchasePricingRuleObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}]);
