﻿"use strict";

app.directive("vrWhsCdrprocessingDefinecdrfieldsGrid", ["UtilsService", "VRNotificationService", "WhS_CDRProcessing_MainService", "WhS_CDRProcessing_DefineCDRFieldsAPIService",
function (UtilsService, VRNotificationService, WhS_CDRProcessing_MainService, WhS_CDRProcessing_DefineCDRFieldsAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var defineCDRFieldsGrid = new DefineCDRFieldsGrid($scope, ctrl, $attrs);
            defineCDRFieldsGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CDRProcessing/Directives/CDRFields/Templates/DefineCDRFieldsGrid.html"

    };

    function DefineCDRFieldsGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.cdrields = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onCDRFieldAdded = function (cdrFieldObj) {
                        gridAPI.itemAdded(cdrFieldObj);
                    }
                    directiveAPI.onCDRFieldUpdated = function (cdrFieldObj) {
                        gridAPI.itemUpdated(cdrFieldObj);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CDRProcessing_DefineCDRFieldsAPIService.GetFilteredCDRFields(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }
        function defineMenuActions() {
            var defaultMenuActions = [
                        {
                            name: "Edit",
                           // clicked: editCustomerIdentificationRule,
                        },
                         {
                             name: "Delete",
                            // clicked: deleteCustomerIdentificationRule,
                         }
            ];

            $scope.gridMenuActions = function (dataItem) {
                return defaultMenuActions;
            }
        }

        //function editCustomerIdentificationRule(customerRuleObj) {
        //    var onCustomerIdentificationRuleUpdated = function (customerRule) {
        //        gridAPI.itemUpdated(customerRule);
        //    }

        //    WhS_CDRProcessing_MainService.editCustomerIdentificationRule(customerRuleObj.Entity, onCustomerIdentificationRuleUpdated);
        //}
        //function deleteCustomerIdentificationRule(customerRuleObj) {
        //    var onCustomerIdentificationRuleObjDeleted = function (customerRule) {
        //        gridAPI.itemDeleted(customerRule);
        //    };

        //    WhS_CDRProcessing_MainService.deleteCustomerIdentificationRule($scope,customerRuleObj, onCustomerIdentificationRuleObjDeleted);
        //}
    }

    return directiveDefinitionObject;

}]);
