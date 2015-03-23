appControllers.controller('ZoneController',
    function ZoneController($scope, $http) {
        $scope.filterzone = '';
        $scope.zones = [];
        $scope.showloading = false;
        $scope.selectedZones = [];
        $scope.code = "";
        $scope.codeList = [];
        $scope.codeInpute = '';
        if ($scope.routeRule != null && $scope.routeRule.CodeSet.ZoneIds != undefined) {
            $scope.codeList = $scope.routeRule.CodeSet.ExcludedCodes;
            $scope.zoneSelectionOption = $scope.routeRule.CodeSet.ZoneIds.SelectionOption;
            $http.get($scope.baseurl + "/api/BusinessEntity/GetZoneList",
              {
                  params: {
                      ZonesIds: $scope.routeRule.CodeSet.ZoneIds.SelectedValues
                  }
              })
              .success(function (response) {
                  $scope.selectedZones = response;

              });

        }
        else {
            $scope.zoneSelectionOption = 1;
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
            $.each($scope.selectedZones, function (i, value) {
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
        $('#dropdownZones').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#dropdownZones').on('hide.bs.dropdown', function (e) {
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
        $scope.getSelectZoneText = function () {
            var label;
            if ($scope.selectedZones.length == 0)
                label = "Select Zones...";
            else if ($scope.selectedZones.length == 1)
                label = $scope.selectedZones[0].Name;
            else if ($scope.selectedZones.length == 2)
                label = $scope.selectedZones[0].Name + "," + $scope.selectedZones[1].Name;
            else if ($scope.selectedZones.length == 3)
                label = $scope.selectedZones[0].Name + "," + $scope.selectedZones[1].Name + "," + $scope.selectedZones[2].Name;
            else
                label = $scope.selectedZones.length + " Zones selected";
            if (label.length > 21)
                label = label.substring(0, 20) + "..";
            return label;
        };

        // zones live search
        
        $scope.searchZones = function () {
            $scope.zones.length = 0;
            if ($scope.filterzone.length > 1) {
                $scope.showloading = true;

                $http.get($scope.baseurl + "/api/BusinessEntity/GetSalesZones",
                {
                    params: {
                        nameFilter: $scope.filterzone
                    }
                })
            .success(function (response) {
                $scope.zones = response;
                $scope.showloading = false;

            });
            }

        }

        $scope.selectZone = function ($event, s) {
            $event.preventDefault();
            $event.stopPropagation();
            var index = $scope.findExsite($scope.selectedZones, s.ZoneId, 'ZoneId');

            if (index >= 0) {
                $scope.selectedZones.splice(index, 1);
            }
            else
                $scope.selectedZones.push(s);
        };

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