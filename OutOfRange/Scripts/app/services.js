(function () {
    'use strict';

    var services = angular.module('app', []);//.factory('services', services);

    services.factory('Auth', ['$resource', function ($resource) {
        var Register = $resource('api/account/register');
        return {
            register: function (user) {
                var _user = new Register(user);
                _user.$save();
            }
        }
    }])

})();