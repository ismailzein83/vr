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
        $scope.typeTemplates = [
           { name: 'Customer', url: '/Client/Templates/PartialTemplate/CustomerTemplate.html' },
           { name: 'Pool', url: '/Client/Templates/PartialTemplate/PoolTemplate.html' },
           { name: 'Product', url: '/Client/Templates/PartialTemplate/ProductTemplate.html' }
        ]
        $scope.editortype = $scope.editorTemplates[0];
        $scope.routeTemplates = [
          { name: 'Customer', url: '/Client/Templates/PartialTemplate/CustomerTemplate.html' },
          { name: 'Pool', url: '/Client/Templates/PartialTemplate/PoolTemplate.html' },
          { name: 'Product', url: '/Client/Templates/PartialTemplate/ProductTemplate.html' }
        ]
        $scope.routetype = $scope.routeTemplates[0];
        
       

    });

