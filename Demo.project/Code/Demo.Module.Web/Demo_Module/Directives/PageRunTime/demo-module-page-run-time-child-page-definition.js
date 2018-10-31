"use strict";

app.directive("demoModulePageRunTimeChildPageDefinition", ["UtilsService", "VRNotificationService", "VRUIUtilsService", 'Demo_Module_PageDefinitionAPIService',
function (UtilsService, VRNotificationService, VRUIUtilsService,Demo_Module_PageDefinitionAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ChildPageDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/PageRunTime/Templates/PageRunTimeChildPageDefinition.html"
        };

        function ChildPageDefinition($scope, ctrl, $attrs) {
            var gridApi;
            var fieldName;
            var filterValue = {}
            var pageRunTimeItem;
            $scope.scopeModel = {};
            $scope.scopeModel.childPageDefinition = [];
            var pageDefinitionEntity = {};
            var onGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            

            this.initializeController = initializeController;
            

            function initializeController() {

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    onGridReadyPromiseDeferred.resolve();
                };
                
                onGridReadyPromiseDeferred.promise.then(function (response) {
                    defineAPI();
                });
              
            }


            function getPageDefinition(payload) {
                var pageDefintionId = payload.pageDefinitionSubViewSettings.PageDefinitionId;
                return Demo_Module_PageDefinitionAPIService.GetPageDefinitionById(pageDefintionId).then(function (response) {
                    pageDefinitionEntity = response;
                   
                });
            }

            function getFilter() {

                var object = {};

                object.query = {
                    pageDefinitionId: pageDefinitionEntity.PageDefinitionId,
                    Filters: filterValue
                }
                object.fields = [];
                object.subviews = [];

                if (pageDefinitionEntity.Details != undefined) {
                    if (pageDefinitionEntity.Details.Fields != undefined) {
                        for (var i = 0; i < pageDefinitionEntity.Details.Fields.length; i++) {
                            var field = pageDefinitionEntity.Details.Fields[i];
                            object.fields.push(field);
                        }
                    }

                    if (pageDefinitionEntity.Details.SubViews != undefined) {
                        for (var i = 0; i < pageDefinitionEntity.Details.SubViews.length; i++) {
                            var subview = pageDefinitionEntity.Details.SubViews[i];
                            object.subviews.push(subview); 
                        }
                    }
                }
                return object;
            }
           

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.pageRunTimeItem != undefined) {

                        pageRunTimeItem = payload.pageRunTimeItem;
                        fieldName = payload.pageDefinitionSubViewSettings.FieldName;

                      return  getPageDefinition(payload).then(function (response) {

                            if (pageDefinitionEntity.Details != undefined && pageDefinitionEntity.Details.Fields != undefined) {
                                for (var i = 0; i < pageDefinitionEntity.Details.Fields.length; i++) {
                                    var field = pageDefinitionEntity.Details.Fields[i];
                                    if (fieldName == field.Name) {
                                        filterValue[fieldName] = (pageRunTimeItem.PageRunTimeId).toString();
                                        break;
                                    }
                                }

                            }
                            gridApi.load(getFilter());
                      });
                    }
                    return UtilsService.waitMultiplePromises(promises);

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