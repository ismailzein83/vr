appControllers.controller('RoutingRules_ZoneTemplateController',
    function ZoneController($scope, $http, ZonesService) {
        $scope.optionsZones = {
            selectedvalues: [],
            datasource: []
        };
        $scope.code = "";
        $scope.codeList = [];
        $scope.codeInpute = ''; 
        if ($scope.routeRule != null && $scope.routeRule.CodeSet.ZoneIds != undefined && $scope.routeRule.CodeSet.$type == 'TOne.LCR.Entities.ZoneSelectionSet, TOne.LCR.Entities') {
            $scope.codeList = $scope.routeRule.CodeSet.ExcludedCodes;
            $scope.zoneSelectionOption = $scope.routeRule.CodeSet.ZoneIds.SelectionOption;
            ZonesService.getZoneList($scope.routeRule.CodeSet.ZoneIds.SelectedValues)
              .then(function (response) {
                  $scope.optionsZones.selectedvalues = response;

              })           
        }
        else {
            $scope.zoneSelectionOption = 1;
            $scope.codeList = [];
        }
        $scope.subViewConnector.getCodeSet = function () {
            return {
                $type: "TOne.LCR.Entities.ZoneSelectionSet, TOne.LCR.Entities",
                ZoneIds:{
                    SelectionOption: ($scope.zoneSelectionOption==1)?"OnlyItems":"AllExceptItems",
                    SelectedValues: $scope.getselectedoption()                   
                },
                ExcludedCodes: $scope.codeList
            };
        }
        $scope.getselectedoption = function () {
            var tab = [];
            $.each($scope.optionsZones.selectedvalues, function (i, value) {
                tab[i] = value.ZoneId;
                           
            });
            return tab ;
        }
        var dropdownHidingTimeoutHandlerz;
        $('#Zoneddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#Zoneddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
       
        $('.dropdown-custom').on('mouseenter', function () {
            var $this = $(this);
            clearTimeout(dropdownHidingTimeoutHandlerz);
            if (!$this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
            }
        });

        $('.dropdown-custom').on('mouseleave', function () {
            var $this = $(this);
            dropdownHidingTimeoutHandlerz = setTimeout(function () {
                if ($this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            }, 150);
        });
      

        // zones live search
        
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }

        $('#CodeListddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#CodeListddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
       
        $scope.getCodes = function () {
            var label = '';
            if ($scope.codeList.length == 0)
                label = "Fill codes...";
            else if ($scope.codeList.length == 1) {
                label += $scope.codeList[0];
            }
            else if ($scope.codeList.length < 5) {
                $.each($scope.codeList, function (i, value) {
                    if (i < $scope.codeList.length - 1)
                        label += value + ',';
                    else
                        label += value;
                });
            }
            else
                label = $scope.codeList.length + " Codes selected";
            // RouteRule.CodeSet.ExcludedCodes = $scope.codeList;
            return label;
        };

        $scope.addCodeEnter = function (e) {

            if (e.keyCode == 13) {
                $scope.addCode(e);
            }
        }
        $scope.addCode = function (e) {
            e.preventDefault();
            e.stopPropagation();
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
       
    });