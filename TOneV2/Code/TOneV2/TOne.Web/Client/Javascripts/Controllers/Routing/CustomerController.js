appControllers.controller('CustomerController',
    function CustomerController($scope, $http) {
        
        $scope.isvalidCompCus = function () {           
            return ( $scope.selectedCustomers.length > 0) ? "" : "required-inpute";
        }
        
        $scope.customers = [];
        $scope.selectedCustomers = [];
        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
         {
             params: {
                 carrierType: 1
             }
         })
         .success(function (response) {
             $scope.customers = response;          
             if ($scope.routeRule != null) {
                 var tab = [];
                 $.each($scope.routeRule.CarrierAccountSet.Customers.SelectedValues, function (i, value) {
                     var existobj = $scope.findExsiteObj($scope.customers, value, 'CarrierAccountID')
                     if (existobj != null)
                         tab[i] = existobj;

                 });
                 $scope.selectedCustomers = tab;
                 $scope.carrierAccountSelectionOption = $scope.routeRule.CarrierAccountSet.Customers.SelectionOption;
             }
             else {
                 $scope.carrierAccountSelectionOption = 1;
             }
         });

        $scope.subViewConnector.getCarrierAccountSet = function () {
            return {
                $type: "TOne.LCR.Entities.CustomerSelectionSet, TOne.LCR.Entities",
                Customers: {
                    SelectionOption: ($scope.carrierAccountSelectionOption == 1) ? "OnlyItems" : "AllExceptItems",
                    SelectedValues: $scope.getSelectedValues()
                }
            }
        }
        $scope.getSelectedValues = function () {
            var tab = [];
            $.each($scope.selectedCustomers, function (i, value) {
                tab[i] = value.CarrierAccountID;

            });
            return tab;
        }

      
        var dropdownHidingTimeoutHandlerc;
        $('#dropdownMenuddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#dropdownMenuddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });

        $('.dropdown-custom').on('mouseenter', function () {
            var $this = $(this);
            clearTimeout(dropdownHidingTimeoutHandlerc);
            if (!$this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
            }
        });
        $('.dropdown-custom').on('mouseleave', function () {
            var $this = $(this);
            dropdownHidingTimeoutHandlerc = setTimeout(function () {
                if ($this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            }, 150);
        });
       
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
            else if ($scope.selectedCustomers.length == 1)
                label = $scope.selectedCustomers[0].Name;
            else if ($scope.selectedCustomers.length == 2)
                label = $scope.selectedCustomers[0].Name + "," + $scope.selectedCustomers[1].Name;
            else if ($scope.selectedCustomers.length == 3)
                label = $scope.selectedCustomers[0].Name + "," + $scope.selectedCustomers[1].Name + "," + $scope.selectedCustomers[2].Name;
            else
                label = $scope.selectedCustomers.length + " Customers selected";
            if (label.length > 21)
                label = label.substring(0, 20) + "..";
            return label;
        };
       
    });