"use strict";

app.directive("vrGenericdataTabscontainereditorDefinition", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_GenericData_DataRecordFieldAPIService", "VR_GenericData_GenericUIRuntimeAPIService",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_GenericUIRuntimeAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TabsContainerEditor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/TabsContainerDefinitionSettingTemplate.html"
        };
        function TabsContainerEditor($scope, ctrl, $attrs) {
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
                   
                };

                api.getData = function (dicData) {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.TabsContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        TabContainers: []
                    };
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }

        return directiveDefinitionObject;
    }
]);