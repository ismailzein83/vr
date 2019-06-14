'use strict';

app.service('ButtonDirService', ['BaseDirService', 'VRLocalizationService', function (BaseDirService, VRLocalizationService) {

    return ({
        getTemplate: getTemplate,
        getButtonAttributes: getButtonAttributes
    });

    function getTemplate(attrs) {

        var actionsMenuTemplate = ''
            + '<ul role="menu" class="dropdown-menu gid-cell-menu am-fade-and-slide-top" ng-show="ctrl.showMenuActions" ng-style="{\'position\': \'fixed\', \'top\': \'initial\' , \'left\': \'initial\' , \'min-width\': \'120px\'} " >'
            + ' <li role="presentation">'
            + '     <div ng-repeat="action in ctrl.menuActions"  ng-hide="action.disable" class="btn-menu-item mark-select ">'
            + '       <div class=" hand-cursor" ng-click="ctrl.menuActionClicked(action)"><span style="font-size:11px">{{action.name}}</span>'
            + '<img src="../../Client/Javascripts/Directives/Button/images/loader-mask.gif" class="img-loader" style="width:14px;" ng-show="action.isSubmitting" /></div>'
            + '    </div>'
            + ' </li>'
            + '   </ul>';

        var type = attrs.type;

        var customlabel = attrs.customlabel;
        var customLabelTag = (customlabel != undefined && customlabel != "") ? '<span class="btn-label" >' + customlabel + '</span>' : '';
        var buttonAttributes = getButtonAttributes(type);
        var buttonAtributesText = VRLocalizationService.getResourceValue(buttonAttributes.localizedText, buttonAttributes.text);
        var buttonTitle = attrs.buttontitle != undefined ? attrs.buttontitle : buttonAtributesText;
        if (type == "Login") {
            return '<div style="position:relative;display:inline-block;width:100%" ng-mouseleave="ctrl.showMenuActions = false" ng-hide="ctrl.hideTemplate">'
                + '<button style="width:100%" type="button" class="btn btn-danger login-btn"'
                + 'aria-label="Left Align" ng-click="ctrl.onInternalClick($event)" ng-disabled="ctrl.isDisabled()">' + buttonAtributesText
                + '<span class="btn-label"  aria-hidden="true" ng-show="ctrl.showIcon()"></span>'
                + '<img src="../../Client/Javascripts/Directives/Button/images/loader-mask.gif" class="img-loader" style="width:14px;margin-left:3px" ng-show="ctrl.showLoader()" />'
                + '</button>'
                + actionsMenuTemplate + '</div>';

        }
        else if (attrs.standalone != undefined) {
            return '<div style="position:relative;display:inline-block;height:28px" ng-if="ctrl.isExculdedOnreadOnly()" ng-show="ctrl.showMenuButton()" ng-mouseleave="ctrl.showMenuActions = false"  title="' + buttonTitle + '" '
                + ' aria-label="Left Align" ng-click="ctrl.onInternalClick($event)" ng-disabled="ctrl.isDisabled()" ng-show="ctrl.showMenuButton()" ng-hide="ctrl.hideTemplate">'
                + (buttonAttributes.class != undefined ? '<span  style="font-size:24px"  class="' + buttonAttributes.class + ' btn-label hand-cursor" aria-hidden="true" ng-show="!ctrl.menuActions && ctrl.showIcon()"></span>' : '')
                + ' <span  class="glyphicon  glyphicon-chevron-down btn-label hand-cursor" ng-show="ctrl.menuActions.length > 0 " ></span>'
                + '<img src="../../Client/Javascripts/Directives/Button/images/loader-mask.gif" style="width:20px;margin-top:3px;" class="img-loader" ng-show="ctrl.showLoader()" />'
                + actionsMenuTemplate + '</div>';
        }
        else {
            return '<div style="position:relative;display:inline-block" ng-if="ctrl.isExculdedOnreadOnly()" ng-show="::ctrl.showMenuButton()" ng-mouseleave="ctrl.showMenuActions = false" ng-hide="ctrl.hideTemplate">'
                + '<button style="border-radius: 0px; border-color: transparent;  background-color: transparent; color: #FFF; "  ng-show="ctrl.showMenuButton()" type="button" class="btn btn-default btncustom"'
                + 'aria-label="Left Align" ng-click="ctrl.onInternalClick($event)" ng-disabled="ctrl.isDisabled()">' + buttonAtributesText
                + (buttonAttributes.class != undefined ? '<span ng-show="!ctrl.menuActions && ctrl.showIcon()"  class="' + buttonAttributes.class + ' btn-label"  aria-hidden="true"></span>' : '')
                + ' <span  class="glyphicon  glyphicon-chevron-down btn-label" ng-show="ctrl.menuActions.length > 0 " ></span>'
                +  '<img src="../../Client/Javascripts/Directives/Button/images/loader-mask.gif" class="img-loader" style="width:14px;" ng-show="ctrl.showLoader()" />'
                + '</button>'
                + actionsMenuTemplate + '</div>';

        }
    }

    function getButtonAttributes(type) {
        switch (type) {
            case "Start":
                return {
                    text:"Start",
                    localizedText: "VRRes.Common.Start.VREnd",
                    class: "glyphicon  glyphicon-play"
                };

            case "Reset":
                return {
                    text: "Reset",
                    localizedText: "VRRes.Common.Reset.VREnd",
                    class: "glyphicon  glyphicon-refresh"
                };
            case "Search":
                return {
                    text:"Search",
                    localizedText: "VRRes.Common.Search.VREnd" ,
                    class: "glyphicon  glyphicon-search"
                };
            case "Add":
                return {
                    text:"Add",
                    localizedText: "VRRes.Common.Add.VREnd" ,
                    class: "glyphicon  glyphicon-plus-sign"
                };
            case "Edit":
                return {
                    text: "Edit",
                    class: "glyphicon  glyphicon-edit"
                };
            case "Remove":
                return {
                    text: "Remove",
                    class: "glyphicon  glyphicon-minus-sign"
                };
            case "Save":
                return {
                    text:"Save",
                    localizedText: "VRRes.Common.Save.VREnd" ,
                    class: "glyphicon  glyphicon-floppy-disk"
                };
            case "Yes":
                return {
                    text:"Yes",
                    localizedText: "VRRes.Common.Yes.VREnd",
                    class: "glyphicon  glyphicon-floppy-disk"
                };
            case "Close":
                return {
                    text:"Close",
                    localizedText: "VRRes.Common.Close.VREnd",
                    class: "glyphicon  glyphicon-remove-circle"
                };

            case "Cancel":
                return {
                    text:"Cancel",
                    localizedText: "VRRes.Common.Cancel.VREnd",
                    class: "glyphicon  glyphicon-remove-circle"
                };

            case "CancelChanges":
                return {
                    text: "Cancel Changes",
                    class: "glyphicon  glyphicon-remove-circle"
                };
            case "Cancel All":
                return {
                    text: "Cancel All",
                    class: "glyphicon  glyphicon-remove-circle"
                };

            case "Disable All":
                return {
                    text: "Disable All",
                    class: "glyphicon  glyphicon-remove-circle"
                };

            case "Enable All":
                return {
                    text: "Enable All",
                    class: "glyphicon  glyphicon-play"
                };

            case "Cancel Selected":
                return {
                    text: "Cancel Selected",
                    class: "glyphicon  glyphicon-remove-circle"
                };
            case "No":
                return {
                    text:"No",
                    localizedText: "VRRes.Common.No.VREnd",
                    class: "glyphicon  glyphicon-remove-circle"
                };
            case "Login":
                return {
                    text: "Login",
                    class: "glyphicon  glyphicon-log-in"
                };
            case "BreakInheritance":
                return {
                    text: "Break Inheritance",
                    class: "glyphicon  glyphicon-stop"
                };
            case "AllowInheritance":
                return {
                    text: "Allow Inheritance",
                    class: "glyphicon  glyphicon-play"
                };
            case 'ApplyChanges':
                return {
                    text: 'Apply Changes',
                    class: 'glyphicon glyphicon-ok-circle'
                };
            case 'AssignOrgChart':
                return {
                    text: 'Choose an Organization Chart',
                    class: 'glyphicon glyphicon-list-alt'
                };
            case 'AssignOperators':
                return {
                    text: 'Assign Operators',
                    class: 'glyphicon glyphicon-link'
                };
            case 'AssignCarriers':
                return {
                    text: 'Assign Carriers',
                    class: 'glyphicon glyphicon-link'
                };
            case 'Validate':
                return {
                    text: 'Validate',
                    class: 'glyphicon glyphicon-ok-circle'
                };
            case 'Upload':
                return {
                    text:"Upload",
                    localizedText: "VRRes.Common.Upload.VREnd",
                    class: 'glyphicon glyphicon-upload'
                };
            case 'ManageCountries':
                return {
                    text: 'Manage Countries',
                    class: undefined
                };
            case 'Download':
                return {
                    text: 'Download Template',
                    class: 'glyphicon glyphicon-download'
                };
            case 'DownloadFile':
                return {
                    text: 'Download',
                    class: 'glyphicon glyphicon-download'
                };
            case 'EvaluateResult':
                return {
                    text: 'Evaluate',
                    class: 'glyphicon glyphicon-download'
                };
            case 'Settings':
                return {
                    text: 'Settings',
                    class: 'glyphicon glyphicon-cog'
                };
            case 'Move':
                return {
                    text: 'Move',
                    class: 'glyphicon glyphicon-move'
                };
            case 'Ranking':
                return {
                    text: 'Ranking',
                    class: 'glyphicon glyphicon-move'
                };
            case 'Pricing':
                return {
                    text: 'Pricing',
                    class: "glyphicon  glyphicon-edit"
                };
            case 'Apply':
                return {
                    text: 'Apply',
                    class: "glyphicon glyphicon-ok-circle"
                };

            case 'Split':
                return {
                    text: 'Split',
                    class: "glyphicon glyphicon-resize-full"
                };

            case 'Merge':
                return {
                    text: 'Merge',
                    class: "glyphicon glyphicon-resize-small"
                };
            case 'Compile':
                return {
                    text: 'Compile',
                    class: "glyphicon glyphicon-tasks"
                };
            case 'SelectAll':
                return {
                    text:"Select All",
                    localizedText: "VRRes.Common.SelectAll.VREnd",
                    class: "glyphicon glyphicon-check"
                };
            case 'DeselectAll':
                return {
                    text:"Deselect All",
                    localizedText: "VRRes.Common.DeselectAll.VREnd",
                    class: "glyphicon glyphicon-unchecked"
                };
            case 'Export':
                return {
                    text:"Export",
                    localizedText: "VRRes.Common.Export.VREnd",
                    class: "glyphicon glyphicon-download"
                };
            case "Continue":
                return {
                    text: "Continue",
                    class: "glyphicon  glyphicon-play"
                };
            case "Stop":
                return {
                    text: "Stop",
                    class: "glyphicon  glyphicon-stop"
                };
            case "End":
                return {
                    text: "End",
                    class: "glyphicon  glyphicon-remove-circle"
                };
            case 'Ok':
                return {
                    text: 'Ok',
                    class: 'glyphicon glyphicon-ok-circle'
                };
            case 'Rename':
                return {
                    text: 'Rename',
                    class: 'glyphicon glyphicon-pencil'
                };
            case 'Help':
                return {
                    text:"Help",
                    localizedText: "VRRes.Common.Help.VREnd",
                    class: 'glyphicon glyphicon-question-sign'
                };
            case 'Next':
                return {
                    text: 'Next',
                    class: 'glyphicon glyphicon-chevron-right'
                };
            case 'Back':
                return {
                    text: 'Back',
                    class: 'glyphicon glyphicon-chevron-left'
                };
            case 'Convert':
                return {
                    text: 'Convert',
                    class: 'glyphicon glyphicon-retweet'
                };
            case 'TestConversion':
                return {
                    text: 'Test Conversion',
                    class: "glyphicon glyphicon-check"
                };
            case 'GenerateInvoice':
                return {
                    text:"Generate Invoice",
                    localizedText: "VRRes.Common.GenerateInvoice.VREnd",
                    class: "glyphicon glyphicon-retweet"
                };
            case 'Generate':
                return {
                    text:"Generate",
                    localizedText: "VRRes.Common.Generate.VREnd",
                    class: "glyphicon glyphicon-retweet"
                };
            case 'Analyze':
                return {
                    text: 'Analyze',
                    class: "glyphicon glyphicon-retweet"
                };
            case 'Preview':
                return {
                    text: 'Preview',
                    class: "glyphicon glyphicon-eye-open"
                };
            case 'SendEmail':
                return {
                    text:'Send Email',
                    localizedText: "VRRes.Common.SendEmail.VREnd",
                    class: "glyphicon glyphicon-envelope"
                };
            case 'ExcludeAll':
                return {
                    text: 'Exclude All',
                    class: "glyphicon glyphicon-eject"
                };
            case "Load":
                return {
                    text: "Load",
                    class: "glyphicon glyphicon-search"
                };
            case "Run":
                return {
                    text: "Run",
                    class: "glyphicon glyphicon-play"
                };
            case "CalculateRate":
                return {
                    text: "Calculate Rate",
                    class: "glyphicon glyphicon-play"
                };
            case "Skip":
                return {
                    text: "Skip",
                    class: "glyphicon glyphicon-step-forward"
                };
            case "BulkActions":
                return {
                    text:"Bulk Actions",
                    localizedText: "VRRes.Common.BulkActions.VREnd",
                    class: "glyphicon glyphicon-tasks"
                };
            case "Evaluate":
                return {
                    text: "Evaluate",
                    class: "glyphicon glyphicon-play"
                };
            case "CreateUser":
                return {
                    text: "Create User",
                    class: "glyphicon  glyphicon-plus-sign"
                };
            case "ResetPassword":
                return {
                    text: "Reset Password",
                    class: "glyphicon glyphicon-repeat"
                };
            case "SectionAction":
                return {
                    text: "Actions"
                };
            case "Reject":
                return {
                    text: "Reject",
                    class: "glyphicon  glyphicon-remove-circle"
                };
            case 'SendAll':
                return {
                    text: 'Send All',
                    class: "glyphicon glyphicon-envelope"
                };
            case 'Compare':
                return {
                    text:"Compare",
                    localizedText: "VRRes.Common.Compare.VREnd",
                    class: "glyphicon glyphicon-retweet"
                };

            case "ReleaseAll":
                return {
                    text: "Release All",
                    class: "glyphicon  glyphicon-play"
                };
            case "Delete":
                return {
                    text:"Delete",
                    localizedText: "VRRes.Common.Delete.VREnd",
                    class: "glyphicon glyphicon-trash"
                };
            case "UploadExcel":
                return {
                    text: "Upload Excel File",
                    class: "glyphicon glyphicon-circle-arrow-up"
                };
            case "Exclude":
                return {
                    text: "Exclude",
                    class: "glyphicon glyphicon-ban-circle"
                };
            case "Execute":
                return {
                    text: "Execute",
                    class: "glyphicon glyphicon-play"
                };
            case "Resume":
                return {
                    text: "Resume",
                    class: "glyphicon glyphicon-play"
                };
            case "Pause":
                return {
                    text: "Pause",
                    class: "glyphicon glyphicon-pause"
                };
            case "Personalization":
                return {
                    text: "Personalization",
                    class: "glyphicon glyphicon-user"
                };
            case "Assign":
                return {
                    text: "Assign",
                    class: "glyphicon glyphicon-user"
                };
            case "Take":
                return {
                    text: "Take",
                    class: "glyphicon glyphicon-lock"
                };
            case "Release":
                return {
                    text: "Release",
                    class: "glyphicon glyphicon-send"
                };
            case "Activate":
                return {
                    text: "Activate",
                    class: "glyphicon glyphicon-ok-circle"
                };
            case "Comment":
                return {
                    text: "Comment",
                    class: "glyphicon glyphicon-circle-arrow-right"
                };
            case "TestGenerate":
                return {
                    text: "Test Generate",
                    class: 'glyphicon glyphicon-download'
                };
            case "SaveAndNew":
                return {
                    text: "Save & New",
                    class: "glyphicon  glyphicon-floppy-disk"
                };
        }
    }
}]);