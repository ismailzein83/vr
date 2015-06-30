BPDefinitionManagementController.$inject = ['$scope', 'BusinessProcessAPIService', 'VRModalService', '$interval','VRNotificationService'];

function BPDefinitionManagementController($scope, BusinessProcessAPIService, VRModalService, $interval, VRNotificationService) {

    "use strict";
    var interval, mainGridAPI;


    function showStartNewInstance(BPDefinitionObj) {
        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/InstanceEditor.html', {
            BPDefinitionID: BPDefinitionObj.BPDefinitionID
        },
        {
            onScopeReady: function (modalScope) {
                modalScope.title = "Start New Instance";
                modalScope.onProcessInputCreated = function (processInstanceId) {
                    $scope.searchClicked();
                    showBPTrackingModal(processInstanceId);
                };

                modalScope.onProcessInputsCreated = function () {
                    VRNotificationService.showSuccess("Bussiness Instances created succesfully;  Open nested grid to see the created instances");
                    $scope.searchClicked();
                };
            }
        });
    }


    function showBPTrackingModal(processInstanceId) {

        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
            BPInstanceID: processInstanceId
        }, {
            onScopeReady: function (modalScope) {
                modalScope.title = "Tracking";
            }
        });
    }

    function startGetData() {
        if (angular.isDefined(interval)) return;
        interval = $interval(function callAtInterval() {
            $scope.searchClicked();
        }, 60000);
    }

    function stopGetData() {
        if (angular.isDefined(interval)) {
            $interval.cancel(interval);
            interval = undefined;
        }
    }

    function getData() {
        var pageInfo = mainGridAPI.getPageInfo();

        var title = $scope.title != undefined ? $scope.title : '';

        $scope.openedInstances = [];

        BusinessProcessAPIService.GetOpenedInstances().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.openedInstances.push(itm);
            });
            BusinessProcessAPIService.GetFilteredDefinitions(pageInfo.fromRow, pageInfo.toRow, title).then(function (response) {
                angular.forEach(response, function (def) {
                    def.OpenedInstances = [];
                    var countRunningInstances = 0;
                    angular.forEach($scope.openedInstances, function (inst) {
                        if (inst.DefinitionID == def.BPDefinitionID) {
                            countRunningInstances++;
                            def.OpenedInstances.push(inst);
                        }

                    });
                    def.RunningInstances = countRunningInstances;
                    $scope.filteredDefinitions.push(def);
                });
            });
        });
    }

    function defineGrid() {
        $scope.filteredDefinitions = [];
        $scope.gridMenuActions = [];
        $scope.loadMoreData = function () {
            return getData();
        };
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            return getData();
        };
        $scope.gridMenuActions = [{
            name: "Start New Instance",
            clicked: showStartNewInstance
        }];

        $scope.$on('$destroy', function () {
            stopGetData();
        });

    }

    $scope.searchClicked = function () {
        mainGridAPI.clearDataAndContinuePaging();
        return getData();
    };


    $scope.processInstanceClicked = function (dataItem) {
        showBPTrackingModal(dataItem.ProcessInstanceID);
    }


    defineGrid();
    startGetData();
};

appControllers.controller('BusinessProcess_BPDefinitionManagementController', BPDefinitionManagementController);


