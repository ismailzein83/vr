(function (app) {

    "use strict";

    vrDropDownSection.$inject = ['$timeout', 'BaseDirService', 'VRValidationService', 'UtilsService', 'MultiTranscludeService'];

    function vrDropDownSection($timeout, BaseDirService, VRValidationService, UtilsService, MultiTranscludeService) {

        return {
            restrict: 'E',
            transclude: true,
            scope: {
                onReady: '=',
                defaultstate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.id = BaseDirService.generateHTMLElementName();
                ctrl.menuid = BaseDirService.generateHTMLElementName();
                ctrl.initializeSection = function () {
                    ctrl.showmenu = true;
                    calculatePosition(ctrl, true);
                    addBackDrop();
                    setTimeout(function () {

                        checkOnExpandMethode();

                        $('#' + ctrl.menuid).slideDown("slow");
                    }, 1000);

                };

                $element.on('$destroy', function () {
                    fixDropdownSectionPosition();
                });
                ctrl.expandSection = function (e) {
                    if (e != undefined && $(e.target).hasClass('vanrise-inpute')) {
                        return UtilsService.convertToPromiseIfUndefined(undefined);
                    }
                    var expandSectionDeferred = UtilsService.createPromiseDeferred();
                    setTimeout(function () {
                        $scope.$apply(function () {
                            ctrl.showmenu = true;
                            calculatePosition(ctrl);
                        });
                        $('#' + ctrl.menuid).slideDown("slow");
                        addBackDrop();
                        expandSectionDeferred.resolve();
                        checkOnExpandMethode();
                    }, 1000);
                    return expandSectionDeferred.promise;
                };
                ctrl.collapseSection = function () {
                    checkOnCollapseMethode();
                    $('#' + ctrl.menuid).slideUp("slow", function () {
                        fixDropdownSectionPosition();
                    });
                };
                ctrl.loadSection = function () {
                    $timeout(function () {
                        if (ctrl.defaultstate == true) {
                            ctrl.initializeSection();
                        }
                    }, 1000)
                };
                $('#' + ctrl.id).parents('div').scroll(function () {
                    fixDropdownSectionPosition();
                });
                $(window).scroll(function () {
                    fixDropdownSectionPosition();
                });
                $(window).resize(function () {
                    fixDropdownSectionPosition();
                });
                $(document).resize(function () {
                    fixDropdownSectionPosition();
                });
                $(document).scroll(function () {
                    fixDropdownSectionPosition();
                });
                $scope.$on('start-drag', function (event, args) {
                    fixDropdownSectionPosition();
                });
                var fixDropdownSectionPosition = function () {
                    ctrl.showmenu = false;
                    $('.vr-backdrop').remove();
                    $('.vr-backdrop-modal').remove();
                };

                function checkOnExpandMethode() {
                    if ($attrs.onexpandsection != undefined) {
                        var onExpandSectiondMethod = $scope.$parent.$eval($attrs.onexpandsection);
                        if (onExpandSectiondMethod != undefined && typeof (onExpandSectiondMethod) == 'function') {
                            onExpandSectiondMethod();
                        }
                    }
                }
                function checkOnCollapseMethode() {
                    if ($attrs.oncollapsesection != undefined) {
                        var onCollapseSectiondMethod = $scope.$parent.$eval($attrs.oncollapsesection);
                        if (onCollapseSectiondMethod != undefined && typeof (onCollapseSectiondMethod) == 'function') {
                            onCollapseSectiondMethod();
                        }
                    }
                }

                function addBackDrop() {
                    if ($element.parents().find('.modal-dialog').length > 0)
                        $('vr-modalbody').last().prepend("<div class='vr-backdrop-modal'></div>");
                    else {
                        $($element.parents().find("vr-form")).prepend("<div class='vr-backdrop'></div>");
                    }
                }

            },

            compile: function (element, attrs) {
                return {
                    post: function postLink(scope, elem, attr, ctrl, transcludeFn) {

                        MultiTranscludeService.transclude(elem, transcludeFn);

                        var api = {};

                        api.collapse = function () {
                            ctrl.collapseSection();
                        };

                        api.expand = function () {
                            return ctrl.expandSection(undefined);
                        };

                        if (ctrl.onReady != null)
                            ctrl.onReady(api);
                    }
                }
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                var startTemplate = '<div id="rootDiv">';
                var endTemplate = '</div>';

                var sectionTemplate = '<div style="position: relative;" >'
                            + '<vr-validation-group validationcontext="ctrl.validationContext">'
                            + '<div class="vr-dropdown-section" id="{{ctrl.id}}" >'
                                + '<div class="summary-container hand-cursor"  ng-click="ctrl.expandSection($event)" ng-class="ctrl.validationContext.validate() != null  ? \'invalid-section\':\'\' " >'
                                    + '<div transclude-id="summary" class="summary-main"></div>'
                                    + '<span  class="glyphicon  glyphicon-sort-by-attributes  toogle-icon" ng-init="ctrl.loadSection()"></span>'
                                    + '<span ng-if="ctrl.validationContext.validate() != null" class="validation-sign"  title="has validation errors!">*</span>'
                              + '</div>'
                                + '<ul class="dropdown-menu drop-down-section"   id="{{ctrl.menuid}}" ng-show="ctrl.showmenu" ng-class="ctrl.validationContext.validate() != null  ? \'invalid-section\':\'\' "  >'
                                    + '<li role="presentation" >'
                                        + '<span class="hand-cursor glyphicon  glyphicon-remove remove-icon" ng-click="ctrl.collapseSection($event,false)" ></span>'
                                        + '<div class="section-body" transclude-id="body" ng-click="ctrl.muteAction($event);"></div>'
                                    + '</li>'
                                + '</ul>'
                            + '</vr-validation-group>'
                        + '</div>';

                ;

                return startTemplate + sectionTemplate + endTemplate;
            }

        };
        function calculatePosition(ctrl, forcedisplay) {
            var self = $('#' + ctrl.id).find('.summary-container');

            var selfHeight = $(self).parent().height();
            var selfOffset = $(self).offset();
            var dropDown = $('#' + ctrl.menuid);
            var top = 0;
            var basetop = selfOffset.top - $(window).scrollTop();
            var baseleft = selfOffset.left - $(window).scrollLeft();

            var height = ctrl.bodysectionheight;
            if (innerHeight - basetop < height + 100)
                top = basetop - (height);
            else
                top = selfOffset.top - $(window).scrollTop() - 2;
            var style = {
                position: 'fixed',
                top: top,
                left: baseleft,
                width: self.parent().width()
            };
            if (forcedisplay == true)
                style.display = 'block';
            $(dropDown).css(style);
        };
    }

    app.directive('vrDropdownSection', vrDropDownSection);

})(app);