'use strict';

app.directive('vrGenericdataDatarecordtypextrafieldsMultipleparentrecordtype', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new extraFieldCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/RecordType/Templates/MultipleParentDataRecordTypeExtraFieldsTemplate.html'
        };


        function extraFieldCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};

            var gridAPI;

            function initializeController() {
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.isGridValid = function () {
                    if ($scope.scopeModel.datasource.length == 0)
                        return 'you should add at least one item';
                    return null;
                };

                $scope.scopeModel.addParentDataRecordType = function () {
                    var gridItem = {
                        extraFieldsSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                        loadExtraFieldsSelector: UtilsService.createPromiseDeferred(),
                        isLoadingExtraFields: true
                    };
                    gridItem.loadExtraFieldsSelector.promise.then(function () { gridItem.isLoadingExtraFields = false; });
                    extendGridItem(gridItem);

                    $scope.scopeModel.datasource.push(gridItem);
                };

                $scope.scopeModel.removeParentDataRecordType = function (gridItem) {
                    var index = $scope.scopeModel.datasource.indexOf(gridItem);
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.DataRecordTypeExtraFields != undefined) {
                        for (var i = 0; i < payload.DataRecordTypeExtraFields.length; i++) {
                            var gridItem = {
                                extraFieldsSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                                loadExtraFieldsSelector: UtilsService.createPromiseDeferred(),
                                extraFieldsSelectionChangePromise: UtilsService.createPromiseDeferred(),
                                directiveReadyDeferred: UtilsService.createPromiseDeferred(),
                                loadDirective: UtilsService.createPromiseDeferred(),
                                extraField: payload.DataRecordTypeExtraFields[i]
                            };

                            promises.push(gridItem.loadExtraFieldsSelector.promise);
                            promises.push(gridItem.loadDirective.promise);

                            extendGridItem(gridItem);
                            $scope.scopeModel.datasource.push(gridItem);
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var dataRecordTypeExtraFields = [];
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++)
                        dataRecordTypeExtraFields.push($scope.scopeModel.datasource[i].directiveAPI.getData());

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordTypeExtraFields.MultipleParentDataRecordTypeExtraFields,Vanrise.GenericData.MainExtensions",
                        DataRecordTypeExtraFields: dataRecordTypeExtraFields
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendGridItem(gridItem) {
                gridItem.onDataRecordTypeExtraFieldsSelectorReady = function (api) {
                    gridItem.dataRecordTypeExtraFieldsSelectorAPI = api;
                    gridItem.extraFieldsSelectorReadyDeferred.resolve();
                };

                gridItem.extraFieldsSelectorReadyDeferred.promise.then(function () {
                    var dataRecordTypeExtraFieldsSelectorPayload;
                    if (gridItem.extraField != undefined)
                        dataRecordTypeExtraFieldsSelectorPayload = { selectedIds: gridItem.extraField.ConfigId };

                    VRUIUtilsService.callDirectiveLoad(gridItem.dataRecordTypeExtraFieldsSelectorAPI, dataRecordTypeExtraFieldsSelectorPayload, gridItem.loadExtraFieldsSelector);
                });

                gridItem.onDataRecordTypeExtraFieldsSelectionChanged = function (selectedExtraField) {
                    if (selectedExtraField == undefined)
                        return;

                    if (gridItem.extraFieldsSelectionChangePromise != undefined) {
                        gridItem.extraFieldsSelectionChangePromise.resolve();
                    }
                    else {
                        gridItem.onDirectiveReady = function (api) {
                            gridItem.directiveAPI = api;
                            var setLoader = function (value) {
                                gridItem.isLoadingDirective = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.directiveAPI, undefined, setLoader);
                        };
                    }
                };

                if (gridItem.extraField != undefined) {
                    gridItem.onDirectiveReady = function (api) {
                        gridItem.directiveAPI = api;
                        gridItem.directiveReadyDeferred.resolve();
                    };

                    gridItem.directiveReadyDeferred.promise.then(function () {
                        gridItem.extraFieldsSelectionChangePromise.promise.then(function () {
                            gridItem.extraFieldsSelectionChangePromise = undefined;
                            VRUIUtilsService.callDirectiveLoad(gridItem.directiveAPI, gridItem.extraField, gridItem.loadDirective);
                        });
                    });
                }
            }
        }

        return directiveDefinitionObject;
    }]);