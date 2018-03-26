"use strict"
app.directive("demoModuleCollegeGrid", ["VRNotificationService", "Demo_Module_CollegeAPIService", "Demo_Module_CollegeService",
    function (VRNotificationService, Demo_Module_CollegeAPIService, Demo_Module_CollegeService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: '='
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var collegeGrid = new CollegeGrid($scope, ctrl, $attrs);
                collegeGrid.initializeController();
            },

            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/College/templates/CollegeGridTemplate.html"
        };

        function CollegeGrid($scope, ctrl, $attrs) {
            $scope.colleges = [];
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveAPI());
                    }

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onCollegeAdded = function (college) {
                            gridAPI.itemAdded(college);
                        };
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_CollegeAPIService.GetFilteredColleges(dataRetrievalInput)
                    .then(function (response) {
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
                    clicked: editCollege,

                }, {
                    name: "Delete",
                    clicked: deleteCollege,
                }];
            }

            function editCollege(college) {
                var onCollegeUpdated = function (college) {
                    gridAPI.itemUpdated(college);
                };
                Demo_Module_CollegeService.editCollege(college.CollegeId,college.UniversityName, onCollegeUpdated);
            }

            function deleteCollege(college) {
                var onCollegeDeleted = function (college) {
                    gridAPI.itemDeleted(college);
                };
                Demo_Module_CollegeService.deleteCollege($scope, college, onCollegeDeleted)
            }


        }
        return directiveDefinitionObject;
    }]
    );