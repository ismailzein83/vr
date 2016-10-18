(function (app) {

    'use strict';

    GenericBusinessEntityFilterDirective.$inject = ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VRUIUtilsService'];

    function GenericBusinessEntityFilterDirective(VR_GenericData_DataRecordFieldAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericBusinessEntityFilter = new GenericBusinessEntityFilter($scope, ctrl, $attrs);
                genericBusinessEntityFilter.initCtrl();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Management/Filter/Templates/GenericBusinessEntityFilterTemplate.html'
        };

        function GenericBusinessEntityFilter($scope, ctrl, $attrs) {
            this.initCtrl = initCtrl;

            function initCtrl() {
                ctrl.filters = [];

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var fieldTypeConfigs;

                    var loadFiltersDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadFiltersDeferred.promise);

                    if (payload != undefined && payload.runtimeFilter != undefined) {
                        var loadFieldTypeConfigsPromise = loadFieldTypeConfigs();
                        promises.push(loadFieldTypeConfigsPromise);

                        loadFieldTypeConfigsPromise.then(function () {
                            var filterLoadPromises = [];
                            var filterFields = payload.runtimeFilter.Fields;
                         
                            for (var i = 0; i < filterFields.length; i++) {
                                var filter = getFilter(filterFields[i]);
                                if (filter != undefined)
                                {
                                    filterLoadPromises.push(filter.directiveLoadDeferred.promise);
                                    ctrl.filters.push(filter);
                                }
                            }
                            
                            UtilsService.waitMultiplePromises(filterLoadPromises).then(function () {
                                loadFiltersDeferred.resolve();
                            }).catch(function (error) {
                                loadFiltersDeferred.reject(error);
                            });
                        });
                    }
                    else {
                        loadFiltersDeferred.resolve();
                    }

                    return UtilsService.waitMultiplePromises(promises);

                    function loadFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            if (response != null) {
                                fieldTypeConfigs = [];
                                for (var i = 0; i < response.length; i++) {
                                    fieldTypeConfigs.push(response[i]);
                                }
                            }
                        });
                    }
                    function getFilter(filterField) {
                        var filter;
                        var fieldTypeConfig = UtilsService.getItemByVal(fieldTypeConfigs, filterField.FieldType.ConfigId, 'ExtensionConfigurationId');
                        if (fieldTypeConfig == undefined || fieldTypeConfig.FilterEditor == undefined) {
                            return filter;
                        }
                    
                        filter = {};
                        filter.fieldPath = filterField.FieldPath;

                        filter.directiveEditor = fieldTypeConfig.FilterEditor;
                        filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        filter.onDirectiveReady = function (api) {
                            filter.directiveAPI = api;
                            var filterDirectivePayload = { fieldTitle: filterField.FieldTitle, fieldType: filterField.FieldType };
                            VRUIUtilsService.callDirectiveLoad(api, filterDirectivePayload, filter.directiveLoadDeferred);
                        };

                        filter.isRequired = filterField.IsRequired;

                        return filter;
                    }
                };
                
                api.getData = function () {
                    var data = {};
                    var isDataEmpty = true;

                    for (var i = 0; i < ctrl.filters.length; i++) {
                        var filterData = ctrl.filters[i].directiveAPI.getData();
                        if (filterData != undefined) {
                            data[ctrl.filters[i].fieldPath] = filterData;
                            isDataEmpty = false;
                        }
                    }

                    return (!isDataEmpty) ? data : undefined;
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataGenericbusinessentityFilter', GenericBusinessEntityFilterDirective);

})(app); 