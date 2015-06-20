
var CodeController = function ($scope, $http) {
    defineScopeObjects();
    defineScopeMethods();
    load();

    function defineScopeObjects() {
        $scope.code = "";
        $scope.codeList = [];
        $scope.codeInpute = '';
    }
    function defineScopeMethods() {
        var numberReg = /^\d+$/;
        $scope.isNumber = function (s) {
            return String(s).search(numberReg) != -1
        };

        $scope.muteAction =  function (e) {
            e.preventDefault();
            e.stopPropagation();
        }
        $scope.getCodes = function () {
            var label = '';
            if ($scope.codeList.length == 0)
                label = "Fill codes...";
            else if ($scope.codeList.length == 1) {
                label += $scope.codeList[0];
            }
            else if ($scope.codeList.length < 3) {
                $.each($scope.codeList, function (i, value) {
                    if (i < $scope.codeList.length - 1)
                        label += value + ',';
                    else
                        label += value;
                });
            }
            else
                label = $scope.codeList.length + " Codes selected";
            return label;
        };

        $scope.addCodeEnter = function (e) {
            if (e.keyCode == 13) {
                $scope.addCode(e);
            }
        }
        $scope.addCode = function (e) {
            var valid = $scope.isNumber($scope.codeInpute);
            if (valid) {
                var index = null;
                var index = $scope.codeList.indexOf($scope.codeInpute);
                if (index >= 0) {
                    $scope.codeInpute = '';
                    return;
                }
                else {
                    $scope.codeList.push($scope.codeInpute);
                    $scope.codeInpute = '';
                }

            }
            else {
                $scope.codeInpute = '';
            }
        }
        $scope.removeCode = function (e, s) {
            e.preventDefault();
            e.stopPropagation();
            var index = $scope.codeList.indexOf(s);
            $scope.codeList.splice(index, 1);
        }
        $scope.subViewConnector.getCodeSet = function () {
            return {
                $type: "TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities",
                Code: $scope.code,
                WithSubCodes: ($scope.subCodes == true) ? true : false,
                ExcludedCodes: $scope.codeList
            };
        };

    }

    function load() {
        $('#CodeListddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#CodeListddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
        $('div[id="CodeListddl"]').on('click', '.dropdown-toggle', function (event) {

            var self = $(this);
            var selfHeight = $(this).parent().height();
            var selfWidth = $(this).parent().width();
            var selfOffset = $(self).offset();
            var selfOffsetRigth = $(document).width() - selfOffset.left - selfWidth;
            var dropDown = self.parent().find('ul');
            $(dropDown).css({ position: 'fixed', top: selfOffset.top + selfHeight, left: 'auto' });
        });

        var fixDropdownPosition = function () {
            $('.drop-down-inside-modal').find('.dropdown-menu').hide();
            $('.drop-down-inside-modal').removeClass("open");

        };

        $(".modal-body").unbind("scroll");
        $(".modal-body").scroll(function () {
            fixDropdownPosition();
        });
        if ($scope.routeRule != null && $scope.routeRule.CodeSet.Code != null && $scope.routeRule.CodeSet.$type == 'TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities') {
            $scope.codeList = $scope.routeRule.CodeSet.ExcludedCodes;
            $scope.code = $scope.routeRule.CodeSet.Code;
            $scope.subCodes = $scope.routeRule.CodeSet.WithSubCodes;
        }
        else {
            $scope.zoneSelectionOption = 1;
            $scope.codeList = [];
            $scope.code = '';
            $scope.subCodes = false;
        }
    }
}

CodeController.$inject = ['$scope', '$http'];
appControllers.controller('RoutingRules_CodeTemplateController', CodeController)


