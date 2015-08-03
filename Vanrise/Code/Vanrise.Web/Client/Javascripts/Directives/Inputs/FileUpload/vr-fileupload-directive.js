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
            var ctrl = this;
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
                url: base + '/api/FileManager/UploadFile2',
                formData: function (form) { return []; },
                replaceFileInput: false,
                datatype: 'json',
                add: function (e, data) {
                    
                    var file = data.files[data.files.length - 1];
                    ctrl.file = {
                        name: file.name,
                        type: file.type,
                        size: file.size,
                        lastModifiedDate: file.lastModifiedDate
                    }
                    
                    data.submit();
                },
                progress: function (e, data) {
                    $scope.$apply(function () {
                        ctrl.num = data.loaded / data.total * 100;
                    });
                },
                change: function (e, data) {
                },
                drop: function (e, data) {
                },
                done: function (e, data) {
                    
                    ctrl.value = {
                        fileId: data.result.fileId
                    }

                },
                fail: function (e, data) {
                    //alert("Oups, une erreur  est survenue.");
                }
            });
            $scope.$watch('ctrl.value', function () {

            });

            BaseDirService.addScopeValidationMethods(ctrl, elementName, this);

        },
        controllerAs: 'ctrl',
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
                            + '<div ng-if=" ctrl.file !=null " class="vr-file">'
                               + ' <a class="vr-file-name">{{ctrl.file.name}}</a>'
                               + ' <div ng-show=" ctrl.num < 100" class="progress-bar progress-bar-success  progress-bar-striped active vr-file-process" role="progressbar" aria-valuemin="0" aria-valuemax="100" ng-style="{width: ctrl.num + \'%\'}"></div>'
                               + ' <div ng-show=" ctrl.num == 100 " class="vr-file-process" style="font-size: 10px;top: 20px;"><span>Complete</span></div>'
                            + '</div>'
                            + '<span ng-show="ctrl.file !=null" class="glyphicon glyphicon-remove hand-cursor vr-file-remove" aria-hidden="true" ng-click="ctrl.value = null;ctrl.file = null"></span>'
                      + '</div>'
                    + '</div>';


            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

            return startTemplate + labelTemplate + fileTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);