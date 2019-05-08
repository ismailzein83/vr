'use strict';

app.directive('vrGenericdataRdbrecordstoragesettingsJoinfieldnameDirective', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new JoinFieldNameController(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBJoinSettings/Templates/JoinFieldNameDirectiveTemplate.html"
        };

        function JoinFieldNameController(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var context;
            var joinsList;
            var joinName;
            var fieldName;

            var joinNameSelectorAPI;
            var joinNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var joinNameSelectionChangedDeferred = UtilsService.createPromiseDeferred();

            var mainFieldSelectorDirectiveAPI;
            var mainFieldSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var fieldNameDirectiveAPI;
            var fieldNameDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.joinNames = [];

                $scope.scopeModel.onJoinNameSelectorReady = function (api) {
                    joinNameSelectorAPI = api;
                    joinNameSelectorReadyDeferred.resolve();
                    defineAPI();
                };

                $scope.scopeModel.onMainFieldSelectorDirectiveReady = function (api) {
                    mainFieldSelectorDirectiveAPI = api;
                    mainFieldSelectorDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onFieldNameDirectiveReady = function (api) {
                    fieldNameDirectiveAPI = api;
                    fieldNameDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onJoinNameSelectionChanged = function (item) {
                    if (item != undefined) {
                        if (joinNameSelectionChangedDeferred != undefined) {
                            joinNameSelectionChangedDeferred.resolve();
                        }
                        else {
                            if (item.Settings.StorageFieldEditor != $scope.scopeModel.storageFieldEditor) {
                                fieldNameDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
                            }

                            $scope.scopeModel.storageFieldEditor = item.Settings.StorageFieldEditor;

                            fieldNameDirectiveReadyDeferred.promise.then(function () {
                                var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value; };
                                var directivePayload = {
                                    settings: item.Settings
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldNameDirectiveAPI, directivePayload, setLoader);
                            });
                        }
                    }
                    else {
                        $scope.scopeModel.storageFieldEditor = undefined;

                        if (joinNameSelectionChangedDeferred != undefined) {
                            joinNameSelectionChangedDeferred.resolve();
                        }
                        else {
                            mainFieldSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

                            mainFieldSelectorDirectiveReadyDeferred.promise.then(function () {
                                var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value; };
                                var mainDataRecordTypeFieldSelectorPayload = {
                                    dataRecordTypeId: context.getMainDataRecordTypeId()
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, mainFieldSelectorDirectiveAPI, mainDataRecordTypeFieldSelectorPayload, setLoader);
                            });
                        }
                    }
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        joinName = payload.joinName;
                        fieldName = payload.fieldName;
                        joinsList = context.getJoinsList();
                    }

                    if (joinsList != undefined) {
                        for (var i = 0; i < joinsList.length; i++) {
                            var join = joinsList[i];
                            $scope.scopeModel.joinNames.push(join);
                        }
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            if (joinName != undefined) {
                                $scope.scopeModel.selectedjoin = UtilsService.getItemByVal(joinsList, joinName, "RDBRecordStorageJoinName");

                                if ($scope.scopeModel.selectedjoin != undefined)
                                    $scope.scopeModel.storageFieldEditor = $scope.scopeModel.selectedjoin.Settings.StorageFieldEditor;

                                directivePromises.push(loadFieldNameDirective());
                            }
                            else {
                                directivePromises.push(loadMainFieldsSelector());
                            }


                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                        joinNameSelectionChangedDeferred = undefined;
                    });
                };

                api.getData = function () {
                    return {
                        JoinName: $scope.scopeModel.selectedjoin != undefined ? $scope.scopeModel.selectedjoin.RDBRecordStorageJoinName : undefined,
                        FieldName: $scope.scopeModel.selectedjoin != undefined ? fieldNameDirectiveAPI.getData() : mainFieldSelectorDirectiveAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadMainFieldsSelector() {
                var mainfieldSelectorsLoadDeferred = UtilsService.createPromiseDeferred();

                mainFieldSelectorDirectiveReadyDeferred.promise.then(function () {
                    var mainDataRecordTypeFieldSelectorPayload = {
                        dataRecordTypeId: context.getMainDataRecordTypeId(),
                        selectedIds: fieldName
                    };
                    VRUIUtilsService.callDirectiveLoad(mainFieldSelectorDirectiveAPI, mainDataRecordTypeFieldSelectorPayload, mainfieldSelectorsLoadDeferred);
                });
                return mainfieldSelectorsLoadDeferred.promise;
            }

            function loadFieldNameDirective() {
                var fieldNameLoadDeferred = UtilsService.createPromiseDeferred();

                fieldNameDirectiveReadyDeferred.promise.then(function () {
                    var directiveFieldNamePayload = {
                        settings: $scope.scopeModel.selectedjoin.Settings,
                        fieldName: fieldName
                    };
                    VRUIUtilsService.callDirectiveLoad(fieldNameDirectiveAPI, directiveFieldNamePayload, fieldNameLoadDeferred);
                });
                return fieldNameLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);