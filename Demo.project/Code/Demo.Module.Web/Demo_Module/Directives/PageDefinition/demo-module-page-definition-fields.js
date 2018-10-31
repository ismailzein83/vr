
"use strict"
app.directive("demoModulePageDefinitionFields", ["UtilsService", "VRNotificationService", "Demo_Module_PageDefinitionService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_PageDefinitionService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var pageDefinitionFieldsGrid = new PageDefinitionFieldsGrid($scope, ctrl, $attrs);
            pageDefinitionFieldsGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/PageDefinition/Templates/PageDefinitionFieldsGridTemplate.html"
    };

    function PageDefinitionFieldsGrid($scope, ctrl) {

        var gridApi;
        var fields;
        var fieldUpdater;
        var fieldAdder;
        $scope.scopeModel = {};
        $scope.scopeModel.pageDefinitionFields = [];

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {

                    var directiveApi = {};

                    directiveApi.load = function (payload) {

                        if (payload != undefined) {
                            if (payload.Fields != undefined) {
                                fields = payload;
                                for (var i = 0; i < payload.Fields.length; i++) {
                                    var field = payload.Fields[i];
                                    $scope.scopeModel.pageDefinitionFields.push(field);
                                }
                            }

                            if (payload.FieldUpdater != undefined) {

                                fieldUpdater = payload.FieldUpdater;
                            }

                            if (payload.FieldAdder != undefined) {

                                fieldAdder = payload.FieldAdder;
                            }
                        }


                    };

                    directiveApi.getData = function () {
                        var fields = [];
                        for (var j = 0; j < $scope.scopeModel.pageDefinitionFields.length; j++) {
                            var field = $scope.scopeModel.pageDefinitionFields[j];
                            fields.push(field);
                        }
                        return { Fields: fields };
                    }

                    return directiveApi;
                };
            };

            $scope.scopeModel.onPageDefinitionFieldAdded = function () {

                var onPageDefinitionFieldAdded = function (pageDefinitionField) {
                    $scope.scopeModel.pageDefinitionFields.push(pageDefinitionField);
                };

                Demo_Module_PageDefinitionService.addPageDefinitionField(onPageDefinitionFieldAdded, fieldAdder);
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editPageDefinitionField,
            }];
        }

        function editPageDefinitionField(PageDefinitionField) {

            var index = $scope.scopeModel.pageDefinitionFields.indexOf(PageDefinitionField); console.log(index)
            var onPageDefinitionFieldUpdated = function (pageDefinitionField) {
                $scope.scopeModel.pageDefinitionFields[index] = pageDefinitionField;
            };

            Demo_Module_PageDefinitionService.editPageDefinitionField(onPageDefinitionFieldUpdated, PageDefinitionField, fieldUpdater, index);
        }

    }

    return directiveDefinitionObject;

}]);

