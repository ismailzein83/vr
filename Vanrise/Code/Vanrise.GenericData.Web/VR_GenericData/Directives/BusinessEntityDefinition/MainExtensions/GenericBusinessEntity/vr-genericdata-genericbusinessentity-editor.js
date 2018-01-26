"use strict";

app.directive("vrGenericdataGenericbusinessentityEditor", ["UtilsService", "VRNotificationService","VR_GenericData_DataRecordTypeAPIService","VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordTypeAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var genericBusinessEntityDefinitionEditor = new GenericBusinessEntityDefinitionEditor($scope, ctrl, $attrs);
                genericBusinessEntityDefinitionEditor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Templates/GenericBusinessEntityDefinitionEditor.html"

        };

        function GenericBusinessEntityDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModal = {};
            
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.GenericBEDefinitionSettings, Vanrise.GenericData.Entities",
                       
                    };
                };

                api.load = function (payload) {
                    var businessEntityDefinitionSettings;
                    var promises = [];
                    if (payload != undefined)
                    {
                        businessEntityDefinitionSettings = payload.businessEntityDefinitionSettings;
                   
                    }                   
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
           
        }

        return directiveDefinitionObject;

    }
]);