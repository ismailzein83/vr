"use strict"

app.directive("vrWhsCdrprocessingNormalizationruleGrid", ["WhS_CDRProcessing_MainService", "WhS_CDRProcessing_NormalizationRuleAPIService", "WhS_CDRProcessing_PhoneNumberTypeEnum", "UtilsService", "VRNotificationService", function (WhS_CDRProcessing_MainService, WhS_CDRProcessing_NormalizationRuleAPIService, WhS_CDRProcessing_PhoneNumberTypeEnum, UtilsService, VRNotificationService) {

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
        templateUrl: "/Client/Modules/WhS_CDRProcessing/Directives/NormalizationRule/Templates/NormalizationRuleGrid.html"
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

                    directiveAPI.onNormalizationRuleAdded = function (normalizationRuleDetail) {
                        gridAPI.itemAdded(normalizationRuleDetail);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return WhS_CDRProcessing_NormalizationRuleAPIService.GetFilteredNormalizationRules(dataRetrievalInput)
                    .then(function (responseArray) {
                        for (var i = 0; i < responseArray.Data.length;i++)
                            AddPhoneTypeDescriptionName(responseArray.Data[i]);
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
                gridAPI.itemUpdated(normalizationRuleObj);
            }

            WhS_CDRProcessing_MainService.editNormalizationRule(dataItem, onNormalizationRuleUpdated);
        }

        function deleteNormalizationRule(dataItem) {
            var onNormalizationRuleDeleted = function (normalizationRuleObj) {
                gridAPI.itemDeleted(normalizationRuleObj);
            }

            WhS_CDRProcessing_MainService.deleteNormalizationRule(dataItem, onNormalizationRuleDeleted);
        }
        function AddPhoneTypeDescriptionName(obj) {
            for (var i = 0; i < obj.Entity.Criteria.PhoneNumberTypes.length; i++)
                for (var p in WhS_CDRProcessing_PhoneNumberTypeEnum)
                    if (obj.Entity.Criteria.PhoneNumberTypes[i] == WhS_CDRProcessing_PhoneNumberTypeEnum[p].value)
                        if (obj.PhoneNumberTypeDescription != undefined)
                            obj.PhoneNumberTypeDescription += "," + WhS_CDRProcessing_PhoneNumberTypeEnum[p].description;
                        else
                            obj.PhoneNumberTypeDescription = WhS_CDRProcessing_PhoneNumberTypeEnum[p].description;
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
    }

    return directiveDefinitionObject;
}]);