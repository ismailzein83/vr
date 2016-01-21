'use strict';
app.directive('vrBiSummaryTemplate', ['UtilsService', '$compile', 'VRNotificationService', 'VRUIUtilsService', 'VR_BI_BIConfigurationAPIService',
function (UtilsService, $compile, VRNotificationService, VRUIUtilsService, VR_BI_BIConfigurationAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new biChart(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BI/Directives/Summary/Templates/BISummaryTemplate.html"

    };

    function biChart(ctrl, $scope, $attrs) {

        function initializeController() {
            ctrl.Measures = [];
            ctrl.selectedMeasureTypes = [];
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {

                if (ctrl.selectedMeasureTypes.length == 0)
                    return false;
                var measureTypes = [];

                for (var i = 0; i < ctrl.selectedMeasureTypes.length; i++) {

                    measureTypes.push(ctrl.selectedMeasureTypes[i].Name);
                }
                return {
                    $type: "Vanrise.BI.Entities.SummaryDirectiveSetting, Vanrise.BI.Entities",
                    MeasureTypes: measureTypes,
                };
            }

            api.load = function (payload) {

                return VR_BI_BIConfigurationAPIService.GetMeasuresInfo()
                    .then(function (response) {
                        ctrl.Measures.length = 0;
                        angular.forEach(response, function (itm) {
                            ctrl.Measures.push(itm);
                        });

                        if (payload != undefined) {
                            for (var i = 0; i < payload.MeasureTypes.length; i++) {

                                for (var j = 0; j < ctrl.Measures.length; j++) {

                                    if (payload.MeasureTypes[i] == ctrl.Measures[j].Name)
                                        ctrl.selectedMeasureTypes.push(ctrl.Measures[j]);
                                }
                            }
                        }
                    });
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);