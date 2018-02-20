(function (app) {

    'use strict';

    FilterDefinitionFilterGroupDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function FilterDefinitionFilterGroupDirective(UtilsService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FilterGroupCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/FilterGroupFilterDefinitionTemplate.html'
        };

        function FilterGroupCtor($scope, ctrl) {

            var context;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {                   
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.FilterGroupFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        AvailableFieldNames: dataRecordTypeFieldsSelectorAPI.getSelectedIds()
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                    }

                    promises.push(loadDataRecordTitleFieldsSelector());


                    function loadDataRecordTitleFieldsSelector() {
                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var namesFieldsPayload = {
                                dataRecordTypeId: context.getDataRecordTypeId(),
                            };
                            if (payload.settings != undefined && payload.settings.AvailableFieldNames != undefined) {
                                namesFieldsPayload.selectedIds = payload.settings.AvailableFieldNames;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, namesFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


               

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            
            function getContext() {
                return context;
            }

           
        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionFiltergroup', FilterDefinitionFilterGroupDirective);

})(app);