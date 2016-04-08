﻿'use strict';

app.service('ButtonDirService', ['BaseDirService', function (BaseDirService) {

    return ({
        getTemplate: getTemplate
    });

    function getTemplate(attrs) {

        var actionsMenuTemplate = ''
          + '<ul role="menu" class="dropdown-menu gid-cell-menu am-fade-and-slide-top" ng-show="ctrl.showMenuActions" ng-style="{\'position\': \'absolute\', \'top\': \'18px\'} " >'
       + ' <li role="presentation">'
       + '     <div ng-repeat="action in ctrl.menuActions"  ng-hide="action.disable" class="mark-select " style="padding-left: 2px; ">'
         + '       <div class=" hand-cursor" ng-click="ctrl.menuActionClicked(action)"><span style="font-size:11px">{{action.name}}</span>'
         + '<img src="../../Client/Javascripts/Directives/Button/images/loader-mask.gif" style="width:14px;margin-left:3px" ng-show="action.isSubmitting" /></div>'
        + '    </div>'
       + ' </li>'
 + '   </ul>';

        var type = attrs.type;

        var buttonAttributes = getButtonAttributes(type);
        if (type == "Login") {
            return '<div style="position:relative;display:inline-block;width:100%" ng-mouseleave="ctrl.showMenuActions = false" ng-hide="ctrl.hideTemplate">'
                + '<button style="width:100%" type="button" class="btn btn-danger login-btn"'
            + 'aria-label="Left Align" ng-click="ctrl.onInternalClick($event)" ng-disabled="ctrl.isDisabled()">' + buttonAttributes.text
                + '<span style="padding-left:4px"  aria-hidden="true" ng-show="ctrl.showIcon()"></span>'
                + '<img src="../../Client/Javascripts/Directives/Button/images/loader-mask.gif" style="width:14px;margin-left:3px" ng-show="ctrl.showLoader()" />'
                + '</button>'
                + actionsMenuTemplate + '</div>';

        }
        else if (attrs.standalone != undefined) {
            return '<div style="position:relative;display:inline-block" ng-mouseleave="ctrl.showMenuActions = false"  title="' + buttonAttributes.text + '" '
           + ' aria-label="Left Align" ng-click="ctrl.onInternalClick($event)" ng-disabled="ctrl.isDisabled()" ng-hide="ctrl.hideTemplate">'
               + '<span style="padding-left:4px;font-size:24px" class="' + buttonAttributes.class + ' hand-cursor" aria-hidden="true" ng-show="ctrl.showIcon()"></span>'
               + '<img src="Client/Javascripts/Directives/Button/images/loader-mask.gif" style="width:14px;margin-left:3px" ng-show="ctrl.showLoader()" />'
                + actionsMenuTemplate + '</div>';
        }
        else {
            return '<div style="position:relative;display:inline-block" ng-mouseleave="ctrl.showMenuActions = false" ng-hide="ctrl.hideTemplate">'
                + '<button style="border-radius: 0px; border-color: transparent;  background-color: transparent; color: #FFF; " type="button" class="btn btn-default btncustom"'
            + 'aria-label="Left Align" ng-click="ctrl.onInternalClick($event)" ng-disabled="ctrl.isDisabled()">' + buttonAttributes.text
                + '<span style="padding-left:4px" class="' + buttonAttributes.class + ' aria-hidden="true" ng-show="ctrl.showIcon()"></span>'
                + '<img src="Client/Javascripts/Directives/Button/images/loader-mask.gif" style="width:14px;margin-left:3px" ng-show="ctrl.showLoader()" />'

                + '</button>'
                + actionsMenuTemplate + '</div>';

        }
    }

    function getButtonAttributes(type) {
        switch (type) {
            case "Start":
                return {
                    text: "Start",
                    class: "glyphicon  glyphicon-play"
                };

            case "Reset":
                return {
                    text: "Reset",
                    class: "glyphicon  glyphicon-refresh"
                };
            case "Search":
                return {
                    text: "Search",
                    class: "glyphicon  glyphicon-search"
                };
            case "Add":
                return {
                    text: "Add",
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
                    text: "Save",
                    class: "glyphicon  glyphicon-floppy-disk"
                };
            case "Yes":
                return {
                    text: "Yes",
                    class: "glyphicon  glyphicon-floppy-disk"
                };
            case "Close":
                return {
                    text: "Close",
                    class: "glyphicon  glyphicon-remove-circle"
                };

            case "Cancel":
                return {
                    text: "Cancel",
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

            case "Cancel Selected":
                return {
                    text: "Cancel Selected",
                    class: "glyphicon  glyphicon-remove-circle"
                };
            case "No":
                return {
                    text: "No",
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
                    text: 'Upload',
                    class: 'glyphicon glyphicon-upload'
                };
            case 'SellNewCountries':
                return {
                    text: 'Sell New Countries',
                    class: 'glyphicon  glyphicon-plus-sign'
                };
            case 'Download':
                return {
                    text: 'Download Template',
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
                }
            case 'Ranking':
                return {
                    text: 'Ranking',
                    class: 'glyphicon glyphicon-move'
                }
            case 'Pricing':
                return {
                    text: 'Pricing',
                    class: "glyphicon  glyphicon-edit"
                }
            case 'Apply':
                return {
                    text: 'Apply',
                    class: "glyphicon glyphicon-ok-circle"
                }

            case 'Split':
                return {
                    text: 'Split',
                    class: "glyphicon glyphicon-resize-full"
                }

            case 'Merge':
                return {
                    text: 'Merge',
                    class: "glyphicon glyphicon-resize-small"
                }
            case 'Compile':
                return {
                    text: 'Compile',
                    class: "glyphicon glyphicon-tasks"
                }


            case 'SelectAll':
                return {
                    text: 'Select All',
                    class: "glyphicon glyphicon-check"
                }
            case 'Export':
                return {
                    text: 'Export',
                    class: "glyphicon glyphicon-download"
                }
            case "Continue":
                return {
                    text: "Continue",
                    class: "glyphicon  glyphicon-play"
                }
            case "Stop":
                return {
                    text: "Stop",
                    class: "glyphicon  glyphicon-stop"
                }
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
                    text: 'Help',
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

               
        } 
    }
}]);