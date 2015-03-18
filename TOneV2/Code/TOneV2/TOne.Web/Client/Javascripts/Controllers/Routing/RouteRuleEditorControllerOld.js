appControllers.controller('RouteRuleEditorControllerOld',
    function RouteRuleEditorControllerOld($scope, $http) {
       
        $('.ddl').dropdown({
            onShow: function (e) {
               
            }
        });
        $('.menuddl').dropdown({
            on: 'hover'
        });       
        $scope.selectedCustomers = [];
        $scope.selectCustomer = function ($event, c) {
            $event.preventDefault();
            $event.stopPropagation();
            var index = null;
            try {
                var index = $scope.selectedCustomers.indexOf(c);
            }
            catch (e) {

            }
            if (index >= 0) {
                $scope.selectedCustomers.splice(index, 1);
            }
            else
                $scope.selectedCustomers.push(c);
        };
        $scope.getSelectCustomerText = function () {
            var label;
            if ($scope.selectedCustomers.length == 0)
                label = "Select Customers...";
            else if( $scope.selectedCustomers.length == 1)
                label = $scope.selectedCustomers[0].Name;
            else if( $scope.selectedCustomers.length == 2)
                label = $scope.selectedCustomers[0].Name + "," + $scope.selectedCustomers[1].Name;
            else if ($scope.selectedCustomers.length == 3)
                label = $scope.selectedCustomers[0].Name + "," + $scope.selectedCustomers[1].Name + "," + $scope.selectedCustomers[2].Name;
            else
                label = $scope.selectedCustomers.length + " Customers selected";
            if (label.length > 21)
                label = label.substring(0, 20) + "..";
            return label;
        };
        $scope.getSelectSuppliersText = function () {
            var label;
            if ($scope.selectedSuppliers.length == 0)
                label = "Select Suppliers...";
            else if ($scope.selectedSuppliers.length == 1)
                label = $scope.selectedSuppliers[0].Name;
            else if ($scope.selectedSuppliers.length == 2)
                label = $scope.selectedSuppliers[0].Name + "," + $scope.selectedSuppliers[1].Name;
            else if ($scope.selectedSuppliers.length == 3)
                label = $scope.selectedSuppliers[0].Name + "," + $scope.selectedSuppliers[1].Name + "," + $scope.selectedSuppliers[2].Name;
            else
                label = $scope.selectedSuppliers.length + " Suppliers selected";
            if (label.length > 21)
                label = label.substring(0, 20) + "..";
            return label;
        };
        
        // 
        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
          {
              params: {
                  carrierType: 2
              }
          })
      .success(function (response) {
          $scope.suppliers = response;
      });
        $scope.selectedSuppliers = [];
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
        $scope.selectSupplier = function ($event, s) {
            $event.preventDefault();
            $event.stopPropagation();
            var index = null;
            try {
                var index = $scope.selectedSuppliers.indexOf(s);
            }
            catch (e) {

            }
            if (index >= 0) {
                $scope.selectedSuppliers.splice(index, 1);
            }
            else
                $scope.selectedSuppliers.push(s);
        };

        $scope.selectedZones = [];
        $scope.Zones = [];
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
        $scope.selectedZones = [];
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
        
        $scope.selectZone = function ($event,s) {
            $event.preventDefault();
            $event.stopPropagation();
           // var index = null;
           // try {
            var index = $scope.findExsite($scope.selectedZones, s.ZoneId, 'ZoneId');
               // JSON.parse(angular.toJson($scope.selectedZones)).indexOf(JSON.parse(s));

               //;

           // }
           // catch (e) {

            // }
               console.log(s)
            if (index >= 0) {
                $scope.selectedZones.splice(index, 1);
            }
            else
                $scope.selectedZones.push(s);
        };


        $scope.findExsite = function (arr, value, attname) {
            var index = -1;
            for (var i = 0; i < arr.length; i++) {
                if (arr[i][attname] == value) {
                    index = i
                }
            }
            return index;
        }
    });