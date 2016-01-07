﻿"use strict";

app.directive("vrIntegrationAdapterFtp", ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Integration/Directives/Adapter/Templates/AdapterFTPTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {


            $scope.extension = undefined;
            $scope.directory = undefined;
            $scope.serverIP = undefined;
            $scope.userName = undefined;
            $scope.password = undefined;
            $scope.directorytoMoveFile = undefined;
            $scope.selectedAction = undefined;

            $scope.actionsAfterImport = [{ value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];
            var api = {};
                        
            api.getData = function () {

                var extension;

                if ($scope.extension.indexOf(".") == 0)
                    extension = $scope.extension;
                else
                    extension = "." + $scope.extension;


                return {
                    $type: "Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments",
                    Extension: extension,
                    Directory: $scope.directory,
                    ServerIP: $scope.serverIP,
                    UserName: $scope.userName,
                    Password: $scope.password,
                    DirectorytoMoveFile: $scope.directorytoMoveFile,
                    ActionAfterImport: $scope.selectedAction.value
                };
            };
            api.getStateData = function () {
                return null;
            };


            api.load = function (payload) {

                if (payload != undefined) {

                    var argumentData = payload.adapterArgument;

                    if (argumentData != null) {
                        $scope.extension = argumentData.Extension;
                        $scope.directory = argumentData.Directory;
                        $scope.serverIP = argumentData.ServerIP;
                        $scope.userName = argumentData.UserName;
                        $scope.password = argumentData.Password;
                        $scope.directorytoMoveFile = argumentData.DirectorytoMoveFile;
                        $scope.selectedAction = UtilsService.getItemByVal($scope.actionsAfterImport, argumentData.ActionAfterImport, "value");
                    }

                }
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
