"use strict";

appControllers.directive("vrToolsGeneratedScriptDelete", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Tools_ColumnsAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService,VR_Tools_ColumnsAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new Delete($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Tools/Elements/GeneratedScriptDesign/Directives/MainExtensions/Templates/GeneratedScriptDeleteTemplate.html"
        };

        function Delete($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};
            $scope.keyValues = [];
            var entity = {};
            var isEditMode;
            var identifierColumnsDirectiveApi;
            var identifierColumnsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();

                $scope.scopeModel.onIdentifierColumnsDirectiveReady = function (api) {
                    identifierColumnsDirectiveApi = api;
                    identifierColumnsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.clearAll = function () {
                    $scope.keyValues.length = 0;
                };

                $scope.scopeModel.disableClear = function () {
                    if ($scope.keyValues.length == 0) return true;
                    return false;
                };

                $scope.scopeModel.disableAdd = function () {
                    if (identifierColumnsDirectiveApi == undefined || identifierColumnsDirectiveApi.getSelectedIds() == undefined || identifierColumnsDirectiveApi.getSelectedIds().length==0 || $scope.scopeModel.keyValue == undefined || $scope.scopeModel.keyValue == "" || $scope.keyValues.includes($scope.scopeModel.keyValue))
                    return true;
                    return false;

                };

                $scope.scopeModel.addKeyValue = function () {
                  $scope.keyValues.push($scope.scopeModel.keyValue);
                  $scope.scopeModel.keyValue = "";
                };

                $scope.scopeModel.validateKeyValueAddition = function () {
                        if ($scope.keyValues.length == 0)
                            return 'You Should At Least Add One Value ';
                    return null;
                };

                function loadIdentifierColumnDirective() {
                    var promises = [];
                    var identifierColumnsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    promises.push(identifierColumnsReadyPromiseDeferred.promise);
                    UtilsService.waitMultiplePromises(promises).then(function (response) {

                        var identifierColumnsPayload = {
                            filter: entity.filter
                        };
                        if (entity.Settings.IdentifierColumn != undefined)
                            identifierColumnsPayload.selectedIds = entity.Settings.IdentifierColumn.ColumnName;
                        VRUIUtilsService.callDirectiveLoad(identifierColumnsDirectiveApi, identifierColumnsPayload, identifierColumnsDirectiveLoadDeferred);
                    });

                    return identifierColumnsDirectiveLoadDeferred.promise;
                }
                function defineAPI() {
                    var api = {};

                    api.load = function (payload) {
                         entity = payload.generatedScriptSettingsEntity;
                        isEditMode = entity.isEditMode;
                        var promises = [];

                        if (isEditMode) {

                            if (entity.Settings.KeyValues)
                            for (var j = 0; j < entity.Settings.KeyValues.length; j++) {
                                $scope.keyValues.push(entity.Settings.KeyValues[j]);
                            }
                           
                            promises.push(loadIdentifierColumnDirective());
                            return UtilsService.waitMultiplePromises(promises);
                        }

                        else {

                            identifierColumnsReadyPromiseDeferred.promise.then(function (response) {

                                var setLoader = function (value) { $scope.scopeModel.isIdentifierColumnsDirectiveloading = value; };
                                var identifierColumnsDirectiveLoadDeferred;
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, identifierColumnsDirectiveApi, { filter: entity.filter }, setLoader, identifierColumnsDirectiveLoadDeferred);

                            });
                        }


                    };

                    api.getData = function () {
                  
                        return {
                            $type: "Vanrise.Common.MainExtensions.DeleteGeneratedScriptItem, Vanrise.Common.MainExtensions",
                            IdentifierColumn: { ColumnName: identifierColumnsDirectiveApi.getSelectedIds() },
                            KeyValues:$scope.keyValues
                        };
                    };

                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                }
            }
        }

        return directiveDefinitionObject;

    }
]);