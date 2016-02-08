'use strict';
app.directive('vrGenericdataDatatransformationExpressionbuilder', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', '$compile', 'VRUIUtilsService', function (VR_GenericData_DataRecordTypeAPIService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            recordlabel: '@',
            fieldlabel: '@',
            hidelabel:'@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];
            ctrl.datasource = [];



            ctrl.datasource = [];
            var ctor = new recordFieldCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function getTemplate(attrs)
    {
        var recordlabel=" ";
        var fieldlabel=" ";
        if (attrs.hidelabel == undefined)
        {
            recordlabel = ' label="' + attrs.recordlabel + '"';
            fieldlabel = ' label="' + attrs.fieldlabel + '"';
        }
            
        var template =

        '<vr-row>'
           + '<vr-columns width="1/2row">'
             + '<vr-select selectedvalues="ctrl.selectedRecordName"'
               + 'isrequired="true" hidelabel'
                + recordlabel  
               + 'onselectionchanged="ctrl.onselectionchanged"'
               + 'hideselectedvaluessection'
               + 'entityname="' + attrs.recordlabel + '"'
               + 'datasource="ctrl.recordNames"'
               + 'datatextfield="Name"'
               + 'datavaluefield="Name" hideremoveicon></vr-select>'
            + '</vr-columns>'
           + '<vr-columns width="1/2row" vr-loader="ctrl.loadingFields">'
             + ' <vr-select selectedvalues="ctrl.selectedFieldName"'
               + 'isrequired="true" hidelabel'
              + fieldlabel 
               + 'hideselectedvaluessection'
               + 'entityname="' + attrs.fieldlabel + '"'
               + 'datasource="ctrl.fieldNames"'
               + 'datatextfield="Name"'
               + 'datavaluefield="Name" hideremoveicon></vr-select>'
           + '</vr-columns>'
       + '</vr-row>';
        return template;
    }

    function recordFieldCtor(ctrl, $scope, $attrs) {
        var mainPayload;
        function initializeController() {

            ctrl.fieldNames = [];

            ctrl.onselectionchanged = function ()
            {
                if (ctrl.selectedRecordName != undefined) {
                    ctrl.loadingFields = true;
                    mainPayload.context.getRecordFields(ctrl.selectedRecordName.Name).then(function (response) {
                        ctrl.fieldNames = response;
                        if (mainPayload.selectedRecords != undefined) {
                            ctrl.selectedFieldName = UtilsService.getItemByVal(ctrl.fieldNames, mainPayload.selectedRecords.split('.')[1], "Name");
                        }

                        ctrl.loadingFields = false;
                    });
                }
                else {
                    ctrl.selectedFieldName = undefined;
                }
            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var value;
                if (ctrl.selectedRecordName != undefined)
                    value = ctrl.selectedRecordName.Name +".";
                if (ctrl.selectedFieldName != undefined)
                    value += ctrl.selectedFieldName.Name;
                return value;
            }

            api.load = function (payload) {
                mainPayload = payload;
                var selectedRecordName;
                if (payload != undefined) {
                    ctrl.recordNames = payload.context.getRecordNames();
                    if (payload.selectedRecords != undefined)
                    {
                        ctrl.selectedRecordName = UtilsService.getItemByVal(ctrl.recordNames, payload.selectedRecords.split('.')[0],"Name");
                    }
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

