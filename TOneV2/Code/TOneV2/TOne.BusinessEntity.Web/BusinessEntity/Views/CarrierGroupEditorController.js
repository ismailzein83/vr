CarrierGroupEditorController.$inject = ['$scope', 'CarrierGroupAPIService', 'CarrierAccountAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'CarrierTypeEnum', 'UtilsService'];

function CarrierGroupEditorController($scope, CarrierGroupAPIService, CarrierAccountAPIService, VRModalService, VRNotificationService, VRNavigationService, CarrierTypeEnum, UtilsService) {


    var treeAPI;
    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.carrierGroupId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.carrierGroupId = parameters.carrierGroupId;

        if ($scope.carrierGroupId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {

        $scope.selectedvalues = [];

        $scope.datasource = [];

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.saveGroup = function () {
            if (editMode) {
               return updateGroup();
            }
            else {
               return insertGroup();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

    }


    function load() {
        $scope.isGettingData = true;

        loadTree().finally(function () {
            $scope.isGettingData = false;
            treeAPI.refreshTree($scope.beList);
        });


        loadCarriers();

        $scope.isGettingData = true;

        UtilsService.waitMultipleAsyncOperations([loadCarriers])
                    .then(function () {
                        
                        if (editMode) {
                            //Load Selected
                            CarrierGroupAPIService.GetCarrierGroupMembers($scope.carrierGroupId).then(function (response) {

                                angular.forEach(response, function (item) {
                                    $scope.selectedvalues.push(item);
                                });
                                loadCarrierGroup();
                                

                            }).catch(function (error) {
                                $scope.isGettingData = false;
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                            });
                        }
                        else {
                            $scope.isGettingData = false;
                        }

                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                    .finally(function () {
                        $scope.isGettingData = false;
                    });
    }

    function loadTree() {
        return CarrierGroupAPIService.GetEntityNodes()
           .then(function (response) {
               $scope.beList = response;
           });
    }

    function loadCarrierGroup() {
        return CarrierGroupAPIService.GetCarrierGroup($scope.carrierGroupId).then(function (response) {
            $scope.name = response.Name;
            
            if ($scope.beList.length > 0) {
                $scope.currentNode = treeAPI.setSelectedNode($scope.beList, response.ID);
                treeAPI.refreshTree($scope.beList);
            }
                
        });
    }

    function addIsSelected(menuList, Id) {
        for (var i = 0; i < menuList.length; i++) {
            if (menuList[i].EntityId == Id) {
                menuList[i].isSelected = true;
                menuList[i].isOpened = true;
                $scope.currentNode = menuList[i];
                return true;
            }
            if (menuList[i].Children != undefined)
                menuList[i].isOpened = addIsSelected(menuList[i].Children, Id);
        }
        return false;
    }


    function loadCarriers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.SaleZone.value).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.datasource.push(itm);
            });
        });
    }

    function getGroup() {
        return CarrierAccountAPIService.GetGroup($scope.groupId)
           .then(function (response) {
               fillScopeFromGroupObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function buildGroupObjFromScope() {
        var selectedCarrierIds = [];

        angular.forEach($scope.selectedvalues, function (carrierGroup) {
            selectedCarrierIds.push(carrierGroup.CarrierAccountID);
        });

        if ($scope.currentNode === undefined)
            $scope.currentNode = {
                EntityId : 0
            };

        if (typeof $scope.currentNode.EntityId === 'undefined')
            $scope.currentNode.EntityId = 0;

        var groupObj = {
            ID: $scope.carrierGroupId,
            ParentID: $scope.currentNode.EntityId,
            Name: $scope.name,
            Members: selectedCarrierIds

        };
        return groupObj;
    }

    function insertGroup() {
        $scope.issaving = true;
        var groupObj = buildGroupObjFromScope();

        return CarrierGroupAPIService.AddGroup(groupObj)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Carrier Group", response)) {
                if ($scope.onTreeAdded != undefined)
                    $scope.onTreeAdded();

                $scope.modalContext.closeModal();
            }

        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }

    function updateGroup() {
        
        var groupObj = buildGroupObjFromScope();

        CarrierGroupAPIService.UpdateGroup(groupObj)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Carrier Group", response)) {
                if ($scope.onTreeUpdated != undefined)
                    $scope.onTreeUpdated();
                $scope.modalContext.closeModal();

            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}

appControllers.controller('BusinessEntity_CarrierGroupEditorController', CarrierGroupEditorController);
