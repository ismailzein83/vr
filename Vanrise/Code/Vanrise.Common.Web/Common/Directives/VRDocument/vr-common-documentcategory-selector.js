'use strict';
app.directive('vrCommonDocumentcategorySelector', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            selectedvalues: '=',
            isrequired: "=",
            onselectitem: "=",
            ondeselectitem: "=",
            hideremoveicon: '@',
            normalColNum: '@',
            label:'@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var ctor = new DocumentCategoryCtor(ctrl, $scope, $attrs);
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
        template: function (element, attrs) {
            return getTimeZoneTemplate(attrs);
        }

    };

    function getTimeZoneTemplate(attrs) {

        var multipleselection = "";
       
        return '<vr-select ' + multipleselection + '  datatextfield="Title" datavaluefield="ItemId" isrequired="ctrl.isrequired"'
            +  '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"   onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select></vr-columns>';
    }

    function DocumentCategoryCtor(ctrl, $scope, attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            var documentCategories;

            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    documentCategories = payload.documentCategories;
                    selectedIds = payload.selectedIds;
                }

                if (documentCategories != undefined) {
                    for (var i = 0; i < documentCategories.length; i++)
                       ctrl.datasource.push(documentCategories[i])
                }

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'ItemId', attrs, ctrl);
                }
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('ItemId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);