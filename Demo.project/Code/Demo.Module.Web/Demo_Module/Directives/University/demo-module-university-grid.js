"use strict"
app.directive("demoModuleUniversityGrid", ["VRNotificationService", "Demo_Module_UniversityAPIService", "Demo_Module_UniversityService", "Demo_Module_CollegeService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (VRNotificationService, Demo_Module_UniversityAPIService, Demo_Module_UniversityService, Demo_Module_CollegeService, VRUIUtilsService, VRCommon_ObjectTrackingService) {
        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: '='
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var universityGrid = new UniversityGrid($scope, ctrl, $attrs);
                universityGrid.initializeController();
            },

            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/University/templates/UniversityGridTemplate.html"
        };

        function UniversityGrid($scope, ctrl, $attrs) {
            var gridAPI;
            var gridDrillDownTabs;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.universities = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;

                    gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(getGridDrillDownDefinitions(), gridAPI, $scope.gridMenuActions);
                    
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveAPI());
                    }

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };

                        directiveAPI.onUniversityAdded = function (university) {
                            gridDrillDownTabs.setDrillDownExtensionObject(university);
                            gridAPI.itemAdded(university);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_UniversityAPIService.GetFilteredUniversities(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var tableItem = response.Data[i];
                                gridDrillDownTabs.setDrillDownExtensionObject(tableItem);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
                defineMenuActions();
            }

            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editUniversity,

                }, {
                    name: "Delete",
                    clicked: deleteUniversity,
                }];
            }

            function editUniversity(university) {
                var onUniversityUpdated = function (university) {
                    gridAPI.itemUpdated(university);
                }
                Demo_Module_UniversityService.editUniversity(university.Entity.UniversityId, onUniversityUpdated);
            }

            function deleteUniversity(university) {
                var onUniversityDeleted = function (university) {
                    gridAPI.itemDeleted(university);
                };
                Demo_Module_UniversityService.deleteUniversity($scope, university, onUniversityDeleted)
            }

            function getGridDrillDownDefinitions() {
                var drillDownDefinitions = [];
                drillDownDefinitions.push(getCollegeDrillDownDefinition());
                return drillDownDefinitions;
            }

            function getCollegeDrillDownDefinition() {
                var drillDownDefinition = {};
                drillDownDefinition.title = "Colleges";
                drillDownDefinition.directive = "demo-module-college-grid";
                drillDownDefinition.loadDirective = function (collegeGridAPI, tableItem) {
                    tableItem.collegeGridAPI = collegeGridAPI;
                    var query = {
                        Name: null,
                        UniversityIds: [tableItem.Entity.UniversityId]
                    };
                    return collegeGridAPI.loadGrid(query);
                };

                drillDownDefinition.parentMenuActions = [{
                    name: "Add College",
                    clicked: function (tableItem) {
                        if (drillDownDefinition.setTabSelected != undefined)
                            drillDownDefinition.setTabSelected(tableItem);

                        var onCollegeAdded = function (collegeObj) {
                            if (tableItem.collegeGridAPI != undefined) {
                                tableItem.collegeGridAPI.onCollegeAdded(collegeObj);
                            }
                        };
                        Demo_Module_CollegeService.addCollege(onCollegeAdded);
                    },
                }];
                return drillDownDefinition;
            }

            function getDrillDownToAnalyticTable() {
                var drillDownDefinition = {};

                drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
                drillDownDefinition.directive = "vr-common-objecttracking-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, analyticTableItem) {

                    analyticTableItem.objectTrackingGridAPI = directiveAPI;
                    var query = {
                        ObjectId: analyticTableItem.Entity.UniversityId,
                        EntityUniqueName: CollegeAPIService.GetCollegesInfo(ObjectId),

                    };

                    return analyticTableItem.objectTrackingGridAPI.load(query);
                };

                return drillDownDefinition;
            }
        }
        return directiveDefinitionObject;
    }]
    );