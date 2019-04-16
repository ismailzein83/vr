'use strict';

app.directive('vrGenericdataDatarecordtypeextrafieldsParentrecordtype', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/RecordType/Templates/ParentDataRecordTypeExtraFieldsTemplate.html'
        };


        function extraFieldCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};

            var selectedDataRecordType;

            var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var datarecordTypeSelectorDirectiveApi;

            function initializeController() {

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    datarecordTypeSelectorDirectiveApi = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };
               
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined)
                        selectedDataRecordType = payload.DataRecordTypeId;

                    var dataRecordTypeSelectorLoadedPromise = loadDataRecordTypeSelector();
                    promises.push(dataRecordTypeSelectorLoadedPromise);

                    function loadDataRecordTypeSelector() {
                        var dataRecordTypeSelectorLoadedPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {

                            var dataRecordTypeSelectorPayload;

                            if (selectedDataRecordType != undefined)
                                dataRecordTypeSelectorPayload = { selectedIds: selectedDataRecordType };

                            VRUIUtilsService.callDirectiveLoad(datarecordTypeSelectorDirectiveApi, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadedPromiseDeferred);
                        });
                        return dataRecordTypeSelectorLoadedPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.MainExtensions.ParentDataRecordTypeExtraFields, Vanrise.GenericData.MainExtensions',
                        DataRecordTypeId: datarecordTypeSelectorDirectiveApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null && typeof(ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);