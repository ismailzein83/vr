ValidateEditorController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNavigationService', 'VR_BI_BIAPIService', 'DataRetrievalResultTypeEnum', 'UtilsService', 'VRNotificationService'];

function ValidateEditorController($scope, UsersAPIService, VRNavigationService, VR_BI_BIAPIService, DataRetrievalResultTypeEnum, UtilsService, VRNotificationService) {
    var mainGridAPI;
    loadParameters();
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
            return VR_BI_BIAPIService.GetUserMeasuresValidator(dataRetrievalInput)
            .then(function (response) {
                if (dataRetrievalInput.DataRetrievalResultType == DataRetrievalResultTypeEnum.Normal.value) {
                    if (response.Data.length == 0) {
                        $scope.showMessage = "All the active audiences of the page have the rights to see all added widgets.";
                    }
                    angular.forEach(response.Data, function (itm) {
                       fillNeededData(itm);
                    });
                    
                }

                onResponseReady(response);
            });
        };

    }

    function fillNeededData(itm) {
        for (var i = 0; i < $scope.users.length; i++) {
            if ($scope.users[i].UserId == itm.UserId)
                itm.Name = $scope.users[i].Name;
            if (itm.MeasuresDenied.length == 0) {
                itm.MeasuresDenied.push("All Allowed");
            }
        }
    }

    function retrieveData() {
        var query = {
            UserIds: $scope.filter.Audience.Users,
            GroupIds: $scope.filter.Audience.Groups,
            Widgets: $scope.widgets
        };
        return mainGridAPI.retrieveData(query);


    }

    function load() {
        $scope.isLoading = true;
        UtilsService.waitMultipleAsyncOperations([ loadWidgets, loadUsers])
            .then(function () {
                if (mainGridAPI != undefined)
                    retrieveData();
            })
            .catch(function (error) {
                $scope.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });     
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
      return  UsersAPIService.GetUsers().then(function (response) {
            angular.forEach(response, function (user) {

                $scope.users.push(user);
            })
        });

    }



}
appControllers.controller('VR_BI_ValidateEditorController', ValidateEditorController);
