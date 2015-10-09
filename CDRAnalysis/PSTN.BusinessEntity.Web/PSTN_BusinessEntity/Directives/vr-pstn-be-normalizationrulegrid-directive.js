﻿"use strict"

app.directive("vrPstnBeNormalizationrulegrid", ["PSTN_BE_Service", "NormalizationRuleAPIService", "PSTN_BE_PhoneNumberTypeEnum", "UtilsService", "VRNotificationService", function (PSTN_BE_Service, NormalizationRuleAPIService, PSTN_BE_PhoneNumberTypeEnum, UtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var normalizationRuleGrid = new NormalizationRuleGrid($scope, ctrl);
            normalizationRuleGrid.defineScope();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/NormalizationRuleGridTemplate.html"
    };

    function NormalizationRuleGrid($scope, ctrl) {

        var gridAPI;
        this.defineScope = defineScope;

        function defineScope() {

            $scope.normalizationRules = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.retrieveData = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onNormalizationRuleAdded = function (normalizationRuleObj) {
                        setPhoneNumberTypeDescripton(normalizationRuleObj);
                        gridAPI.itemAdded(normalizationRuleObj);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return NormalizationRuleAPIService.GetFilteredNormalizationRules(dataRetrievalInput)
                    .then(function (responseArray) {

                        angular.forEach(responseArray.Data, function (item) {
                            setPhoneNumberTypeDescripton(item);
                        });

                        onResponseReady(responseArray);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function editNormalizationRule(dataItem) {
            
            var onNormalizationRuleUpdated = function (normalizationRuleObj) {
                setPhoneNumberTypeDescripton(normalizationRuleObj);
                gridAPI.itemUpdated(normalizationRuleObj);
            }

            PSTN_BE_Service.editNormalizationRule(dataItem, onNormalizationRuleUpdated);
        }

        function deleteNormalizationRule(dataItem) {
            var onNormalizationRuleDeleted = function (normalizationRuleObj) {
                gridAPI.itemDeleted(normalizationRuleObj);
            }

            PSTN_BE_Service.deleteNormalizationRule(dataItem, onNormalizationRuleDeleted);
        }

        function defineMenuActions() {

            $scope.gridMenuActions = [
               {
                   name: "Edit",
                   clicked: editNormalizationRule
               },
               {
                   name: "Delete",
                   clicked: deleteNormalizationRule
               }
            ];
        }

        function setPhoneNumberTypeDescripton(dataItem) {
            var phoneNumberType = UtilsService.getEnum(PSTN_BE_PhoneNumberTypeEnum, "value", dataItem.PhoneNumberType);
            dataItem.PhoneNumberTypeDescription = phoneNumberType.description;
        }
    }

    return directiveDefinitionObject;
}]);