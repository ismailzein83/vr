'use strict';


app.directive('vrFileupload', ['VRValidationService', 'BaseDirService', 'VRNotificationService', 'BaseAPIService', 'UtilsService', 'SecurityService', 'FileAPIService','VRLocalizationService', function (VRValidationService, BaseDirService, VRNotificationService, BaseAPIService, UtilsService, SecurityService, FileAPIService, VRLocalizationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            value: '=',
            hint: '@',
            modulename: '@',
            tempfile: '@',
            validationfunction: '='
        },
        controller: function ($scope, $element, $attrs, $timeout) {
            $scope.$on("$destroy", function () {
                $element.off();
                $(window).off("resize.Viewport");
            });
            var ctrl = this;
            ctrl.readOnly = UtilsService.isContextReadOnly($scope) || $attrs.readonly != undefined;
            ctrl.validate = function () {
                return VRValidationService.validate(ctrl.value, $scope, $attrs);
            };
            ctrl.tabindex = "";
            setTimeout(function () {
                if ($($element).hasClass('divDisabled') || $($element).parents('.divDisabled').length > 0) {
                    ctrl.tabindex = "-1"
                }
            }, 10);


            //var inputElement = $element.find('#mainInput');
            //var validationOptions = {};
            //if ($attrs.isrequired !== undefined)
            //    validationOptions.requiredValue = true;
            //if ($attrs.customvalidate !== undefined)
            //    validationOptions.customValidation = true;
            //var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, $element.find('#rootDiv'));
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
                    $scope.extensionList = $attrs.extension.toLowerCase().split(',');
            }
           
            var pathArray = location.href.split('/');
            var base = pathArray[0] + '//' + pathArray[2];
            $scope.num = 0;
            $scope.complet = false;
            $scope.broken = false;
            $scope.isUploading = false;           
            var isInternalSetValue;
            filecontrol.fileupload({
                url: base + '/api/VRCommon/File/UploadFile',
                beforeSend: function (xhr, data) {
                    xhr.setRequestHeader('Auth-Token', SecurityService.getUserToken());
                    var moduleName = getModuleName();
                    if (moduleName != null) {
                        xhr.setRequestHeader('Module-Name', moduleName);
                    }                   
                    if (ctrl.tempfile != undefined && (ctrl.tempfile == "true" || ctrl.tempfile == true))
                        xhr.setRequestHeader('Temp-File', "true");
                },
                formData: function (form) { return form },
                replaceFileInput: true,
                datatype: 'json',
                add: function (e, data) {
                    if (ctrl.readOnly)
                        return;
                    var nameAsTab = data.originalFiles[0].name.split(".");
                    var fileExt = nameAsTab[nameAsTab.length - 1];
                    if ($scope.extensionList.indexOf(fileExt.toLowerCase()) == -1 && $scope.extensionList.length > 0) {
                        data.originalFiles.length = 0;
                        VRNotificationService.showError("Invalide file type.");
                        filecontrol.val("");
                        return false;
                    }
                    else {
                        var file = data.files[data.files.length - 1];
                        
                        if (ctrl.validationfunction != undefined) {
                            if (ctrl.validationfunction(file.name, file.size)) {
                                sumbitData();
                            }
                        }
                        else {
                            sumbitData();
                        }
                    }
                    
                    function sumbitData() {
                        $scope.isUploading = true;
                        ctrl.file = {
                            name: file.name,
                            type: file.type,
                            size: file.size,
                            lastModifiedDate: file.lastModifiedDate
                        };
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
                    if (data == undefined || data.result == undefined || data.result.FileId == undefined) {
                        $scope.broken = true;
                        $scope.isUploading = false;
                    }
                    else {
                        ctrl.value = {
                            fileId: data.result.FileId,
                            fileName: data.result.Name
                        };
                        var obj = ctrl.value;
                        isInternalSetValue = true;
                        $timeout(function () { $scope.complet = true }, 2000);
                        $scope.isUploading = false;
                        data.originalFiles.length = 0;
                        if ($attrs.onvaluechanged != undefined) {
                            var onvaluechangedMethod = $scope.$parent.$eval($attrs.onvaluechanged);
                            if (onvaluechangedMethod != undefined && typeof (onvaluechangedMethod) == 'function') {
                                onvaluechangedMethod(obj);
                            }
                        }
                    }
                },
                fail: function (e, data) {
                    $scope.broken = true;
                    $scope.isUploading = false;

                }
            });
            $scope.$watch('ctrl.value', function () {

                if (isInternalSetValue) {
                    isInternalSetValue = false;
                    return;
                }
                if (ctrl.value != null) {
                    BaseAPIService.get("/api/VRCommon/File/GetFileInfo", {
                        fileId: ctrl.value.fileId,
                        moduleName: getModuleName()
                    }).then(function (response) {
                        if (response != null) {
                            ctrl.value.fileName = response.Name;
                            ctrl.file = {
                                name: response.Name,
                                type: response.Extension,
                                fileId: response.FileId,
                                lastModifiedDate: response.lastModifiedDate
                            };                            
                        }
                        else
                            ctrl.file = {};
                    });

                }
                else {
                    ctrl.file = null;
                }
                if ($attrs.onvaluechanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval($attrs.onvaluechanged);
                    if (onvaluechangedMethod != undefined && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }
            });
            ctrl.downloadFile = function () {
                var id = ctrl.value.fileId;
                FileAPIService.DownloadFile(id, getModuleName())
                   .then(function (response) {
                       UtilsService.downloadFile(response.data, response.headers);
                   });
            };
            if ($attrs.hint != undefined)
                ctrl.hint = $attrs.hint;

            var getInputeStyle = function () {
                var div = $element.find('div[validator-section]')[0];
                if ($attrs.hint != undefined) {
                    $(div).css({ "display": "inline-block", "width": "calc(100% - 15px)", "margin-right": "1px" });
                }
            };
            getInputeStyle();

            ctrl.adjustTooltipPosition = function (e) {
                setTimeout(function () {
                    var self = angular.element(e.currentTarget);
                    var selfHeight = $(self).height();
                    var selfOffset = $(self).offset();
                    var tooltip = self.parent().find('.tooltip-info')[0];
                    $(tooltip).css({ display: 'block' });
                    var innerTooltip = self.parent().find('.tooltip-inner')[0];
                    var innerTooltipArrow = self.parent().find('.tooltip-arrow')[0];
                    var innerTooltipWidth = parseFloat(($(innerTooltip).width() / 2) + 2.5);
                    $(innerTooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 15, left: selfOffset.left - innerTooltipWidth });
                    $(innerTooltipArrow).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + selfHeight + 10, left: selfOffset.left });

                }, 1);
            };

            function getModuleName() {
                return (ctrl.modulename == undefined || ctrl.modulename == null) ? null : ctrl.modulename;
            }
        },
        controllerAs: 'ctrl',
        compile: function (element, attrs) {

            //var inputElement = element.find('#mainInput');
            //var validationOptions = {};
            //if (attrs.isrequired !== undefined)
            //    validationOptions.requiredValue = true;
            //if (attrs.customvalidate !== undefined)
            //    validationOptions.customValidation = true;

            //var elementName = BaseDirService.prepareDirectiveHTMLForValidation(validationOptions, inputElement, inputElement, element.find('#rootDiv'));
            return {
                pre: function ($scope, iElem, iAttrs) {


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
                    };
                    ctrl.remove = function () {
                        $scope.complet = false;
                        $scope.broken = false;
                        $scope.isUploading = false;
                        ctrl.value = null;
                        ctrl.file = null;
                    };
                    var api = {};
                    api.clearFileUploader = ctrl.remove;

                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                    //BaseDirService.addScopeValidationMethods(ctrl, elementName, formCtrl);

                }
            };
        },
        bindToController: true,
        template: function (element, attrs) {
            var startTemplate = '<div id="rootDiv" style="position: relative;">';
            var endTemplate = '</div>';

            var labelTemplate = '';
            var label = VRLocalizationService.getResourceValue(attrs.localizedlabel, attrs.label);

            if (label != undefined)
                labelTemplate = '<vr-label>' + label + '</vr-label>';
            var fileTemplate =
                 '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false" ng-class="isUploading == true? \'vr-disabled-div\':\'\'" >'
                  + '<vr-validator validate="ctrl.validate()" vr-input>'
                     + '<div id="mainInput" ng-model="ctrl.value" class="form-control vr-file-upload"  style="border-radius: 4px;padding: 0px;">'
                            + '<div  class="vr-file">'
                               +'<div ng-if=" ctrl.file !=null ">'
                               + ' <a href="" class="vr-file-name" ng-click="ctrl.downloadFile()">{{ctrl.file.name}}</a>'
                               + ' <div ng-show=" ctrl.num < 100" class="progress-bar progress-bar-success  progress-bar-striped active vr-file-process" role="progressbar" aria-valuemin="0" aria-valuemax="100" ng-style="{width: ctrl.num + \'%\'}"></div>'
                               + ' <div ng-show="complet == true " class="vr-file-process" ><span>Complete</span></div>'
                               + ' <div ng-show="broken ==true " class="vr-file-process" style="font-size: 10px;top: 20px;"><span>fail</span></div>'
                               +'</div>'
                           + '</div>'
                            + '<span ng-show="ctrl.file !=null || broken ==true" ng-if="!ctrl.readOnly"  class="glyphicon glyphicon-remove hand-cursor vr-file-remove" aria-hidden="true" ng-click="ctrl.remove()"></span>'
                            + '<span vr-disabled="ctrl.file !=null" ng-show="!ctrl.readOnly" class="btn btn-success fileinput-button vr-file-btn">'
                                + '<i class="glyphicon glyphicon-paperclip  vr-file-upload-paperclip"></i>'
                                + '<input  type="file" tabindex="{{ctrl.tabindex}}" id="fileUpload"  title=" ">'
                            + '</span>'
                      + '</div>'
                  + '</vr-validator>'
                  + '<span  ng-if="ctrl.hint!=undefined" ng-mouseenter="ctrl.adjustTooltipPosition($event)" bs-tooltip class="glyphicon glyphicon-question-sign hand-cursor vr-hint-input"  html="true" ng-mouseenter="ctrl.adjustTooltipPosition($event)" placement="bottom" trigger="hover" data-type="info" data-title="{{ctrl.hint}}"></span>'
            + '</div>';

            var validationTemplate = BaseDirService.getValidationMessageTemplate(true, false, true, true, true, true, attrs.label != undefined);

            return startTemplate + labelTemplate + fileTemplate + validationTemplate + endTemplate;
        }

    };

    return directiveDefinitionObject;

}]);