//(function (app) {

//    'use strict';

//    BECompatibleFieldsSelectorDirective.$inject = ['VR_GenericData_BusinessEntityDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

//    function BECompatibleFieldsSelectorDirective(VR_GenericData_BusinessEntityDefinitionAPIService, UtilsService, VRUIUtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                selectedvalues: '=',
//                onselectitem: "=",
//                ondeselectitem: "=",
//                onselectionchanged: '=',
//                ismultipleselection: "@",
//                isrequired: "=",
//                customlabel: "@",
//                hideselectedvaluessection: "@",
//                normalColNum: "@",
//                hidelabel: "@",
//                usefullcolumn: "@"
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                ctrl.datasource = [];
//                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

//                $scope.label = ctrl.customlabel;
//                if (ctrl.customlabel == "") {
//                    $scope.label = "Compatible Field";

//                    if ($attrs.ismultipleselection != undefined) {
//                        $scope.label = "Compatible Fields";
//                    }
//                }

//                var beCompatibleFieldSelector = new BECompatibleFieldSelector(ctrl, $scope, $attrs);
//                beCompatibleFieldSelector.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            template: function (element, attrs) {
//                return getDirectiveTemplate(attrs);
//            }
//        };

//        function BECompatibleFieldSelector(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;

//            var selectorAPI;

//            function initializeController() {
//                ctrl.onSelectorReady = function (api) {
//                    selectorAPI = api;
//                    defineAPI();
//                };
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

//                    var filter;
//                    var selectedIds;

//                    if (payload != undefined) {
//                        filter = payload.filter;
//                        selectedIds = payload.selectedIds;
//                    }

//                    if (filter != undefined && filter.entityDefinitionId != undefined && filter.dataRecordFieldType != undefined) {
//                        var beGetCompatibleFieldsContext = {
//                            EntityDefinitionId: filter.entityDefinitionId,
//                            CompatibleWithFieldType: filter.dataRecordFieldType
//                        };

//                        return VR_GenericData_BusinessEntityDefinitionAPIService.GetCompatibleFields(beGetCompatibleFieldsContext).then(function (response) {
//                            loadDirectivePromiseDeferred.resolve();

//                            selectorAPI.clearDataSource();

//                            if (response) {
//                                for (var i = 0; i < response.length; i++) {
//                                    ctrl.datasource.push(response[i]);
//                                }
//                            }

//                            if (selectedIds != undefined) {
//                                VRUIUtilsService.setSelectedValues(selectedIds, 'FieldName', attrs, ctrl);
//                            }
//                        });
//                    }
//                    else {
//                        loadDirectivePromiseDeferred.resolve();
//                    }

//                    return loadDirectivePromiseDeferred.promise;
//                };

//                api.getSelectedIds = function () {
//                    return VRUIUtilsService.getIdSelectedIds('FieldName', attrs, ctrl);
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }

//        function getDirectiveTemplate(attrs) {
//            var entityName = 'Compatible Field';
//            var multipleselection = '';
//            var label = '';

//            if (attrs.hidelabel == undefined) {
//                if (ctrl.customlabel == "") {
//                    label = "label ='Compatible Field'";
//                    if ($attrs.ismultipleselection != undefined) {
//                        label = "label ='Compatible Fields'";
//                    }
//                }
//                else {
//                    label = "label ='" + ctrl.customlabel + "'";
//                }
//            }

//            if (attrs.ismultipleselection != undefined) {
//                entityName = 'Compatible Fields';
//                multipleselection = 'ismultipleselection';
//            }

//            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : '';

//            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : '';

//            var haschildcolumns = '';

//            if (attrs.usefullcolumn != undefined)
//                haschildcolumns = "haschildcolumns";

//            return '<vr-columns colnum="{{ctrl.normalColNum}}" ' + haschildcolumns + ' >'
//                + '<vr-select on-ready="ctrl.onSelectorReady"'
//                + ' datasource="ctrl.datasource"'
//                + ' selectedvalues="ctrl.selectedvalues"'
//                + ' onselectionchanged="ctrl.onselectionchanged"'
//                + ' onselectitem="ctrl.onselectitem"'
//                + ' ondeselectitem="ctrl.ondeselectitem"'
//                + ' datavaluefield="FieldName"'
//                + ' datatextfield="FieldTitle"'
//                + ' ' + label + ' '
//                + ' ' + multipleselection
//                + ' ' + hideselectedvaluessection
//                + ' isrequired="ctrl.isrequired"'
//                + ' ' + hideremoveicon
//                + ' entityName="' + entityName + '">'
//                + '</vr-select>'
//                + '</vr-columns>';
//        }
//    }

//    app.directive('vrGenericdataBusinessentitydefinitionCompatiblefieldsSelector', BECompatibleFieldsSelectorDirective);

//})(app);