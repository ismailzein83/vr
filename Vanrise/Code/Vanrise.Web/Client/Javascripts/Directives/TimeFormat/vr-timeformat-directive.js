'use strict';
app.directive('vrTimeformat', ['UtilsService', '$compile', 'VRModalService', function (UtilsService, $compile, VRModalService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            label: '@',
            hidelabel: '@',
            isrequired: '=',
            value: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new timeFormatCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    var ctrl = $scope.ctrl;
                    $scope.$watch('ctrl.value', function (newValue, oldValue) {

                        var retrunedValue;
                        if (newValue == "") {
                            setTimeout(function () {
                                ctrl.value = undefined;
                                $scope.$apply();
                            });
                        }
                        if (!newValue == "") {
                            retrunedValue = newValue;
                        }
                        if (iAttrs.onvaluechanged != undefined) {
                            var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onvaluechanged);
                            if (onvaluechangedMethod != undefined && typeof (onvaluechangedMethod) == 'function') {
                                onvaluechangedMethod(retrunedValue);
                            }
                        }

                    });
                }
            }
        },
        template: function (element, attrs) {
            return getTamplate(attrs);
        }
    };


    function getTamplate(attrs) {
        var withemptyline = 'withemptyline';
        if (attrs.hidelabel != undefined)
            withemptyline = '';
        var template =
            '<vr-row removeline>'
             + '<vr-columns width="3/4row">'
             + '<vr-label ng-if="ctrl.hidelabel ==undefined">{{ctrl.label}}</vr-label>'
             + '<vr-textbox value="ctrl.value" isrequired="ctrl.isrequired"></vr-textbox>'
             + '</vr-columns>'
             + '<vr-columns width="1/4row" ' + withemptyline + ' > '
             + '   <vr-button type="Help" data-onclick="openTimeFormatBuilder" standalone></vr-button>'
             + '</vr-columns>'
            + '</vr-row>';
        return template;

    }
    function timeFormatCtor(ctrl, $scope, $attrs) {
        function initializeController() {


            $scope.openTimeFormatBuilder = function () {
                var modalSettings = {};

                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.onSetTimeFormatBuilder = function (timeFormatBuilderValue) {
                        ctrl.value = timeFormatBuilderValue;
                    };
                };
                var parameter;
                if (ctrl.value != undefined)
                {
                     parameter = {
                        timeFormatValue: ctrl.value
                    };
                }
                   
                VRModalService.showModal('/Client/Javascripts/Directives/TimeFormat/Templates/TimeFormatEditor.html', parameter, modalSettings);
            }
            
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

