"use strict";

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
            };
        },
        templateUrl: "/Client/Modules/Integration/Directives/Adapter/Templates/AdapterFTPTemplate.html"
    };
    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            $scope.actionsAfterImport = [{ value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];

            $scope.actionIsRequired = function () {
                return !$scope.basedOnTime;
            };

            defineAPI();
        }

        function defineAPI() {

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
                    Mask: $scope.mask == undefined ? "" : $scope.mask,
                    Directory: $scope.directory,
                    ServerIP: $scope.serverIP,
                    UserName: $scope.userName,
                    Password: $scope.password,
                    DirectorytoMoveFile: $scope.directorytoMoveFile,
                    ActionAfterImport: $scope.selectedAction ? $scope.selectedAction.value : undefined,
                    BasedOnLastModifiedTime: $scope.basedOnTime
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
                        $scope.mask = argumentData.Mask;
                        $scope.directory = argumentData.Directory;
                        $scope.serverIP = argumentData.ServerIP;
                        $scope.userName = argumentData.UserName;
                        $scope.password = argumentData.Password;
                        $scope.directorytoMoveFile = argumentData.DirectorytoMoveFile;
                        $scope.selectedAction = UtilsService.getItemByVal($scope.actionsAfterImport, argumentData.ActionAfterImport, "value");
                        $scope.basedOnTime = argumentData.BasedOnLastModifiedTime;
                    }

                }
            };


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
