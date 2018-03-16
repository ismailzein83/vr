"use strict";

app.directive("vrCommonExcelfileparser", ["UtilsService", "VRNotificationService", "VRModalService",
function (UtilsService, VRNotificationService, VRModalService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            type: '@',
            fieldname: '@',
            onokclicked: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var excelfileParserCtor = new ExcelFileParser($scope, ctrl, $attrs);
            excelfileParserCtor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        template: function (element, attrs) {
            var standalone = '';
            if (attrs.standalone != undefined)
                standalone = 'standalone';
            return '<vr-button type="UploadExcel" data-onclick="openExcelFileParserEditor"  ' + standalone + ' ></vr-button>'
        }

    };

    function ExcelFileParser($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            $scope.openExcelFileParserEditor = function () {
                var settings = {

                };
                settings.onScopeReady = function (modalScope) {
                    modalScope.onOkPressed = function (dataValues) {
                        if (ctrl.onokclicked != undefined && typeof (ctrl.onokclicked) == 'function')
                            ctrl.onokclicked(dataValues);
                    };
                };
                var parameters = {
                    type: ctrl.type == 'number' ? 1 : 0,
                    fieldName: ctrl.fieldname
                };
                VRModalService.showModal('/Client/Modules/Common/Directives/ExcelFileParser/Templates/ExcelFileParserEditor.html', parameters, settings);
            };
        }
    }

    return directiveDefinitionObject;

}]);