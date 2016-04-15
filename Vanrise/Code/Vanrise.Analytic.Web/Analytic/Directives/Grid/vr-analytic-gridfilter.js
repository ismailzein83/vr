"use strict";

app.directive("vrAnalyticGridfilter", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService', 'VR_Analytic_AnalyticConfigurationAPIService',
function (UtilsService, VRNotificationService, VRUIUtilsService, VRValidationService, VR_Analytic_AnalyticConfigurationAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var genericFilter = new GenericFilter($scope, ctrl, $attrs);
            genericFilter.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: function () {
            return "/Client/Modules/Analytic/Directives/Grid/Templates/AnalyticReportGridFilter.html";
        }
    };
    function GenericFilter($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            ctrl.dimensions = [];
            ctrl.measures = [];

            ctrl.selecteddimensions = [];
            ctrl.selectedMeasures = [];

            ctrl.fromdate = new Date();

            ctrl.validateDateTime = function () {
                return VRValidationService.validateTimeRange(ctrl.fromdate, ctrl.todate);
            }
                     
       
            defineAPI();

        }

        function defineAPI() {
            var api = {};

            api.getData = function () {

                var selectedobject = {
                    selecteddimensions: UtilsService.getPropValuesFromArray(ctrl.selecteddimensions, "value"),
                    //selectedfilters: selectedfilters,
                    selectedperiod: undefined,
                    fromdate: ctrl.fromdate,
                    todate: ctrl.todate,
                    currency: undefined
                };
                return selectedobject;
            }

            api.load = function (payload) {

                var promises = [];

                function loadDimensions() {

                    return VR_Analytic_AnalyticConfigurationAPIService.GetDimensionsInfo().then(function (response) {
                        ctrl.dimensions.length = 0;
                        angular.forEach(response, function (itm) {
                            ctrl.dimensions.push(itm);
                        });
                    });
                }

                function loadMeasures() {
                    return VR_Analytic_AnalyticConfigurationAPIService.GetMeasuresInfo().then(function (response) {
                        ctrl.measures.length = 0;
                        angular.forEach(response, function (itm) {
                            ctrl.measures.push(itm);
                        });
                    });
                }

                loadDimensions();
                loadMeasures();
                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
    return directiveDefinitionObject;

}]);