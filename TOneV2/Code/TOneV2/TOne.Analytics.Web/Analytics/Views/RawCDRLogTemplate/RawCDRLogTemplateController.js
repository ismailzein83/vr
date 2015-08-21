RawCDRLogTemplateController.$inject = ['$scope', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService','RawCDRLogWhereInfoEnum'];

function RawCDRLogTemplateController($scope, UtilsService, VRModalService, VRNotificationService, VRNavigationService, RawCDRLogWhereInfoEnum) {
    //var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }
    function defineScope() {
        $scope.InfoData = [];
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }
    function defineInfoData() {
        $scope.InfoData = [];
        for (var td in RawCDRLogWhereInfoEnum)
            $scope.InfoData.push(RawCDRLogWhereInfoEnum[td]);
    }
    function load() {
        defineInfoData();
    }
}
appControllers.controller('Analytics_RawCDRLogTemplateController', RawCDRLogTemplateController);
