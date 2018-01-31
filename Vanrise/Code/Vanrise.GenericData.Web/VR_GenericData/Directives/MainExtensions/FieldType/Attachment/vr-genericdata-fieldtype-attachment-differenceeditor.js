'use strict';
app.directive('vrGenericdataFieldtypeAttachmentDifferenceeditor', ['UtilsService',
    function (UtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            var ctor = new booleanTypeViewerEditorCtor(ctrl, $scope);
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
        templateUrl: function (element, attrs) {
            return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Attachment/Templates/AttachmentFieldTypeDifferenceEditorTemplate.html';
        }
    };

    function booleanTypeViewerEditorCtor(ctrl, $scope) {

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.changes = [];
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var changes;
                if (payload != undefined) {
                    changes = payload.changes;
                    if (changes != undefined)
                    {
                        for (var i = 0; i < changes.length; i++)
                        {
                            var change = changes[i];
                            $scope.scopeModel.changes.push({
                                fileId: change.FileId,
                                description: change.Description
                            });
                        }
                    }
                }
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }
        }

        this.initializeController = initializeController;
    }
 }]);