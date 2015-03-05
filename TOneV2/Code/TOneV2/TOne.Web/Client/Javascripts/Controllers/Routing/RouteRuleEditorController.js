appControllers.controller('RouteRuleEditorController',
    function RouteRuleEditorController($scope,$http) {
        
        $('.ddl').dropdown();
        $('.menuddl').dropdown({
            on: 'hover'
        });
        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
           {
               params: {
                   carrierType:1
               }
           })
       .success(function (response) {
           $scope.customers = response;
       });
        $scope.selectedCustomers = [];
        $scope.selectCustomer = function (c) {
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
        $scope.selectSupplier = function (s) {
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

    });