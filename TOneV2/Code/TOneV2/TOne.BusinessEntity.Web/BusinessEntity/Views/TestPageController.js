TestPageController.$inject = ['$scope', 'UsersAPIService', 'UtilsService', 'VRModalService'];

function TestPageController($scope, UsersAPIService, UtilsService, VRModalService) {
  
    defineScope();
    load();

    function defineScope() {
        $scope.users = [];
        $scope.roleList = [];
        $scope.selectedManager = undefined;
        $scope.selectedUsers = [];
        
        function findNode(id) {

            for (var i = 0; i < $scope.roleList.length; i++)
            {
                var node = getNodeRecursively(id, $scope.roleList[i]);
                if (node != null)
                    return node;
            }
            
            return null;
        }

        function getNodeRecursively(id, node)
        {
            if (node.roleId == id)
                return node;
           
            if (node.children != null && node.children.length > 0)
            {
                for (var i = 0; i < node.children.length; i++)
                {
                    var result = getNodeRecursively(id, node.children[i]);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        function removeNode (list, i, removeId) {
            if (i == list.length)
                return;
            else if (list[i].roleId == removeId) {
                list.splice(i, 1)[0];
                return;
            }
            else {
                removeNode(list, i + 1, removeId);

                if (list[i].children.length > 0) {
                    removeNode(list[i].children, 0, removeId);
                }
            }
        }

        function addNode(list, i, role, supervisorId) {
            if (i == list.length)
                return;
            else if (list[i].roleId == supervisorId) {
                list[i].children.push(role);
                return;
            }
            else {
                addNode(list, i + 1, role, supervisorId);

                if (list[i].children.length > 0) {
                    addNode(list[i].children, 0, role, supervisorId);
                }
            }
        }

        function reIndexNode(userIds, managerId) {

            for (var i = 0; i < userIds.length; i++)
            {
                var userNode = findNode(userIds[i]);

                // remove the node from the tree
                removeNode($scope.roleList, 0, userNode.roleId);

                // add the node to the supervisor
                addNode($scope.roleList, 0, userNode, managerId);
            }
        };

        function getUserIds() {
            var ids = [];

            for (var i = 0; i < $scope.selectedUsers.length; i++) {
                ids.push($scope.selectedUsers[i].UserId);
            }

            return ids;
        }

        function validate() {
            //var current = $scope.tree.currentNode;
            //var supervisorId = $scope.supervisorId;
            //var options = $scope.selectedUsers;

            //if (!current) {
            //    alert('Select a member');
            //    return false;
            //}

            //if (supervisorId == 'Supervisor' && options.length == 0) {
            //    alert('Select a supervisor and/or members');
            //    return false;
            //}

            //if (current.roleId == supervisorId) {
            //    alert('Invalid supervisor');
            //    return false;
            //}

            //return true;
        }

        $scope.apply = function () {
            var current = $scope.tree.currentNode;

            if ($scope.selectedUsers.length) {
                var userIds = getUserIds();
                reIndexNode(userIds, current.roleId);
            }

            if ($scope.selectedManager != undefined) {
                var userIds = [current.roleId];
                reIndexNode(userIds, $scope.selectedManager.UserId);
            }
        };

        $scope.test = function () {
            var settings = {
                useModalTemplate: true,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Testing VR-Code-Editor";

                modalScope.onModalClosed = function () {

                };
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/TestPageEditor.html', null, settings);
        }
    };

    function load() {
        UsersAPIService.GetUsers().then(function (response) {
            $scope.users = response;

            var setRoleList = function () {
                // set $scope.roleList
                for (var i = 0; i < $scope.users.length; i++) {
                    var role = {
                        roleName: $scope.users[i].Name,
                        roleId: $scope.users[i].UserId,
                        children: []
                    }

                    $scope.roleList.push(role);
                }
            };
            setRoleList();
        });

        $scope.$watch('tree.currentNode', function (newObj, oldObj) {
            if ($scope.tree && angular.isObject($scope.tree.currentNode)) {
                // empty the selected users
                $scope.selectedUsers.length = 0;
                console.log($scope.selectedUsers);

                for (var i = 0; i < newObj.children.length; i++) {
                    var index = UtilsService.getItemIndexByVal($scope.users, newObj.children[i].roleId, 'UserId');
                    $scope.selectedUsers.push($scope.users[index]);
                }
            }
        }, false);

        loadCode();
    }

    function loadCode() {
        $scope.customCode = '<script>document.write("Hello world!");</script>';
    }
}

appControllers.controller('BusinessEntity_TestPageController', TestPageController);
