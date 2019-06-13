'use strict';

app.directive('vrGenericdataDatarecordtypextrafieldsMultipleparentrecordtype', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new extraFieldCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/RecordType/Templates/MultipleParentDataRecordTypeExtraFieldsTemplate.html'
        };


        function extraFieldCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};


            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];


                };


                api.getData = function () {

                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);