ValidateEditorController.$inject = ['$scope', 'MenuAPIService', 'WidgetAPIService', 'UsersAPIService', 'ViewAPIService', 'UtilsService','GroupAPIService', 'VRNotificationService', 'VRNavigationService', 'WidgetSectionEnum', 'PeriodEnum', 'TimeDimensionTypeEnum', 'ColumnWidthEnum','BIAPIService','DataRetrievalResultTypeEnum'];

function ValidateEditorController($scope, MenuAPIService, WidgetAPIService, UsersAPIService, ViewAPIService, UtilsService, GroupAPIService, VRNotificationService, VRNavigationService, WidgetSectionEnum, PeriodEnum, TimeDimensionTypeEnum, ColumnWidthEnum, BIAPIService, DataRetrievalResultTypeEnum) {
    loadParameters();
    var mainGridAPI;
    defineScope();
    load();
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null) {
            $scope.filter = {
                Audience: parameters.Audience,
                BodyContents: parameters.ViewContent.BodyContents,
                SummaryContents: parameters.ViewContent.SummaryContents,
            }
        }
    }

    function defineScope() {
        $scope.bodyWidgets = [];
        $scope.summaryWidgets = [];
        $scope.users = [];
        $scope.groups = [];
        $scope.dataSrouce = [];
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            if ($scope.widgets.length != 0)
                retrieveData();
        };
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return BIAPIService.GetUserMeasuresValidator(dataRetrievalInput)
            .then(function (response) {
                if (dataRetrievalInput.DataRetrievalResultType == DataRetrievalResultTypeEnum.Normal.value) {
                    var data = [];
                    angular.forEach(response.Data, function (itm) {
                        data.push(fillNeededData(itm));
                    });
                    response.Data = data;
                }
                $scope.dataSrouce = response.Data;
                onResponseReady(response);
            });
        };

    }
    function fillNeededData(itm) {
        for (var i = 0; i < $scope.users.length; i++) {
            if ($scope.users[i].UserId == itm.UserId)
                itm.Name = $scope.users[i].Name;
        }
        return itm;
    }
    function retrieveData() {
        var query = {
            UserIds: $scope.filter.Audience.Users,
            GroupIds: $scope.filter.Audience.Groups,
            Widgets: $scope.widgets
        }
        return mainGridAPI.retrieveData(query);


    }
    function load() {
        loadWidgets();
        loadUsers();
        if (mainGridAPI!=undefined)
        retrieveData();
    }
    function loadWidgets() {
        $scope.widgets = [];
        for (var i = 0; i < $scope.filter.BodyContents.length; i++) {
            $scope.widgets.push($scope.filter.BodyContents[i].WidgetId);
        }
        for (var i = 0; i < $scope.filter.SummaryContents.length; i++) {
            $scope.widgets.push($scope.filter.SummaryContents[i].WidgetId);
        }
    }
    function loadUsers() {
        UsersAPIService.GetUsers().then(function (response) {
            angular.forEach(response, function (users) {
                $scope.users.push(users);
            })
        });

    }



}
appControllers.controller('Security_ValidateEditorController', ValidateEditorController);
