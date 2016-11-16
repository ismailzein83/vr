TimeTriggerTemplateController.$inject = ['$scope', 'TimeSchedulerTypeEnum', 'UtilsService'];

function TimeTriggerTemplateController($scope, TimeSchedulerTypeEnum, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.schedulerTypes = [];
        $scope.selectedType = undefined;

        $scope.schedulerTypeTaskTrigger = {};

        $scope.schedulerTaskTrigger.getData = function () {
            return $scope.schedulerTypeTaskTrigger.getData();
        };

        $scope.schedulerTaskTrigger.loadTemplateData = function () {
            loadForm();
        };
    }

    var isFormLoaded;
    function loadForm() {
        if ($scope.schedulerTaskTrigger.data == undefined || isFormLoaded)
            return;

        var data = $scope.schedulerTaskTrigger.data;
        if (data != null) {
            
            $scope.selectedType = UtilsService.getItemByVal($scope.schedulerTypes, data.TimerTriggerTypeFQTN, "FQTN");
            $scope.schedulerTypeTaskTrigger.data = data;
            if ($scope.schedulerTypeTaskTrigger.loadTemplateData != undefined)
                $scope.schedulerTypeTaskTrigger.loadTemplateData();
        }

        isFormLoaded = true;
    }

    function setFormToDefault() {
        $scope.selectedType = UtilsService.getItemByVal($scope.schedulerTypes, TimeSchedulerTypeEnum.Interval.FQTN, "FQTN");
    }

    function load() {
        $scope.schedulerTypes = UtilsService.getArrayEnum(TimeSchedulerTypeEnum);
        setFormToDefault();
        loadForm();
    }
}
appControllers.controller('Runtime_TimeTriggerTemplateController', TimeTriggerTemplateController);
