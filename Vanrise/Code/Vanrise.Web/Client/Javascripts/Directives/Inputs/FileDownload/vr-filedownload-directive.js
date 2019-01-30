'use strict';


app.directive('vrFiledownload', ['VRValidationService', 'BaseDirService', 'VRNotificationService', 'BaseAPIService', 'UtilsService', 'SecurityService', 'FileAPIService', 'VRLocalizationService', 'RemoteFileAPIService', function (VRValidationService, BaseDirService, VRNotificationService, BaseAPIService, UtilsService, SecurityService, FileAPIService, VRLocalizationService, RemoteFileAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            value: '=',
            hint: '@',
            modulename: '@',
            connectionid: '@'
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
                    ctrl.tabindex = "-1";
                }
            }, 10);



            $scope.$watch('ctrl.value', function () {
                if (ctrl.value != null) {
                    if (ctrl.value.fileName && ctrl.value.fileUniqueId == undefined && ctrl.value.fileId == undefined) {
                        ctrl.file = {
                            name: ctrl.value.fileName
                        };
                        return;
                    }
                    var connectionId = getConnectionId();
                    var input = {
                        moduleName: getModuleName(),
                    };
                    var url = "/api/VRCommon/File/";
                    if (connectionId != undefined) {
                        url = "/api/VRCommon/RemoteFile/";
                        input.connectionId = connectionId;
                    }
                    if (ctrl.value.fileUniqueId == undefined) {
                        input.fileId = ctrl.value.fileId;
                        if (connectionId == undefined) {
                            url += "GetFileInfo";
                        }
                        else {
                            url += "GetRemoteFileInfo";
                        }

                    } else {
                        input.fileUniqueId = ctrl.value.fileUniqueId;
                        url += "GetFileInfoByUniqueId";
                    }
                    BaseAPIService.get(url, input).then(function (response) {
                        if (response != null) {
                            ctrl.value.fileName = response.Name;
                            ctrl.value.fileId = response.FileId;
                            ctrl.value.fileUniqueId = response.FileUniqueId;

                            ctrl.file = {
                                name: response.Name,
                                type: response.Extension,
                                fileId: response.FileId,
                                fileUniqueId: response.FileUniqueId,
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
                if (id == undefined) {
                    if (ctrl.value.downloadFileCallBackAPI != undefined && typeof (ctrl.value.downloadFileCallBackAPI) == 'function') {
                        ctrl.value.downloadFileCallBackAPI().then(function (response) {
                            UtilsService.downloadFile(response.data, response.headers);
                        });
                    }
                }
                else {
                    var connectionId = getConnectionId();
                    if (connectionId == undefined) {
                        FileAPIService.DownloadFile(id, getModuleName()).then(function (response) {
                            UtilsService.downloadFile(response.data, response.headers);
                        });
                    }
                    else {
                        RemoteFileAPIService.DownloadRemoteFile(connectionId, id, getModuleName()).then(function (response) {
                            UtilsService.downloadFile(response.data, response.headers);
                        });
                    }
                }

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

            function getConnectionId() {
                return (ctrl.connectionid == undefined || ctrl.connectionid == null) ? null : ctrl.connectionid;
            }
        },
        controllerAs: 'ctrl',
        compile: function (element, attrs) {

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
                        }, 1);
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
                 '<div ng-mouseenter="showtd=true" ng-mouseleave="showtd=false"  >'
                  + '<vr-validator validate="ctrl.validate()" vr-input>'
                     + '<div id="mainInput" ng-model="ctrl.value" class="form-control vr-file-upload"  style="border-radius: 4px;padding: 0px;">'
                            + '<div  class="vr-file">'
                               + '<div ng-if=" ctrl.file !=null "  title="{{ctrl.file.name}}">'
                               + ' <a href="" class="vr-file-name" ng-click="ctrl.downloadFile()">{{ctrl.file.name}}</a>'
                               + '</div>'
                           + '</div>'
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