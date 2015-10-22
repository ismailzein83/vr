
app.service('WhS_BE_MainService', ['WhS_BE_RouteRuleAPIService', 'WhS_BE_PricingProductAPIService', 'WhS_BE_CustomerPricingProductAPIService', 'VRModalService', 'VRNotificationService', 'WhS_Be_PricingTypeEnum', 'WhS_BE_SalePricingRuleAPIService', 'UtilsService','WhS_BE_PurchasePricingRuleAPIService', function (WhS_BE_RouteRuleAPIService, WhS_BE_PricingProductAPIService, WhS_BE_CustomerPricingProductAPIService, VRModalService, VRNotificationService, WhS_Be_PricingTypeEnum, WhS_BE_SalePricingRuleAPIService, UtilsService, WhS_BE_PurchasePricingRuleAPIService) {

    return ({
        addRouteRule: addRouteRule,
        editRouteRule: editRouteRule,
        deleteRouteRule: deleteRouteRule,
        addPricingProduct: addPricingProduct,
        editPricingProduct: editPricingProduct,
        deletePricingProduct: deletePricingProduct,
        addCustomerPricingProduct: addCustomerPricingProduct,
        deleteCustomerPricingProduct: deleteCustomerPricingProduct,
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
        var settings = {};

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

    function addPricingProduct(onPricingProductAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Pricing Product";
            modalScope.onPricingProductAdded = onPricingProductAdded;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingProduct/PricingProductEditor.html', null, settings);
    }

    function editPricingProduct(pricingProductObj, onPricingProductUpdated) {
        var modalSettings = {
        };
        var parameters = {
            PricingProductId: pricingProductObj.PricingProductId,
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Pricing Product";
            modalScope.onPricingProductUpdated = onPricingProductUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingProduct/PricingProductEditor.html', parameters, modalSettings);
    }

    function deletePricingProduct($scope, pricingProductObj, onPricingProductDeleted) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_PricingProductAPIService.DeletePricingProduct(pricingProductObj.PricingProductId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Pricing Product", deletionResponse);
                            onPricingProductDeleted(pricingProductObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function addCustomerPricingProduct(onCustomerPricingProductAdded, dataItem) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Customer Pricing Product";
            modalScope.onCustomerPricingProductAdded = onCustomerPricingProductAdded;
        };
        var parameters=null;
        if (dataItem != undefined) {
             parameters = {
                 PricingProductId: dataItem.PricingProductId,
                 CarrierAccountId: dataItem.CarrierAccountId
            };
        }
            
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingProduct/CustomerPricingProductEditor.html', parameters, settings);
    }

    function deleteCustomerPricingProduct($scope,customerPricingProductObj, onCustomerPricingProductDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_CustomerPricingProductAPIService.DeleteCustomerPricingProduct(customerPricingProductObj.CustomerPricingProductId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Customer Pricing Product", deletionResponse);
                            onCustomerPricingProductDeleted(deletionResponse.UpdatedObject);
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
