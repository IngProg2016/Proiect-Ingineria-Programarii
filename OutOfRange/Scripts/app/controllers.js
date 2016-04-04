(function () {
    'use strict';

    var controllers = angular
        .module('OutOfRangeApp',[]);
        //.controller('controllers', controllers);

    controllers.controller('MainCtrl', ['$scope', function ($scope) {
        
    }]);

    controllers.controller('RegisterCtrl', ['$scope', 'Auth', function ($scope, Auth) {
        $scope.user = {
            Email: "radu.niculcea@gmail.com",
            Password: "PENSETA1",
            ConfirmPassword: "PENSETA1"
        }
        ;
        $scope.register = function () {
            Auth.register($scope.user);
        };
    }])

})();
