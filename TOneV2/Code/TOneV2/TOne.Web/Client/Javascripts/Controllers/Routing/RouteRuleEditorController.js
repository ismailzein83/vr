appControllers.controller('RouteRuleEditorController',
    function RouteRuleEditorController($scope,$http) {
       
        $('.dropdown').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

         //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('.dropdown').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });
        
        var dropdownHidingTimeoutHandler;

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
        $scope.muteAction = function (e) {
            e.preventDefault();
            e.stopPropagation();
        }
        $scope.update = function (val,model) {            
            $scope[model] = val;
        }       
        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
           {
               params: {
                   carrierType:1
               }
           })
       .success(function (response) {
           $scope.customers = response;
       });
        $scope.ruletype = { name: 'Select ..', url: '' }
        $scope.templates = [  
            { name: 'Override Route', url: '/Client/Templates/PartialTemplate/RouteOverrideTemplate.html' },
            { name: 'Block Route', url: '' },
            { name: 'Priority Rule', url: '/Client/Templates/PartialTemplate/PriorityTemplate.html' }
        ]
       
        $scope.editorTemplates = [
            { name: 'Zone', url: '/Client/Templates/PartialTemplate/ZoneTemplate.html' },
            { name: 'Code', url: '/Client/Templates/PartialTemplate/CodeTemplate.html' }
        ]
        $scope.editortype = $scope.editorTemplates[0];
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
       

    });

