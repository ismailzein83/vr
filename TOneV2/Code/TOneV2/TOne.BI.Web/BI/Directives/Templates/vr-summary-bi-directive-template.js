VrSummaryDirectiveTemplateController.$inject = ['$scope', 'BIConfigurationAPIService', 'VRNotificationService'];

function VrSummaryDirectiveTemplateController($scope, BIConfigurationAPIService, VRNotificationService) {
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }
    function defineScope() {
        $scope.Measures = [];
        $scope.selectedMeasureTypes = [];
        $scope.subViewConnector.getValue = function () {
            return getSubViewValue();
        }
    }
    function getSubViewValue() {
        if ($scope.selectedMeasureTypes.length == 0)
            return false;
        var measureTypes = [];

        for (var i = 0; i < $scope.selectedMeasureTypes.length; i++) {

            measureTypes.push($scope.selectedMeasureTypes[i].Name);
        }
        return {
            $type: "TOne.BI.Entities.SummaryDirectiveSetting, TOne.BI.Entities",
            MeasureTypes: measureTypes,
        };
    }

    function setSubViewValue(settings) {
        for (var i = 0; i < settings.MeasureTypes.length; i++) {

            for (j = 0; j < $scope.Measures.length; j++) {

                if (settings.MeasureTypes[i] == $scope.Measures[j].Name)
                    $scope.selectedMeasureTypes.push($scope.Measures[j]);
            }
        }
    }
    function load() {
        $scope.isGettingData = true;
        loadMeasures();

    }

    function loadMeasures() {
        $scope.isGettingData = true;
        return BIConfigurationAPIService.GetMeasures().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Measures.push(itm);
            });
            if ($scope.subViewConnector.value != null && $scope.subViewConnector.value != undefined) {
                setSubViewValue($scope.subViewConnector.value);
            }
            $scope.isGettingData = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
    }


}
appControllers.controller('BI_VrSummaryDirectiveTemplateController', VrSummaryDirectiveTemplateController);
