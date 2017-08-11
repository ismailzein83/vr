'use strict';

app.directive('retailBeDatarecordtypefieldsFormulaZoneevaluator', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new destinationZoneEvaluatorFieldFormulaCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/DataRecordFieldFormulas/Templates/ZoneEvaluatorFieldFormulaTemplate.html"
        };

        function destinationZoneEvaluatorFieldFormulaCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.getFields != undefined) {
                            $scope.scopeModel.fields = context.getFields();
                        }

                        if (payload.formula != undefined) {
                            $scope.scopeModel.selectedSubscriberZoneFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.SubscriberZoneFieldName, "fieldName");
                            $scope.scopeModel.selectedOtherPartyZoneFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.OtherPartyZoneFieldName, "fieldName");
                            $scope.scopeModel.selectedTrafficDirectionFieldName = UtilsService.getItemByVal($scope.scopeModel.fields, payload.formula.TrafficDirectionFieldName, "fieldName");
                            $scope.scopeModel.trafficDirectionInputValue = payload.formula.TrafficDirectionInputValue;
                            $scope.scopeModel.trafficDirectionOutputValue = payload.formula.TrafficDirectionOutputValue;
                            $scope.scopeModel.isDestinationZoneEvaluator = payload.formula.IsDestinationZoneEvaluator
                        }
                    }
                };

                api.getData = function () {

                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.DataRecordFieldFormulas.ZoneEvaluatorFieldFormula, Retail.BusinessEntity.MainExtensions",
                        SubscriberZoneFieldName: $scope.scopeModel.selectedSubscriberZoneFieldName.fieldName,
                        OtherPartyZoneFieldName: $scope.scopeModel.selectedOtherPartyZoneFieldName.fieldName,
                        TrafficDirectionFieldName: $scope.scopeModel.selectedTrafficDirectionFieldName.fieldName,
                        TrafficDirectionInputValue: $scope.scopeModel.trafficDirectionInputValue,
                        TrafficDirectionOutputValue: $scope.scopeModel.trafficDirectionOutputValue,
                        IsDestinationZoneEvaluator: $scope.scopeModel.isDestinationZoneEvaluator
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);