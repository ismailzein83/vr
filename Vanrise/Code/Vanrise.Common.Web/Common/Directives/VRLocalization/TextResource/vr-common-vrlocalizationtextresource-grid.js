"use strict";

app.directive("vrCommonLocalizationtextresourceGrid", ["VRNotificationService", 'VRCommon_VRLocalizationTextResourceAPIService', 'VRCommon_VRLocalizationTextResourceService',
    function (VRNotificationService, VRCommon_VRLocalizationTextResourceAPIService, VRCommon_VRLocalizationTextResourceService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrLocalizationTextResourceGrid = new VRLocalizationTextResourceGrid($scope, ctrl, $attrs);
                vrLocalizationTextResourceGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: '/Client/Modules/Common/Directives/VRLocalization/TextResource/Templates/VRLocalizationTextResourceGridTemplate.html'
        };
        function VRLocalizationTextResourceGrid($scope, ctrl, $attrs) {

            var gridAPI;

            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.vrLocalizationTextResources = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRLocalizationTextResourceAPIService.GetFilteredVRLocalizationTextResources(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {

                var api = {};

                api.load = function (query) {
                    if (query.showModule != undefined)
                    {
                        $scope.scopeModel.moduleShow = false;
                    }
                    else {
                        $scope.scopeModel.moduleShow = true;
                    }
                    return gridAPI.retrieveData(query);
                };

                api.onVRLocalizationTextResourceAdded = function (addedVRLocalizationTextResource) {
                    gridAPI.itemAdded(addedVRLocalizationTextResource);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: "Edit",
                    clicked: editVRLocalizationTextResource
                }];
            }

            function editVRLocalizationTextResource(vrLocalizationTextResourceItem) {

                var onVRLocalizationTextResourceUpdated = function (updatedvrLocalizationTextResource) {
                    gridAPI.itemUpdated(updatedvrLocalizationTextResource);
                };

                VRCommon_VRLocalizationTextResourceService.editVRLocalizationTextResource(vrLocalizationTextResourceItem.VRLocalizationTextResourceId, onVRLocalizationTextResourceUpdated);
            }
        }

        return directiveDefinitionObject;
    }
]);