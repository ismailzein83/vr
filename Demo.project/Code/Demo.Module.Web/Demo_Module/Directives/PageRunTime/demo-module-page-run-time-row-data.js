"use strict";

app.directive("demoModulePageRunTimeRowData", ["UtilsService", "VRNotificationService", "VRUIUtilsService",'Demo_Module_PageDefinitionAPIService',
function (UtilsService, VRNotificationService, VRUIUtilsService,Demo_Module_PageDefinitionAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RowData($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/PageRunTime/Templates/RowData.html"
        };

        function RowData($scope, ctrl, $attrs) {

            $scope.scopeModel = {};
            $scope.scopeModel.rowData = [];
            var pageDefinitionEntity = {};

            this.initializeController = initializeController;
            

            function initializeController() {
                
                defineAPI();

              
            }
            function getPageDefinition(payload) {
                var pageDefintionId = payload.pageRunTimeItem.PageDefinitionId;
                return Demo_Module_PageDefinitionAPIService.GetPageDefinitionById(pageDefintionId).then(function (response) {
                    pageDefinitionEntity = response;
                    if (pageDefinitionEntity.Details != undefined && pageDefinitionEntity.Details.Fields != undefined) {
                        for (var i = 0; i < pageDefinitionEntity.Details.Fields.length; i++) {
                            var field = { label: pageDefinitionEntity.Details.Fields[i].Name };
                            field.value = payload.pageRunTimeItem.FieldValues[field.label];
                            $scope.scopeModel.rowData.push(field);
                        }

                    }
                });
            }
           

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined && payload.pageRunTimeItem != undefined) {
                        getPageDefinition(payload);
                    }
                };

                api.getData = function () {
                };
                

                if (ctrl.onReady != null)
                  ctrl.onReady(api);
                
            }
        }

        return directiveDefinitionObject;

    }
]);