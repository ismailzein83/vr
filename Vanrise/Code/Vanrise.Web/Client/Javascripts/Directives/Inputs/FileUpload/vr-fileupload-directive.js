'use strict';


app.directive('vrFileupload', ['ValidationMessagesEnum', 'BaseDirService', 'VRNotificationService', 'BaseAPIService', 'UtilsService', 'SecurityService', 'FileAPIService', function (ValidationMessagesEnum, BaseDirService, VRNotificationService, BaseAPIService ,UtilsService, SecurityService, FileAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        require: '^form',
        scope: {
            onReady: '=',
            value: '=',
            hint:'@',
            customvalidate: '&'
        },
        controller: function ($scope, $element, $attrs,$timeout) {
            var ctrl = this;
            var inputElement = $element.find('#mainInput');
            var validationOptions = {};
            if ($attrs.isrequired !== undefined)
                validationOptions.requiredValue = true;
            if ($attrs.customvalidate !== undefined)
                validationOptions.customValidation = true;
            var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, $element.find('#rootDiv'));
            var filecontrol = $element.find('#fileUpload');
            $scope.extensionList = [];
            if ($attrs.extension !== undefined) {
                if ($attrs.extension == "images")
                    $scope.extensionList = ['jpg', 'png', 'gif', 'jpeg', 'bmp'];
                else if ($attrs.extension == "reports")
                    $scope.extensionList = ['xls', 'xlsx', 'pdf', 'csv'];
                else if ($attrs.extension == "all")
                    $scope.extensionList.length = 0;
                else
                   $scope.extensionList = $attrs.extension.split(',');
            }
           
            var pathArray = location.href.split('/');
            var base = pathArray[0] + '//' + pathArray[2];
            $scope.num = 0;
            $scope.complet = false;
            $scope.broken = false;
            $scope.isUploading = false;           
            var isInternalSetValue;
            filecontrol.fileupload({
                url: base + '/api/VRFile/UploadFile',
                beforeSend: function (xhr, data) {
                    var userInfoCookie = SecurityService.getAccessCookie();
                    if (userInfoCookie != undefined) {
                        var userInfo = JSON.parse(userInfoCookie);
                        xhr.setRequestHeader('Auth-Token', userInfo.Token);
                    }
                    else {
                        xhr.setRequestHeader('Auth-Token', "");
                    }

                },
                formData: function (form) { return form },
                replaceFileInput: false,
                datatype: 'json',
                add: function (e, data) {                   
                    var nameAsTab = data.originalFiles[0].name.split(".");
                    var fileExt = nameAsTab[nameAsTab.length - 1];
                    if ($scope.extensionList.indexOf(fileExt) == -1 && $scope.extensionList.length > 0 ) {
                        data.originalFiles.length = 0;
                        VRNotificationService.showError("Invalide file type.");
                        filecontrol.val("");
                        return false;
                    }
                    else{
                        $scope.isUploading = true;
                        var file = data.files[data.files.length - 1];
                        ctrl.file = {
                            name: file.name,
                            type: file.type,
                            size: file.size,
                            lastModifiedDate: file.lastModifiedDate
                        }
                        data.submit();
                    }
                    
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
                        fileId: data.result.FileId
                    };
                    isInternalSetValue = true;        
                    $timeout(function () { $scope.complet = true }, 2000);
                    $scope.isUploading = false;
                },
                fail: function (e, data) {
                    $scope.broken = true;
                }
            });
            $scope.$watch('ctrl.value', function () {
                if(isInternalSetValue){
                    isInternalSetValue = false;
                    return;
                }
                if(ctrl.value!=null){
                    BaseAPIService.get("/api/VRFile/GetFileInfo",
                        {
                            fileId: ctrl.value.fileId
                        }
                       ).then(function (response) {
                           ctrl.file = {
                               name: response.Name,
                               type: response.Extension,
                               fileId: response.FileId,
                               lastModifiedDate: response.lastModifiedDate
                           };
                       });

                }
               
            });
            ctrl.downloadFile = function(){
                var id = ctrl.value.fileId;
                FileAPIService.DownloadFile(id)
                   .then(function (response) {
                      UtilsService.downloadFile(response.data, response.headers);
                   });
            }
            if ($attrs.hint != undefined)
                ctrl.hint = $attrs.hint;
            ctrl.getInputeStyle = function () {
                return ($attrs.hint != undefined) ? {
                    "display": "inline-block",
                    "width": "calc(100% - 15px)",
                    "margin-right": "1px"
                } : {
                    "width": "100%",
                };
            }

            ctrl.adjustTooltipPosition = function (e) {
                setTimeout(function () {
                    var self = angular.element(e.currentTarget);
                    var selfHeight = $(self).height();
                    var selfOffset = $(self).offset();
                    console.log(selfOffset);
                    var innerTooltip = self.parent().find('.tooltip-inner')[0];
                    $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 17, left: selfOffset.left - 30 });
                }, 1)
            }
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
                    ctrl.adjustTooltipPosition = function (e) {
                        setTimeout(function () {
                            var self = angular.element(e.currentTarget);
                            var selfHeight = $(self).height();
                            var selfOffset = $(self).offset();
                            var tooltip = self.parent().find('.tooltip-info')[0];
                            $(tooltip).css({ display: 'block !important' });
                            var innerTooltip = self.parent().find('.tooltip-inner')[0];
                            var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                            $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 5, left: selfOffset.left - 30 });
                            $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight, left: selfOffset.left });
                        }, 1)
                    }
                    
                    BaseDirService.addScopeValidationMethods(ctrl, elementName, formCtrl);

                }
            }
        },
        bindToController: true,
        template: function (element, attrs) {
            var startTemplate = '<div id="rootDiv" style="position: relative;">';
            var endTemplate = '</div>';

            var labelTemplate = '';
            if (attrs.label != undefined)
                labelTemplate = '<vr-label>' + attrs.label + '</vr-label>';
            var fileTemplate =
                 '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false" ng-class="isUploading == true? \'vr-disabled-div\':\'\'" >'
                     + '<div id="mainInput" ng-model="ctrl.value" class="form-control vr-file-ulpoad"  ng-style="ctrl.getInputeStyle()" style="border-radius: 4px;padding: 0px;">'
                            + '<div  class="vr-file">'
                               +'<div ng-if=" ctrl.file !=null ">'
                               + ' <a href="" class="vr-file-name" ng-click="ctrl.downloadFile() ">{{ctrl.file.name}}</a>'
                               + ' <div ng-show=" ctrl.num < 100" class="progress-bar progress-bar-success  progress-bar-striped active vr-file-process" role="progressbar" aria-valuemin="0" aria-valuemax="100" ng-style="{width: ctrl.num + \'%\'}"></div>'
                               + ' <div ng-show="complet == true " class="vr-file-process" style="font-size: 10px;top: 20px;"><span>Complete</span></div>'
                               + ' <div ng-show="broken ==true " class="vr-file-process" style="font-size: 10px;top: 20px;"><span>fail</span></div>'
                               +'</div>'
                           + '</div>'
                            + '<span ng-show="ctrl.file !=null" class="glyphicon glyphicon-remove hand-cursor vr-file-remove" aria-hidden="true" ng-click="ctrl.value = null;ctrl.file = null"></span>'
                            + '<span class="btn btn-success fileinput-button vr-file-btn">'
                                +'<i class="glyphicon glyphicon-paperclip " style="top:0px"></i>'
                                + '<input type="file" id="fileUpload">'
                            + '</span>'
                      + '</div>'
                      + '<span  ng-if="ctrl.hint!=undefined" ng-mouseenter="ctrl.adjustTooltipPosition($event)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor" style="color:#337AB7;" html="true" ng-mouseenter="ctrl.adjustTooltipPosition($event)" placement="bottom" trigger="hover" data-type="info" data-title="{{ctrl.hint}}"></span>'

                    + '</div>';


            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

            return startTemplate + labelTemplate + fileTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);