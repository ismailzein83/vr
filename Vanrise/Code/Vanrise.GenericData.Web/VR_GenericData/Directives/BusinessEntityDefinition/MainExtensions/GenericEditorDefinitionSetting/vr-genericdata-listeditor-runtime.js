

(function (app) {

    'use strict';

    listEditorRuntime.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function listEditorRuntime(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FaultCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/ListEditorRuntimeSettingTemplate.html"

        };
        function FaultCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var definitionSettings;
            var selectedValues;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.itemList = [];

                $scope.scopeModel.isItemValid = function () {

                    var itemIsValid = true;
                    var itemToAdd = $scope.scopeModel.itemToAdd;
                    if (itemToAdd == undefined || itemToAdd.length == 0 || itemToAdd == '') {
                        return "should add at least one element";
                    }
                    else {
                        for (var j = 0; j < $scope.scopeModel.itemList.length; j++) {
                            var listItem = $scope.scopeModel.itemList[j];
                            if (itemToAdd == listItem) {
                                return "item already exist";
                            }
                        }
                    }
                    return null;
                };
                $scope.scopeModel.addItem = function () {
                    $scope.scopeModel.itemList.push($scope.scopeModel.itemToAdd);
                    $scope.scopeModel.itemToAdd = undefined;
                };
                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var selectedValues;
                    var settings;
                    if (payload != undefined) {
                        definitionSettings = payload.definitionSettings;
                        selectedValues = payload.selectedValues;
                        if (selectedValues != undefined)
                            settings = selectedValues.Settings;
                        if (settings != undefined && settings.Codes != undefined) {
                            for (var i = 0; i < settings.Codes.length; i++) {
                                var codeItem = settings.Codes[i];
                                $scope.scopeModel.itemList.push(codeItem.Code);
                            }
                        }
                    }
                };

                api.setData = function (data) {
                    var objectList = data[definitionSettings.FieldName] = { $type: definitionSettings.RootFQTN };
                    var listItems = objectList[definitionSettings.RootFieldName] = [];
                    for (var i = 0; i < $scope.scopeModel.itemList.length; i++) {
                        var item = $scope.scopeModel.itemList[i];
                        listItems.push({
                            $type: definitionSettings.ChildFQTN,
                            [definitionSettings.ChildFieldName]: item
                        });
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataListeditorRuntime', listEditorRuntime);

})(app);
