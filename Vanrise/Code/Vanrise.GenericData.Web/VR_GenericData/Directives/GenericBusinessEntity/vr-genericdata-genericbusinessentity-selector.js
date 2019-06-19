(function (app) {

    'use strict';

    GenericBusinessEntitySelector.$inject = ['VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityService'];

    function GenericBusinessEntitySelector(VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBusinessEntityAPIService, UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                customlabel: "@",
                normalColNum: '@',
                hidelabel: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var businessentitySelector = new BusinessentitySelector(ctrl, $scope, $attrs);
                businessentitySelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function BusinessentitySelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var businessEntityDefinitionId;
            var titleFieldName;
            var idFieldName;
            var filter;

            var isRemote;
            var isFirstLoad = true;
            var hasEmtyRequiredDependentField;

            var selectorAPI;


            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.addNewGenericBusinessEntity = function () {
                    var onGenericBEAdded = function (genericBusinessEntity) {
                        var idField = genericBusinessEntity.FieldValues[idFieldName];
                        var selectedIds;
                        if (idField != undefined) {
                            if (attrs.ismultipleselection != undefined)
                                selectedIds = [idField.Value];
                            else
                                selectedIds = idField.Value;

                        }
                        $scope.isLoadingSelector = true;

                        getGenericBusinessEntityInfo(selectedIds).finally(function () {
                            $scope.isLoadingSelector = false;
                        });
                    };
                    VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBEAdded, businessEntityDefinitionId);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var selectedIds;
                    var selectIfSingleItem;
                    var beRuntimeSelectorFilter;

                    if (payload != undefined) {
                        ctrl.fieldTitle = payload.fieldTitle;
                        ctrl.isDisabled = payload.isDisabled;

                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        filter = payload.filter;
                        beRuntimeSelectorFilter = payload.beRuntimeSelectorFilter;

                        selectedIds = payload.selectedIds;
                        selectIfSingleItem = payload.selectIfSingleItem;
                        hasEmtyRequiredDependentField = payload.hasEmtyRequiredDependentField;

                        var getGenericBERuntimeInfoPromise = getGenericBusinessEntityRuntimeInfo();
                        promises.push(getGenericBERuntimeInfoPromise);
                    }

                    //if (beRuntimeSelectorFilter != undefined) {
                    //    if (filter == undefined)
                    //        filter = { GenericBESelectorCondition: beRuntimeSelectorFilter.GenericBESelectorCondition };
                    //}

                    if (!hasEmtyRequiredDependentField) {
                        var getGenericBusinessEntityInfoPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(getGenericBusinessEntityInfoPromiseDeferred.promise);

                        getGenericBERuntimeInfoPromise.then(function () {
                            if (isRemote) {
                                if (selectedIds != undefined) {
                                    if (isFirstLoad) {
                                        if (!Array.isArray(selectedIds))
                                            selectedIds = [selectedIds];

                                        if (filter == undefined) {
                                            filter = { SelectedIds: selectedIds };
                                        } else {
                                            filter.SelectedIds = selectedIds;
                                        }
                                    }

                                    VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter)).then(function (response) {
                                        selectorAPI.clearDataSource();
                                        ctrl.datasource.length = 0;

                                        angular.forEach(response, function (item) {
                                            ctrl.datasource.push(item);
                                        });

                                        VRUIUtilsService.setSelectedValues(selectedIds, 'GenericBusinessEntityId', attrs, ctrl);
                                        getGenericBusinessEntityInfoPromiseDeferred.resolve();
                                    });
                                }
                                else {
                                    if (!isFirstLoad)
                                        selectorAPI.clearDataSource();

                                    getGenericBusinessEntityInfoPromiseDeferred.resolve();
                                }
                            }
                            else {
                                getGenericBusinessEntityInfo(selectedIds, selectIfSingleItem).then(function () {
                                    getGenericBusinessEntityInfoPromiseDeferred.resolve();
                                });
                            }
                        });
                    }

                    function getGenericBusinessEntityRuntimeInfo() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBusinessEntityRuntimeInfo(businessEntityDefinitionId).then(function (response) {
                            if (response != undefined) {
                                if (attrs.ismultipleselection != undefined) {
                                    if (response.SelectorPluralTitle != undefined) {
                                        ctrl.fieldTitle = response.SelectorPluralTitle;
                                    }
                                }
                                else {
                                    if (response.SelectorSingularTitle != undefined) {
                                        ctrl.fieldTitle = response.SelectorSingularTitle;
                                    }
                                }

                                titleFieldName = response.TitleFieldName;
                                idFieldName = response.IdFieldName;
                                isRemote = response.IsRemote;

                                if (isRemote && !hasEmtyRequiredDependentField)
                                    ctrl.searchGenericBE = searchGenericBE;
                                else
                                    ctrl.searchGenericBE = ctrl.datasource;
                            }
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('GenericBusinessEntityId', attrs, ctrl);
                };

                api.clearDataSource = function () {
                    ctrl.searchGenericBE = [];
                    selectorAPI.clearDataSource();
                };

                api.setFieldValues = function (value, context) {
                    var promises = [];

                    $scope.isDisabled = context.isDisabled;
                    filter = context.filter;

                    if (isRemote) {
                        var getGenericBusinessEntityInfoPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(getGenericBusinessEntityInfoPromiseDeferred.promise);

                        var resetFilter = false;
                        var selectedIds = !Array.isArray(value) ? [value] : value;
                        if (filter == undefined) {
                            filter = { SelectedIds: [selectedIds] };
                            resetFilter = true;
                        } else {
                            filter.SelectedIds = [selectedIds];
                            resetFilter = true;
                        }

                        VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter)).then(function (response) {
                            selectorAPI.clearDataSource();
                            ctrl.datasource.length = 0;

                            if (resetFilter) {
                                filter = undefined;
                            }

                            angular.forEach(response, function (item) {
                                ctrl.datasource.push(item);
                            });

                            VRUIUtilsService.setSelectedValues(value, 'GenericBusinessEntityId', attrs, ctrl);
                            getGenericBusinessEntityInfoPromiseDeferred.resolve();
                        });
                    }
                    else {
                        promises.push(getGenericBusinessEntityInfo(value));
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getGenericBusinessEntityInfo(selectedIds, selectIfSingleItem) {
                return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter)).then(function (response) {
                    selectorAPI.clearDataSource();

                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }
                    }

                    if (selectedIds) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'GenericBusinessEntityId', attrs, ctrl);
                    }
                    else if (selectedIds == undefined && selectIfSingleItem) {
                        selectorAPI.selectIfSingleItem();
                    }
                });
            }

            function searchGenericBE(searchValue) {
                if (filter != undefined && filter.SelectedIds != undefined)
                    filter.SelectedIds = undefined;

                return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter), searchValue);
            }
        }

        function getDirectiveTemplate(attrs) {
            var multipleselection = '';

            var label = '{{ctrl.fieldTitle}}';
            if (attrs.ismultipleselection != undefined) {
                label = '{{ctrl.fieldTitle}}';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hidelabel = "";
            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";
            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewGenericBusinessEntity"';

            var haschildcolumns = "";
            if (attrs.usefullcolumn != undefined)
                haschildcolumns = "haschildcolumns";

            //var requiredParentTemplate = '<span ng-if="parentSelector != undefined && parentSelector.parentDirective != undefined">'
            //    + '<vr-columns colnum="{{ctrl.normalColNum}}" ' + haschildcolumns + '>'
            //    + '<vr-directivewrapper directive="parentSelector.parentDirective" normal-col-num="{{scopeModel.calculatedColNum}}"  on-ready="parentSelector.onParentDirectiveReady" '
            //    + ' onselectionchanged="parentSelector.onParentSelectionChanged" isrequired = "runtimeEditorCtrl.isrequired" ></vr-directivewrapper > '
            //    + '</vr-columns>'
            //    + '</span>';

            return '<vr-columns colnum="{{ctrl.normalColNum}}" vr-loader="isLoadingSelector" ' + haschildcolumns + '>'
                + '<span vr-disabled="ctrl.isDisabled">'
                + '<vr-label ng-if="ctrl.hidelabel ==undefined">' + label + '</vr-label>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                + ' datasource="ctrl.searchGenericBE"'
                + ' selectedvalues="ctrl.selectedvalues"'
                + ' onselectionchanged="ctrl.onselectionchanged"'
                + ' onselectitem="ctrl.onselectitem"'
                + ' ondeselectitem="ctrl.ondeselectitem"'
                + ' datavaluefield="GenericBusinessEntityId"'
                + ' datatextfield="Name"'
                + ' ' + multipleselection
                + ' ' + hideselectedvaluessection
                + ' isrequired="ctrl.isrequired"  '
                + ' ' + hideremoveicon
                + ' ' + addCliked
                + ' limitcharactercount="ctrl.limitcharactercount">'
                + '</vr-select></span></vr-columns>';
        }
    }

    app.directive('vrGenericdataGenericbusinessentitySelector', GenericBusinessEntitySelector);
})(app);