(function (app) {

    'use strict';

    CDRCorrelationDefinitionSettings.$inject = ['UtilsService', 'VRUIUtilsService'];

    function CDRCorrelationDefinitionSettings(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CDRCorrelationDefinitionSettingsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/CDRCorrelation/Templates/CDRCorrelationDefinitionSettings.html'
        };

        function CDRCorrelationDefinitionSettingsCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var mergeDataTransformationAPI;
            var mergeDataTransformationSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var inputDataRecordTypeSelectorAPI;
            var inputDataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var onInputRecordTypeSelectionChangedDeferred;

            var inputDataRecordStorageSelectorAPI;
            var inputDataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var callingNumberFieldSelectorAPI;
            var callingNumberFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var calledNumberFieldSelectorAPI;
            var calledNumberFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var durationFieldSelectorAPI;
            var durationFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var datetimeFieldSelectorAPI;
            var datetimeFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var outputDataRecordTypeSelectorAPI;
            var outputDataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var onOutputRecordTypeSelectionChangedDeferred;

            var outputDataRecordStorageSelectorAPI;
            var outputDataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var settings;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataTransformationSelectorDirectiveReady = function (api) {
                    mergeDataTransformationAPI = api;
                    mergeDataTransformationSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onInputDataRecordTypeSelectorReady = function (api) {
                    inputDataRecordTypeSelectorAPI = api;
                    inputDataRecordTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onInputDataRecordStorageSelectorReady = function (api) {
                    inputDataRecordStorageSelectorAPI = api;
                    inputDataRecordStorageSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onInputRecordTypeSelectionChanged = function (selectedInputRecordType) {
                    if (selectedInputRecordType != undefined) {
                        if (onInputRecordTypeSelectionChangedDeferred != undefined) {
                            onInputRecordTypeSelectionChangedDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.isInputSectionLoading = true;
                            var reLoadInputPromises = [];

                            reLoadInputPromises.push(reLoadInputDataRecordStorageSelector(selectedInputRecordType));
                            reLoadInputPromises.push(reLoadCallingNumberFieldSelector(selectedInputRecordType));
                            reLoadInputPromises.push(reLoadCalledNumberFieldSelector(selectedInputRecordType));
                            reLoadInputPromises.push(reLoadDurationFieldSelector(selectedInputRecordType));
                            reLoadInputPromises.push(reLoadDatetimeFieldSelector(selectedInputRecordType));

                            function reLoadInputDataRecordStorageSelector(selectedInputRecordType) {
                                var loadInputDataRecordStorageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                                inputDataRecordStorageSelectorReadyDeferred.promise.then(function () {
                                    var directivePayload = {
                                        DataRecordTypeId: selectedInputRecordType.DataRecordTypeId
                                    };
                                    VRUIUtilsService.callDirectiveLoad(inputDataRecordStorageSelectorAPI, directivePayload, loadInputDataRecordStorageSelectorPromiseDeferred);
                                });
                                return loadInputDataRecordStorageSelectorPromiseDeferred.promise;
                            }

                            function reLoadCallingNumberFieldSelector(selectedInputRecordType) {
                                var loadCallingNumberFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                                callingNumberFieldSelectorReadyDeferred.promise.then(function () {
                                    var directivePayload = {
                                        dataRecordTypeId: selectedInputRecordType.DataRecordTypeId
                                    };
                                    VRUIUtilsService.callDirectiveLoad(callingNumberFieldSelectorAPI, directivePayload, loadCallingNumberFieldSelectorPromiseDeferred);
                                });
                                return loadCallingNumberFieldSelectorPromiseDeferred.promise;
                            }

                            function reLoadCalledNumberFieldSelector(selectedInputRecordType) {
                                var loadCalledNumberFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                                calledNumberFieldSelectorReadyDeferred.promise.then(function () {
                                    var directivePayload = {
                                        dataRecordTypeId: selectedInputRecordType.DataRecordTypeId
                                    };
                                    VRUIUtilsService.callDirectiveLoad(calledNumberFieldSelectorAPI, directivePayload, loadCalledNumberFieldSelectorPromiseDeferred);
                                });
                                return loadCalledNumberFieldSelectorPromiseDeferred.promise;
                            }

                            function reLoadDurationFieldSelector(selectedInputRecordType) {
                                var loadDurationFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                                durationFieldSelectorReadyDeferred.promise.then(function () {
                                    var directivePayload = {
                                        dataRecordTypeId: selectedInputRecordType.DataRecordTypeId
                                    };
                                    VRUIUtilsService.callDirectiveLoad(durationFieldSelectorAPI, directivePayload, loadDurationFieldSelectorPromiseDeferred);
                                });
                                return loadDurationFieldSelectorPromiseDeferred.promise;
                            }

                            function reLoadDatetimeFieldSelector(selectedInputRecordType) {
                                var loadDatetimeFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                                datetimeFieldSelectorReadyDeferred.promise.then(function () {
                                    var directivePayload = {
                                        dataRecordTypeId: selectedInputRecordType.DataRecordTypeId
                                    };
                                    VRUIUtilsService.callDirectiveLoad(datetimeFieldSelectorAPI, directivePayload, loadDatetimeFieldSelectorPromiseDeferred);
                                });
                                return loadDatetimeFieldSelectorPromiseDeferred.promise;
                            }

                            UtilsService.waitMultiplePromises(reLoadInputPromises).then(function () {
                                $scope.scopeModel.isInputSectionLoading = false;
                            });
                        }
                    }
                };

                $scope.scopeModel.onCallingNumberFieldSelectorDirectiveReady = function (api) {
                    callingNumberFieldSelectorAPI = api;
                    callingNumberFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onCalledNumberFieldSelectorDirectiveReady = function (api) {
                    calledNumberFieldSelectorAPI = api;
                    calledNumberFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDurationFieldSelectorDirectiveReady = function (api) {
                    durationFieldSelectorAPI = api;
                    durationFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDatetimeFieldSelectorDirectiveReady = function (api) {
                    datetimeFieldSelectorAPI = api;
                    datetimeFieldSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onOutputDataRecordTypeSelectorReady = function (api) {
                    outputDataRecordTypeSelectorAPI = api;
                    outputDataRecordTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onOutputDataRecordStorageSelectorReady = function (api) {
                    outputDataRecordStorageSelectorAPI = api;
                    outputDataRecordStorageSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onOutputRecordTypeSelectionChanged = function (selectedOutputRecordType) {
                    if (selectedOutputRecordType != undefined) {
                        if (onOutputRecordTypeSelectionChangedDeferred != undefined) {
                            onOutputRecordTypeSelectionChangedDeferred.resolve();
                        }
                        else {
                            outputDataRecordStorageSelectorReadyDeferred.promise.then(function () {
                                var directivePayload = {
                                    DataRecordTypeId: selectedOutputRecordType.DataRecordTypeId
                                };
                                var setLoader = function (value) { $scope.scopeModel.isOutputDataRecordStorageLoading = value; };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, outputDataRecordStorageSelectorAPI, directivePayload, setLoader);
                            });
                        }
                    }
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.componentType != undefined) {
                        $scope.scopeModel.name = payload.componentType.Name;
                        settings = payload.componentType.Settings;
                    }

                    var promises = [];

                    promises.push(loadMergeDataTransformationSelector());
                    promises.push(loadInputDataRecordTypeSelector());
                    promises.push(loadOutputDataRecordTypeSelector());

                    if (settings != undefined) {
                        onInputRecordTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                        onOutputRecordTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var loadInputPromises = [];
                        var loadInputDataRecordStorageSelectorPromise = loadInputDataRecordStorageSelector();
                        var loadCallingNumberFieldSelectorPromise = loadCallingNumberFieldSelector();
                        var loadCalledNumberFieldSelectorPromise = loadCalledNumberFieldSelector();
                        var loadDurationFieldSelectorPromise = loadDurationFieldSelector();
                        var loadDatetimeFieldSelectorPromise = loadDatetimeFieldSelector();

                        loadInputPromises.push(loadInputDataRecordStorageSelectorPromise);
                        loadInputPromises.push(loadCallingNumberFieldSelectorPromise);
                        loadInputPromises.push(loadCalledNumberFieldSelectorPromise);
                        loadInputPromises.push(loadDurationFieldSelectorPromise);
                        loadInputPromises.push(loadDatetimeFieldSelectorPromise);
                        UtilsService.waitMultiplePromises(loadInputPromises).then(function () {
                            onInputRecordTypeSelectionChangedDeferred = undefined;
                        });

                        promises.concat(loadInputPromises);

                        var loadOutputDataRecordStorageSelectorPromise = loadOutputDataRecordStorageSelector();

                        loadOutputDataRecordStorageSelectorPromise.then(function () {
                            onOutputRecordTypeSelectionChangedDeferred = undefined;
                        });

                        promises.push(loadOutputDataRecordStorageSelectorPromise);
                    }

                    function loadMergeDataTransformationSelector() {
                        var loadMergeDataTransformationSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        mergeDataTransformationSelectorReadyDeferred.promise.then(function () {
                            var directivePayload;
                            if (settings != undefined) {
                                directivePayload = {
                                    selectedIds: settings.MergeDataTransformationDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(mergeDataTransformationAPI, directivePayload, loadMergeDataTransformationSelectorPromiseDeferred);
                        });
                        return loadMergeDataTransformationSelectorPromiseDeferred.promise;
                    }

                    function loadInputDataRecordTypeSelector() {
                        var loadInputDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        inputDataRecordTypeSelectorReadyDeferred.promise.then(function () {
                            var directivePayload;
                            if (settings != undefined) {
                                directivePayload = {
                                    selectedIds: settings.InputDataRecordTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(inputDataRecordTypeSelectorAPI, directivePayload, loadInputDataRecordTypeSelectorPromiseDeferred);
                        });
                        return loadInputDataRecordTypeSelectorPromiseDeferred.promise;
                    }

                    function loadInputDataRecordStorageSelector() {
                        var loadInputDataRecordStorageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        onInputRecordTypeSelectionChangedDeferred.promise.then(function () {
                            inputDataRecordStorageSelectorReadyDeferred.promise.then(function () {
                                var directivePayload = {
                                    DataRecordTypeId: inputDataRecordTypeSelectorAPI.getSelectedIds()
                                };
                                if (settings != undefined) {
                                    directivePayload.selectedIds = settings.InputDataRecordStorageId;
                                }
                                VRUIUtilsService.callDirectiveLoad(inputDataRecordStorageSelectorAPI, directivePayload, loadInputDataRecordStorageSelectorPromiseDeferred);
                            });
                        });
                        return loadInputDataRecordStorageSelectorPromiseDeferred.promise;
                    }

                    function loadCallingNumberFieldSelector() {
                        var loadCallingNumberFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        onInputRecordTypeSelectionChangedDeferred.promise.then(function () {
                            callingNumberFieldSelectorReadyDeferred.promise.then(function () {
                                var directivePayload = {
                                    dataRecordTypeId: inputDataRecordTypeSelectorAPI.getSelectedIds()
                                };
                                if (settings != undefined) {
                                    directivePayload.selectedIds = settings.CallingNumberFieldName;
                                }
                                VRUIUtilsService.callDirectiveLoad(callingNumberFieldSelectorAPI, directivePayload, loadCallingNumberFieldSelectorPromiseDeferred);
                            });
                        });
                        return loadCallingNumberFieldSelectorPromiseDeferred.promise;
                    }

                    function loadCalledNumberFieldSelector() {
                        var loadCalledNumberFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        onInputRecordTypeSelectionChangedDeferred.promise.then(function () {
                            calledNumberFieldSelectorReadyDeferred.promise.then(function () {
                                var directivePayload = {
                                    dataRecordTypeId: inputDataRecordTypeSelectorAPI.getSelectedIds()
                                };
                                if (settings != undefined) {
                                    directivePayload.selectedIds = settings.CalledNumberFieldName;
                                }
                                VRUIUtilsService.callDirectiveLoad(calledNumberFieldSelectorAPI, directivePayload, loadCalledNumberFieldSelectorPromiseDeferred);
                            });
                        });
                        return loadCalledNumberFieldSelectorPromiseDeferred.promise;
                    }

                    function loadDurationFieldSelector() {
                        var loadDurationFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        onInputRecordTypeSelectionChangedDeferred.promise.then(function () {
                            durationFieldSelectorReadyDeferred.promise.then(function () {
                                var directivePayload = {
                                    dataRecordTypeId: inputDataRecordTypeSelectorAPI.getSelectedIds()
                                };
                                if (settings != undefined) {
                                    directivePayload.selectedIds = settings.DurationFieldName;
                                }
                                VRUIUtilsService.callDirectiveLoad(durationFieldSelectorAPI, directivePayload, loadDurationFieldSelectorPromiseDeferred);
                            });
                        });
                        return loadDurationFieldSelectorPromiseDeferred.promise;
                    }

                    function loadDatetimeFieldSelector() {
                        var loadDatetimeFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        onInputRecordTypeSelectionChangedDeferred.promise.then(function () {
                            datetimeFieldSelectorReadyDeferred.promise.then(function () {
                                var directivePayload = {
                                    dataRecordTypeId: inputDataRecordTypeSelectorAPI.getSelectedIds()
                                };
                                if (settings != undefined) {
                                    directivePayload.selectedIds = settings.DatetimeFieldName;
                                }
                                VRUIUtilsService.callDirectiveLoad(datetimeFieldSelectorAPI, directivePayload, loadDatetimeFieldSelectorPromiseDeferred);
                            });
                        });
                        return loadDatetimeFieldSelectorPromiseDeferred.promise;
                    }

                    function loadOutputDataRecordTypeSelector() {
                        var loadOutputDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        outputDataRecordTypeSelectorReadyDeferred.promise.then(function () {
                            var directivePayload;
                            if (settings != undefined) {
                                directivePayload = {
                                    selectedIds: settings.OutputDataRecordTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(outputDataRecordTypeSelectorAPI, directivePayload, loadOutputDataRecordTypeSelectorPromiseDeferred);
                        });
                        return loadOutputDataRecordTypeSelectorPromiseDeferred.promise;
                    }

                    function loadOutputDataRecordStorageSelector() {
                        var loadOutputDataRecordStorageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        onOutputRecordTypeSelectionChangedDeferred.promise.then(function () {
                            outputDataRecordStorageSelectorReadyDeferred.promise.then(function () {
                                var directivePayload = {
                                    DataRecordTypeId: outputDataRecordTypeSelectorAPI.getSelectedIds()
                                };
                                if (settings != undefined) {
                                    directivePayload.selectedIds = settings.OutputDataRecordStorageId;
                                }
                                VRUIUtilsService.callDirectiveLoad(outputDataRecordStorageSelectorAPI, directivePayload, loadOutputDataRecordStorageSelectorPromiseDeferred);
                            });
                        });
                        return loadOutputDataRecordStorageSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings:
							{
							    $type: "Vanrise.GenericData.Entities.CDRCorrelationDefinitionSettings, Vanrise.GenericData.Entities",
							    InputDataRecordTypeId: (inputDataRecordTypeSelectorAPI != undefined) ? inputDataRecordTypeSelectorAPI.getSelectedIds() : null,
							    OutputDataRecordTypeId: (outputDataRecordTypeSelectorAPI != undefined) ? outputDataRecordTypeSelectorAPI.getSelectedIds() : null,
							    InputDataRecordStorageId: (inputDataRecordStorageSelectorAPI != undefined) ? inputDataRecordStorageSelectorAPI.getSelectedIds() : null,
							    OutputDataRecordStorageId: (outputDataRecordStorageSelectorAPI != undefined) ? outputDataRecordStorageSelectorAPI.getSelectedIds() : null,
							    CallingNumberFieldName: (callingNumberFieldSelectorAPI != undefined) ? callingNumberFieldSelectorAPI.getSelectedIds() : null,
							    CalledNumberFieldName: (calledNumberFieldSelectorAPI != undefined) ? calledNumberFieldSelectorAPI.getSelectedIds() : null,
							    DurationFieldName: (durationFieldSelectorAPI != undefined) ? durationFieldSelectorAPI.getSelectedIds() : null,
							    DatetimeFieldName: (datetimeFieldSelectorAPI != undefined) ? datetimeFieldSelectorAPI.getSelectedIds() : null,
							    MergeDataTransformationDefinitionId: (mergeDataTransformationAPI != undefined) ? mergeDataTransformationAPI.getSelectedIds() : null
							}
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
    app.directive('vrGenericdataCdrcorrelationdefinitionSettings', CDRCorrelationDefinitionSettings);

})(app);
