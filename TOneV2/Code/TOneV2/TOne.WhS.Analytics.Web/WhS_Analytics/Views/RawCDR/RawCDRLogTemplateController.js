rawCDRLogTemplateController.$inject = ['$scope', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'WhS_Analytics_RawCDRLogWhereInfoEnum'];

function rawCDRLogTemplateController($scope, UtilsService, VRModalService, VRNotificationService, VRNavigationService, WhS_Analytics_RawCDRLogWhereInfoEnum) {
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }

    function defineScope() {

        $scope.InfoData = [];
        $scope.close = function () {
            $scope.modalContext.closeModal();
        };

    }
    function defineInfoData() {
        $scope.InfoData = [];
        for (var td in WhS_Analytics_RawCDRLogWhereInfoEnum)
            $scope.InfoData.push(WhS_Analytics_RawCDRLogWhereInfoEnum[td]);
    }
    function load() {
        defineInfoData();
    }
}
appControllers.controller('WhS_Analytics_RawCDRLogTemplateController', rawCDRLogTemplateController);
