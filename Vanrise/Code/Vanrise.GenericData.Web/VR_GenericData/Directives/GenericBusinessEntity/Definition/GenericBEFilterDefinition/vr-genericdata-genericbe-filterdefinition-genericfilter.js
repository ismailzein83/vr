(function (app) {

    'use strict';

    FilterDefinitionGenericFilterDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function FilterDefinitionGenericFilterDirective(UtilsService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericFilterCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/GenericFilterDefinitionTemplate.html'
        };

        function GenericFilterCtor($scope, ctrl) {

            var context;
            var firstLoad = true;
            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeFieldsSelectionChanged = function () {
                    if (firstLoad) return;
                    if (dataRecordTypeFieldsSelectorAPI.getSelectedValue() != undefined) {
                        $scope.scopeModel.fieldTitle = dataRecordTypeFieldsSelectorAPI.getSelectedValue().Title;
                    }
                    else {
                        $scope.scopeModel.fieldTitle = undefined;
                    }

                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        FieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                        FieldTitle: $scope.scopeModel.fieldTitle,
                        IsRequired: $scope.scopeModel.isRequired
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                    }
                    function loadDataRecordTitleFieldsSelector() {
                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var fieldsPayload = {
                                dataRecordTypeId: context.getDataRecordTypeId(),
                            };
                            if (payload.settings != undefined && payload.settings.FieldName != undefined) {
                                fieldsPayload.selectedIds = payload.settings.FieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                    }

                    function loadStaticData() {
                        if (payload != undefined && payload.settings != undefined) {
                            $scope.scopeModel.fieldTitle = payload.settings.FieldTitle;
                            $scope.scopeModel.isRequired = payload.settings.IsRequired;
                        }

                    }

                    return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadDataRecordTitleFieldsSelector]).then(function () {
                    }).finally(function () {
                        setTimeout(function () {
                            firstLoad = false;
                        },1);
                    });
                };




                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function getContext() {
                return context;
            }


        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionGenericfilter', FilterDefinitionGenericFilterDirective);

})(app);