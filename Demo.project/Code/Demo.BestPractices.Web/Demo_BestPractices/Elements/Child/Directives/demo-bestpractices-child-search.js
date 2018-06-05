"use strict";

app.directive("demoBestpracticesChildSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Demo_BestPractices_ChildService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Demo_BestPractices_ChildService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new ChildSearch($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_BestPractices/Elements/Child/Directives/Templates/ChildSearchTemplate.html"

    };

    function ChildSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var parentId;

        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addChild = function () {
                var parentIdItem = { ParentId: parentId };
                var onChildAdded = function (obj) {
                    gridAPI.onChildAdded(obj);
                };
                Demo_BestPractices_ChildService.addChild(onChildAdded, parentIdItem);
            };
         
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    parentId = payload.parentId;
                }
                return gridAPI.load(getGridQuery());
            };
            api.onChildAdded = function (childObject) {
                gridAPI.onChildAdded(childObject);
            };


            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getGridQuery() {
            var payload = {
                query: { ParentIds: [parentId] },
                parentId: parentId,
                hideParentColumn:true
            };
            return payload;
        }
    }

    return directiveDefinitionObject;

}]);
