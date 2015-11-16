
app.service('WhS_BE_MainService', ['WhS_BE_RouteRuleAPIService', 'WhS_BE_SellingProductAPIService', 'WhS_BE_CustomerSellingProductAPIService', 'WhS_BE_RoutingProductAPIService',
    'VRModalService', 'VRNotificationService', 'WhS_Be_PricingTypeEnum', 'WhS_BE_SalePricingRuleAPIService', 'UtilsService', 'WhS_BE_PurchasePricingRuleAPIService',
    function (WhS_BE_RouteRuleAPIService, WhS_BE_SellingProductAPIService, WhS_BE_CustomerSellingProductAPIService, WhS_BE_RoutingProductAPIService, VRModalService,
        VRNotificationService, WhS_Be_PricingTypeEnum, WhS_BE_SalePricingRuleAPIService, UtilsService, WhS_BE_PurchasePricingRuleAPIService) {

    return ({
        addRoutingProduct: addRoutingProduct,
        editRoutingProduct: editRoutingProduct,
        deleteRoutingProduct: deleteRoutingProduct,
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
        addCodeGroup: addCodeGroup,
        editCodeGroup: editCodeGroup,
        editRateType: editRateType,
        addRateType: addRateType,
        editZoneServiceConfig: editZoneServiceConfig,
        addZoneServiceConfig: addZoneServiceConfig,
        editCustomerSellingProduct: editCustomerSellingProduct,
        openOrgChartsModal: openOrgChartsModal,
        assignCarriers: assignCarriers
    });

    function addRoutingProduct(onRoutingProductAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Routing Product";
            modalScope.onRoutingProductAdded = onRoutingProductAdded
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', null, settings);
    };

    function editRoutingProduct(routingProductObj, onRoutingProductUpdated) {
        var modalSettings = {
        };

        var parameters = {
            routingProductId: routingProductObj.RoutingProductId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Routing Product: " + routingProductObj.Name;
            modalScope.onRoutingProductUpdated = onRoutingProductUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', parameters, modalSettings);
    };

    function deleteRoutingProduct(scope, routingProductObj, onRoutingProductDeleted) {
        VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {

                        return WhS_BE_RoutingProductAPIService.DeleteRoutingProduct(routingProductObj.RoutingProductId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Routing Product", deletionResponse);
                                onRoutingProductDeleted(routingProductObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, scope);
                            });
                    }
                });
    };

    function addRouteRule(onRouteRuleAdded, routingProductId, sellingNumberPlanId)
    {
        var settings = {
        };

        var parameters = {
            routingProductId: routingProductId,
            sellingNumberPlanId: sellingNumberPlanId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Route Rule";
            modalScope.onRouteRuleAdded = onRouteRuleAdded;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RouteRule/RouteRuleEditor.html', parameters, settings);
    }

    function editRouteRule(routeRuleObj, onRouteRuleUpdated) {
        var modalSettings = {
        };
        var parameters = {
            routeRuleId: routeRuleObj.Entity.RuleId,
            routingProductId: routeRuleObj.Entity.Criteria != null? routeRuleObj.Entity.Criteria.RoutingProductId : undefined
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Route Rule";
            modalScope.onRouteRuleUpdated = onRouteRuleUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
    }

    function deleteRouteRule(scope, routeRuleObj, onRouteRuleDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_RouteRuleAPIService.DeleteRule(routeRuleObj.Entity.RuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Route Rule", deletionResponse);
                            onRouteRuleDeleted(routeRuleObj);
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
            modalScope.title = "New Selling Product";
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
            modalScope.title = "Edit Selling Product";
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
                            VRNotificationService.notifyOnItemDeleted("Selling Product", deletionResponse);
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
            modalScope.title = "Customer Selling Product";
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
                            VRNotificationService.notifyOnItemDeleted("Customer Selling Product", deletionResponse);
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
            CarrierProfileId: carrierProfileObj.Entity.CarrierProfileId,
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

    function editRateType(obj, onRateTypeUpdated) {
        var settings = {
            useModalTemplate: true

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.Name, "Rate Type");
            modalScope.onRateTypeUpdated = onRateTypeUpdated;
        };
        var parameters = {
            RateTypeId: obj.RateTypeId
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RateType/RateTypeEditor.html', parameters, settings);
    }
    function addRateType(onRateTypeAdded) {
        var settings = {
            useModalTemplate: true

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForAddEditor("Rate Type");
            modalScope.onRateTypeAdded = onRateTypeAdded;
        };
        var parameters = {};

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RateType/RateTypeEditor.html', parameters, settings);
    }


    function editZoneServiceConfig(obj, onZoneServiceConfigUpdated) {
        var settings = {
            useModalTemplate: true

        };
        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.Name, "Zone Service Config");
            modalScope.onZoneServiceConfigUpdated = onZoneServiceConfigUpdated;
        };
        var parameters = {
            ServiceFlag: obj.ServiceFlag
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigEditor.html', parameters, settings);
    }

    function addZoneServiceConfig(onZoneServiceConfigAdded) {
        var settings = {
            useModalTemplate: true

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForAddEditor("Zone Service Config");
            modalScope.onZoneServiceConfigAdded = onZoneServiceConfigAdded;
        };
        var parameters = {};

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigEditor.html', parameters, settings);
    }


    
    function editCodeGroup(obj, onCodeGroupUpdated , disableCountry) {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onCodeGroupUpdated = onCodeGroupUpdated;
        };
        var parameters = {
            CodeGroupId: obj.CodeGroupId,
            disableCountry: disableCountry != undefined
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CodeGroup/CodeGroupEditor.html', parameters, settings);
    }
    function addCodeGroup(onCodeGroupAdded , dataItem) {
        var settings = {

        };

        settings.onScopeReady = function (modalScope) {
             modalScope.onCodeGroupAdded = onCodeGroupAdded;
        };
        var parameters;
        if (dataItem != undefined) {
            parameters = {
                CountryId: dataItem.CountryId
            };
        }

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/CodeGroup/CodeGroupEditor.html', parameters, settings);
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
    function editCustomerSellingProduct(obj, onCustomerSellingProductUpdated) {
        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.SellingProductName, "Customer Selling Product");
            modalScope.onCustomerSellingProductUpdated = onCustomerSellingProductUpdated;
        };
        var parameters = {
            CustomerSellingProductId: obj.CustomerSellingProductId,
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/CustomerSellingProductEditor.html', parameters, settings);
    }
    function openOrgChartsModal(onOrgChartAssigned, assignedOrgChartId) {
        var settings = {};
        var parameters = null;

        if (assignedOrgChartId != 0) {
            parameters = {
                assignedOrgChartId: assignedOrgChartId
            };
        }
        settings.onScopeReady = function (modalScope) {
            modalScope.title = 'Assign Org Chart';
            modalScope.onOrgChartAssigned = onOrgChartAssigned;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/AccountManager/OrgChartAssignmentEditor.html', parameters, settings);
    };

    function assignCarriers(onCarriersAssigned, nodeId) {
        var settings = {};

        var parameters = {
            selectedAccountManagerId: nodeId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = 'Assign Carriers';
            modalScope.onCarriersAssigned = onCarriersAssigned
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/AccountManager/CarrierAssignmentEditor.html', parameters, settings);
    };
    
}]);
