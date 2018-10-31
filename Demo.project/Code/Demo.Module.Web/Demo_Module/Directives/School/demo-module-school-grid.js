"use strict"
app.directive("demoModuleSchoolGrid", ["UtilsService", "VRNotificationService", "Demo_Module_SchoolAPIService", "Demo_Module_SchoolService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_SchoolAPIService, Demo_Module_SchoolService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var schoolGrid = new SchoolGrid($scope, ctrl, $attrs);
            schoolGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/School/Templates/SchoolGridTemplate.html"
    };

    function SchoolGrid($scope, ctrl) {

        var gridApi;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.schools = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                var drillDownDefinitions = [];
                AddStudentDrillDown();
                function AddStudentDrillDown() {
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Student";
                    drillDownDefinition.directive = "demo-module-student-search";

                    drillDownDefinition.loadDirective = function (directiveAPI, schoolItem) {
                        schoolItem.studentGridAPI = directiveAPI;
                        var payload = {
                            schoolId: schoolItem.SchoolId
                        };
                        return schoolItem.studentGridAPI.load(payload);
                    };
                    drillDownDefinitions.push(drillDownDefinition);
                }

                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }



                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        var query = payload.query;
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onSchoolAdded = function (school) {
                        gridApi.itemAdded(school);
                    };
                    return directiveApi;
                };
            };


            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_SchoolAPIService.GetFilteredSchools(dataRetrievalInput)
                .then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                        }
                    }
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editSchool,

            }];
        };
        function editSchool(school) {
            var onSchoolUpdated = function (school) {
                gridApi.itemUpdated(school);
                gridDrillDownTabsObj.setDrillDownExtensionObject(school);

            };
            Demo_Module_SchoolService.editSchool(school.SchoolId, onSchoolUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
