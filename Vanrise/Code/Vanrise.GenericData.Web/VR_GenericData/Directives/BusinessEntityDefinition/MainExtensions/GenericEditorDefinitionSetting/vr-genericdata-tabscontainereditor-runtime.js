"use strict";

app.directive("vrGenericdataTabscontainereditorRuntime", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_GenericData_DataRecordFieldAPIService", "VR_GenericData_GenericUIRuntimeAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericUIRuntimeAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericEditorDefinitionSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/TabsContainerRuntimeSettingTemplate.html"
        };
        function GenericEditorDefinitionSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedValues;
            var definitionSettings;
            var dataRecordTypeId;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tabContainers = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined)
                    {
                        selectedValues = payload.selectedValues;
                        definitionSettings = payload.definitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                    }
                    if (definitionSettings != undefined)
                    {
                        if(definitionSettings.TabContainers != undefined)
                        {
                            for(var i=0; i< definitionSettings.TabContainers.length; i++)
                            {
                                var tabContainer = definitionSettings.TabContainers[i];
                                var tabContainerDef = {
                                    payload: tabContainer,
                                    directiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    directiveLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                };
                                addTabContainer(tabContainerDef);
                                promises.push(tabContainerDef.directiveLoadPromiseDeferred.promise);
                            }
                        }
                    }
                    function addTabContainer(tabContainerDef)
                    {
                        var tabContainer = {
                            showTab :tabContainerDef.payload.ShowTab,
                            tabTitle: tabContainerDef.payload.TabTitle,
                            runtimeEditor: tabContainerDef.payload.TabSettings.RuntimeEditor,
                        };
                        tabContainer.onEditorRuntimeDirectiveReady = function (api) {
                            tabContainer.editorRuntimeAPI = api;
                            tabContainerDef.directiveReadyPromiseDeferred.resolve();
                        };
                        tabContainerDef.directiveReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                dataRecordTypeId: dataRecordTypeId,
                                definitionSettings: tabContainerDef.payload.TabSettings,
                                selectedValues: selectedValues
                            };
                            VRUIUtilsService.callDirectiveLoad(tabContainer.editorRuntimeAPI, directivePayload, tabContainerDef.directiveLoadPromiseDeferred);
                        });
                        $scope.scopeModel.tabContainers.push(tabContainer);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {
                    for (var i = 0; i < $scope.scopeModel.tabContainers.length; i++) {
                        var tabContainer = $scope.scopeModel.tabContainers[i];
                        tabContainer.editorRuntimeAPI.setData(dicData);
                    }
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }

        return directiveDefinitionObject;
    }
]);