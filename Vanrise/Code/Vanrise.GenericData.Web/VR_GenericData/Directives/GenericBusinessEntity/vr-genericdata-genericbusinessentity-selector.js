(function (app) {

    'use strict';

    GenericBusinessEntitySelector.$inject = ['VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService', 'UtilsService', 'VRUIUtilsService','VR_GenericData_GenericBusinessEntityService'];

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
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function BusinessentitySelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var businessEntityDefinitionId;

            var titleFieldName;
            var idFieldName;
            var selectorAPI;
            var filter;
            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
                $scope.addNewGenericBusinessEntity = function () {
                    var onGenericBEAdded = function (genericBusinessEntity) {
                        var idField = genericBusinessEntity.FieldValues[idFieldName];
                        var selectedIds;
                        if (idField != undefined)
                        {
                            if (attrs.ismultipleselection != undefined)
                                selectedIds = [idField.Value];
                            else
                                selectedIds = idField.Value;

                        }
                        $scope.isLoadingSelector = true;

                        GetGenericBusinessEntityInfo(selectedIds).finally(function () {
                            $scope.isLoadingSelector = false;
                        });
                    };
                    VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBEAdded, businessEntityDefinitionId);
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    var selectedIds;
                    var selectIfSingleItem;
                    var promises = [];

                    if (payload != undefined) {
                        ctrl.fieldTitle = payload.fieldTitle;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        selectIfSingleItem = payload.selectIfSingleItem;
                     var getGenericBERuntimeInfoPromise = getGenericBusinessEntityRuntimeInfo();
                     promises.push(getGenericBERuntimeInfoPromise);

                     function getGenericBusinessEntityRuntimeInfo() {

                            return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBusinessEntityRuntimeInfo(businessEntityDefinitionId).then(function (response) {
                                
                                if (response != undefined) {

                                    if(attrs.ismultipleselection != undefined){
                                        if (response.SelectorPluralTitle != undefined) ctrl.fieldTitle = response.SelectorPluralTitle;
                                    } else {
                                        if (response.SelectorSingularTitle != undefined) ctrl.fieldTitle = response.SelectorSingularTitle;
                                    }
                                   // titleFieldName = response.TitleFieldName;
                                    idFieldName = response.IdFieldName;
                                }

                            });

                        }
                    }

                    var getGenericBusinessEntityInfoPromise = GetGenericBusinessEntityInfo(selectedIds, selectIfSingleItem);
                    promises.push(getGenericBusinessEntityInfoPromise);

                    return UtilsService.waitMultiplePromises(promises);

                };



                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('GenericBusinessEntityId', attrs, ctrl);
                };

                return api;
            }

            function GetGenericBusinessEntityInfo(selectedIds, selectIfSingleItem) {
                
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
                    else if (selectedIds == undefined && selectIfSingleItem)
                    {
                        selectorAPI.selectIfSingleItem();
                    }
                });

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

            return '<vr-columns colnum="{{ctrl.normalColNum}}" vr-loader="isLoadingSelector" ' + haschildcolumns + '>'
                    + '<vr-label ng-if="ctrl.hidelabel ==undefined">' + label + '</vr-label>'
                    + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="GenericBusinessEntityId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                 + ' ' + addCliked
                + '</vr-select></vr-columns>';
        }
    }

    app.directive('vrGenericdataGenericbusinessentitySelector', GenericBusinessEntitySelector);

})(app);