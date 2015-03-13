appControllers.controller('ZoneController',
    function ZoneController($scope, $http) {
        var dropdownHidingTimeoutHandler;
        $('.dropdown').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('.dropdown').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });

        $('.dropdown-custom').on('mouseenter', function () {
            var $this = $(this);
            clearTimeout(dropdownHidingTimeoutHandler);
            if (!$this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
            }
        });

        $('.dropdown-custom').on('mouseleave', function () {
            var $this = $(this);
            dropdownHidingTimeoutHandler = setTimeout(function () {
                if ($this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            }, 150);
        });
        $scope.selectedZones = [];
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
        $scope.filterzone = '';
        $scope.zones = [];
        $scope.showloading = false;
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
        // $scope.texttest = {label:''};
       
    });