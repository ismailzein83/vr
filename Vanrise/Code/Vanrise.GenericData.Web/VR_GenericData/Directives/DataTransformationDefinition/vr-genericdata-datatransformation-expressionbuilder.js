'use strict';
app.directive('vrGenericdataDatatransformationExpressionbuilder', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', '$compile', 'VRUIUtilsService', function (VR_GenericData_DataRecordTypeAPIService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
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
        templateUrl: "/Client/Modules/VR_GenericData/Directives/DataTransformationDefinition/Templates/ExpressionBuilderTemplate.html"

    };
    function recordFieldCtor(ctrl, $scope, $attrs) {
        var mainPayload;
        function initializeController() {

            ctrl.fieldNames = [];

            ctrl.onselectionchanged = function ()
            {
               
                if (ctrl.selectedRecordName != undefined)
                {
                    ctrl.loadingFields = true;
                    mainPayload.getRecordFields(ctrl.selectedRecordName.Name).then(function (response) {
                        ctrl.fieldNames = response;
                        ctrl.loadingFields = false;
                    });
                }
                   
            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return ctrl.selectedRecordName + "." + ctrl.selectedFieldName;
            }
            api.load = function (payload) {
                mainPayload = payload;
                var selectedIds;
                if (payload != undefined) {
                    ctrl.recordNames = payload.getRecordNames();
                    if(payload.selectedIds !=undefined)
                    {
                        VRUIUtilsService.setSelectedValues(payload.selectedIds, 'Name', $attrs, ctrl);
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

