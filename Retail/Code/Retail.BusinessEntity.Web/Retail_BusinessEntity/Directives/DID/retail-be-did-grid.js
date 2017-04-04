'use strict';

app.directive('retailBeDidGrid', ['VRNotificationService', 'Retail_BE_DIDAPIService', 'Retail_BE_DIDService', 'VRUIUtilsService',
    function (VRNotificationService, Retail_BE_DIDAPIService, Retail_BE_DIDService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DIDGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/DID/Templates/DIDGridTemplate.html'
        };

        function DIDGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dids = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = defineDrillDownDefinitions();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_DIDAPIService.GetFilteredDIDs(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onDIDAdded = function (addedDID) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedDID);
                    gridAPI.itemAdded(addedDID);
                };

                api.onDIDUpdated = function (updatedDID) {
                    gridAPI.itemUpdated(updatedDID);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineDrillDownDefinitions() {
                var drillDownDefinitions = [];
                drillDownDefinitions.push(getDIDAccountRelationsDrillDownDefinition());

                var dynamicDrillDownDefinitions = Retail_BE_DIDService.getDrillDownDefinition();
                for (var index = 0; index < dynamicDrillDownDefinitions.length; index++)
                    drillDownDefinitions.push(dynamicDrillDownDefinitions[index]);


                function getDIDAccountRelationsDrillDownDefinition() {
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Subscribers";

                    drillDownDefinition.directive = "retail-be-didaccountrelations-view";

                    drillDownDefinition.loadDirective = function (directiveAPI, didItem) {
                        didItem.didAccountBERelationsGridAPI = directiveAPI;
                        var payload = {
                            didId: didItem.Entity.DIDId
                        };
                        return didItem.didAccountBERelationsGridAPI.load(payload);
                    };

                    return drillDownDefinition;
                }

                return drillDownDefinitions;
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editDID,
                    haspermission: hasEditDIDPermission
                });
            }
            function editDID(item) {
                var onDIDUpdated = function (updatedDID) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedDID);
                    gridAPI.itemUpdated(updatedDID);
                };

                Retail_BE_DIDService.editDID(item.Entity.DIDId, onDIDUpdated);
            }
            function hasEditDIDPermission() {
                return Retail_BE_DIDAPIService.HasUpdateDIDPermission()
            }
        }
    }]);
