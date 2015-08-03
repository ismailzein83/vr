'use strict';


app.directive('vrFileupload', ['ValidationMessagesEnum', 'BaseDirService', function (ValidationMessagesEnum, BaseDirService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        require: '^form',
        scope: {
            value: '=',
            customvalidate: '&'
        },
        controller: function ($scope, $element, $attrs) {
            var inputElement = $element.find('#mainInput');
            var validationOptions = {};
            if ($attrs.isrequired !== undefined)
                validationOptions.requiredValue = true;
            if ($attrs.customvalidate !== undefined)
                validationOptions.customValidation = true;
            var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, $element.find('#rootDiv'));
            var filecontrol = $element.find('#fileUpload');
            var pathArray = location.href.split('/');
            var base = pathArray[0] + '//' + pathArray[2];
            $scope.num = 0;
            filecontrol.fileupload({
                url: base + '/api/FileManager/UploadFile',
                formData: function (form) { return []; },
                replaceFileInput: false,
                datatype: 'json',
                add: function (e, data) {
                    $scope.ctrl.value = data.files[data.files.length - 1];
                    console.log($scope.ctrl.value);
                    data.submit();
                },
                progress: function (e, data) {
                    $scope.$apply(function () {
                        $scope.ctrl.num = data.loaded / data.total * 100;
                    });
                },
                change: function (e, data) {
                },
                drop: function (e, data) {
                },
                done: function (e, data) {
                },
                fail: function (e, data) {
                    //alert("Oups, une erreur  est survenue.");
                }
            });

            var ctrl = $scope.ctrl;
            $scope.$watch('ctrl.value', function () {

            });

            BaseDirService.addScopeValidationMethods(ctrl, elementName, this);

        },
        compile: function (element, attrs) {

            var inputElement = element.find('#mainInput');
            var validationOptions = {};
            if (attrs.isrequired !== undefined)
                validationOptions.requiredValue = true;
            if (attrs.customvalidate !== undefined)
                validationOptions.customValidation = true;

            var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
            return {
                pre: function ($scope, iElem, iAttrs, formCtrl) {


                    var ctrl = $scope.ctrl;


                    BaseDirService.addScopeValidationMethods(ctrl, elementName, formCtrl);

                }
            }
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            var startTemplate = '<div id="rootDiv" style="position: relative;">';
            var endTemplate = '</div>';

            var labelTemplate = '';
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var fileTemplate =
                 '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false"  >'
                     + '<div id="mainInput" ng-model="ctrl.value" class="form-control vr-file-ulpoad" style="border-radius: 4px;padding: 0px;">'
                            + '<span class="btn btn-success fileinput-button vr-file-btn">'
                                +'<i class="glyphicon glyphicon-plus " style="top:-1px"></i>'
                                + '<input type="file" id="fileUpload">'
                            + '</span>'
                            + '<div ng-if=" ctrl.value !=null " class="vr-file">'
                               + ' <a class="vr-file-name">{{ctrl.value.name}}</a>'
                               + ' <div ng-show=" ctrl.num < 100" class="progress-bar progress-bar-success  progress-bar-striped active vr-file-process" role="progressbar" aria-valuemin="0" aria-valuemax="100" ng-style="{width: ctrl.num + \'%\'}"></div>'
                               + ' <div ng-show=" ctrl.num == 100 " class="vr-file-process" style="font-size: 10px;top: 20px;"><span>Complete</span></div>'
                            + '</div>'
                            + '<span ng-show="ctrl.value !=null" class="glyphicon glyphicon-remove hand-cursor vr-file-remove" aria-hidden="true" ng-click="ctrl.value = null"></span>'
                      + '</div>'
                    + '</div>';


            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

            return startTemplate + labelTemplate + fileTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);