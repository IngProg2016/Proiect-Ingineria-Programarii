(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.controller('MainCtrl', ['$scope', function ($scope) {
        
    }]);

    OutOfRangeApp.controller('RegisterCtrl', ['$scope', 'Auth', function ($scope, Auth) {
        $scope.user = {
            email: "test.user@gmail.com",
            password: "testP@ss1",
            confirmPassword: "testP@ss1"
        };

        $scope.register = function () {
            Auth.register($scope.user);
        };
    }])

})();
